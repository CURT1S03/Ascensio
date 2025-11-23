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
    [Tooltip("A reference to the player GameObject.")]
    public GameObject player;

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

    // --- Animation (added) ---
    [Header("Animation")]
    [SerializeField] private Animator anim;
    [SerializeField] private float damp = 0.1f;
    [SerializeField] private float runThreshold = 3.0f;
    
    // --- Private Variables ---
    private NavMeshAgent navMeshAgent;
    private float growlCooldown = 6;
    private float growlStartTime = 0;
    private CharacterCommon groundChecker;
    private BasicControlScript playerGroundReporter;


    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        // For robustness, if the player isn't assigned in the Inspector, try to find them by tag.
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (!playerObject)
            {
                Debug.LogError("EnemyMovement: Cannot find GameObject with 'Player' tag and player transform is not assigned. AI will be inactive.");
                this.enabled = false; // Disable the script if no player is found.
            }
        }

        if(player != null)
        {
            groundChecker = GetComponent<CharacterCommon>();
            if (groundChecker == null) Debug.Log("CharacterCommon could not be found");
            playerGroundReporter = player.GetComponent<BasicControlScript>();
            if(playerGroundReporter == null) Debug.Log("Unable to access player control");
        }

        // --- Animation hookup (added) ---
        if (anim == null)
            anim = GetComponentInChildren<Animator>(true);
        if (anim != null)
            anim.applyRootMotion = false; // AI (NavMeshAgent) controls movement, not animation        

        aiState = AIState.passive;
    }

    void Update()
    {
        // If we don't have a valid player reference, do nothing.
        if (player == null || playerGroundReporter == null) return;
        
        IsPlayerOnSamePlatform();

        switch (aiState)
        {
            case AIState.chase:
                // If they are on the same platform, chase the player.
                navMeshAgent.isStopped = false; // Ensure the agent can move.
                navMeshAgent.SetDestination(player.transform.position);

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

        // --- Drive Animator every frame (added) ---
        UpdateAnimatorParameters();
    }

    /// <summary>
    /// Checks if the AI and the player are standing on the same ground object using raycasts.
    /// </summary>
    private void IsPlayerOnSamePlatform()
    {
        // Raycast down from the AI to find its ground.
        // The AI can't leave their platforms so we could replace this with a public GameObject to save resources
        groundChecker.CheckGroundNear(transform.position, 45, 5f, 1f);
        bool aiIsGrounded = groundChecker.gh.ClosestGround != null;
        
        if (playerGroundReporter.playerGround.ClosestGround != null && aiIsGrounded && playerGroundReporter.playerGround.DistanceToGround < groundCheckDistance)
        {
            // They are on the same platform if both are grounded AND the colliders they are standing on are the same object.
            if (groundChecker.gh.ClosestGround == playerGroundReporter.playerGround.ClosestGround)
            {
                // Set AI state to chase (if it is not already)
                if (aiState != AIState.chase)
                    aiState = AIState.chase;
                return;
            }
            
            if (aiState == AIState.chase && groundChecker.gh.ClosestGround != playerGroundReporter.playerGround.ClosestGround)
            {
                EventManager.TriggerEvent<TigerRoarEvent, Vector3>(this.gameObject.transform.position);
                //set the growl cooldown so it doesn't immediately growl if the player jumps back on the platform
                growlStartTime = Time.time;
                growlCooldown = Random.Range(3f, 6f);
            }
        }

        // Catches all other cases of the player leaving the platform the AI is on
        else if (aiState == AIState.chase && (!playerGroundReporter.playerGround.ClosestGround || aiIsGrounded && groundChecker.gh.ClosestGround != playerGroundReporter.playerGround.ClosestGround || !aiIsGrounded))
        {
            EventManager.TriggerEvent<TigerRoarEvent, Vector3>(this.gameObject.transform.position);
            growlStartTime = Time.time;
            growlCooldown = Random.Range(3f, 6f);
        }

        // If they're not on the same platform, set the AI state to passive
        if (aiState != AIState.passive)
            aiState = AIState.passive;
    }

    // --- Animator driver for Vert/State ---
    private void UpdateAnimatorParameters()
    {
        if (anim == null) return;

        // World-space linear speed (m/s) from NavMeshAgent
        float speed = navMeshAgent != null ? navMeshAgent.velocity.magnitude : 0f;

        bool isMoving  = speed > 0.05f;
        bool isRunning = speed >= runThreshold;

        // Vert: 0 idle, 1 moving; State: 0 walk, 1 run
        float targetVert  = isMoving ? 1f : 0f;
        float targetState = isRunning ? 1f : 0f;

        anim.SetFloat("Vert",  targetVert,  damp, Time.deltaTime);
        anim.SetFloat("State", targetState, damp, Time.deltaTime);

        // Ensure root motion stays off (AI controls translation)
        if (anim.applyRootMotion) anim.applyRootMotion = false;
    }
}