using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Controls an enemy that uses a NavMeshAgent to chase the player,
/// but only when the player is on the same ground platform as the enemy.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    // --- Public Variables ---
    [Header("Targeting")]
    [Tooltip("A reference to the player's transform.")]
    public Transform player;

    [Header("Platform Detection")]
    [Tooltip("The layermask used to identify ground platforms.")]
    public LayerMask groundLayer;
    [Tooltip("How far down to check for a platform beneath the AI and player.")]
    public float groundCheckDistance = 1.5f;

    public enum AIState
    {
        passive,
        chase

    };
    [Header("AI State")]
    public AIState aiState;

    // --- Private Variables ---
    private NavMeshAgent navMeshAgent;
    private float growlCooldown = 6;
    private float growlStartTime = 0;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        // For robustness, if the player isn't assigned in the Inspector, try to find them by tag.
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("EnemyMovement: Cannot find GameObject with 'Player' tag and player transform is not assigned. AI will be inactive.");
                this.enabled = false; // Disable the script if no player is found.
            }
        }
        aiState = AIState.passive;
    }

    void Update()
    {
        // If we don't have a valid player reference, do nothing.
        if (player == null) return;

        IsPlayerOnSamePlatform();

        switch (aiState)
        {
            case AIState.chase:
                // If they are on the same platform, chase the player.
                navMeshAgent.isStopped = false; // Ensure the agent can move.
                navMeshAgent.SetDestination(player.position);

                //Only growl if it's been enough time since the last one
                if (Time.time - growlStartTime > growlCooldown)
                {
                    //Debug.Log("Trigger Growl");
                    EventManager.TriggerEvent<TigerGrowlEvent, Vector3>(this.gameObject.transform.position);
                    growlStartTime = Time.time;
                    growlCooldown = Random.Range(3f, 6f);
                }
                break;

            case AIState.passive:
                // If they are on a different platform, stop the agent.
                navMeshAgent.isStopped = true;
                break;

        }
    }

    /// <summary>
    /// Checks if the AI and the player are standing on the same ground object using raycasts.
    /// </summary>
    private void IsPlayerOnSamePlatform()
    {
        // Raycast down from the AI to find its ground.
        RaycastHit aiHit;
        bool aiIsGrounded = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out aiHit, groundCheckDistance, groundLayer);

        // Raycast down from the Player to find its ground.
        RaycastHit playerHit;
        bool hit = Physics.Raycast(player.position, transform.TransformDirection(Vector3.down), out playerHit, Mathf.Infinity, groundLayer);
    
        //Debug.Log("Distance to hit: " + Vector3.Distance(player.position, playerHit.point));
        if (hit)
        {
            bool playerIsGrounded = playerHit.distance < groundCheckDistance;
            //Debug.DrawRay(player.position, transform.TransformDirection(Vector3.down) * playerHit.distance, Color.red);
            // They are on the same platform if both are grounded AND the colliders they are standing on are the same object.
            if (aiIsGrounded && playerIsGrounded && aiHit.collider.gameObject == playerHit.collider.gameObject)
            {
                // Set AI state to chase (if it is not already)
                if (aiState != AIState.chase)
                    aiState = AIState.chase;
                return;
            }

            //Debug.Log("Hit: " + playerHit.point);
            
            if (aiState == AIState.chase && aiHit.collider.gameObject != playerHit.collider.gameObject)
            {
                EventManager.TriggerEvent<TigerRoarEvent, Vector3>(this.gameObject.transform.position);
                //set the growl cooldown so it doesn't immediately growl if the player jumps back on the platform
                growlStartTime = Time.time;
                growlCooldown = Random.Range(3f, 6f);
            }
        }

        // If the player raycast doesn't hit the same frame that the aiState is chase, play roar
        // maybe this will cause problems, but it works for some unknown reason so I'm leaving it in for now
        else if (aiState == AIState.chase)
        {
            EventManager.TriggerEvent<TigerRoarEvent, Vector3>(this.gameObject.transform.position);
            growlStartTime = Time.time;
            growlCooldown = Random.Range(3f, 6f);
        }

        // If they're not on the same platform, set the AI state to passive
        if (aiState != AIState.passive)
            aiState = AIState.passive;
    }

}
