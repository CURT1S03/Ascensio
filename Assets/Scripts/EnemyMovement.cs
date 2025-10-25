using UnityEngine;
using UnityEngine.AI;

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

    // --- Private Variables ---
    private NavMeshAgent navMeshAgent;

// remove me

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
    }

    void Update()
    {
        // If we don't have a valid player reference, do nothing.
        if (player == null) return;

        // Check if the player is on the same platform as the AI.
        if (IsPlayerOnSamePlatform())
        {
            // If they are on the same platform, chase the player.
            navMeshAgent.isStopped = false; // Ensure the agent can move.
            navMeshAgent.SetDestination(player.position);
        }
        else
        {
            // If they are on a different platform, stop the agent.
            navMeshAgent.isStopped = true;
        }
    }

    /// <summary>
    /// Checks if the AI and the player are standing on the same ground object using raycasts.
    /// </summary>
    /// <returns>True if they are on the same platform, false otherwise.</returns>
    private bool IsPlayerOnSamePlatform()
    {
        // Raycast down from the AI to find its ground.
        RaycastHit aiHit;
        bool aiIsGrounded = Physics.Raycast(transform.position, Vector3.down, out aiHit, groundCheckDistance, groundLayer);

        // Raycast down from the Player to find its ground.
        RaycastHit playerHit;
        bool playerIsGrounded = Physics.Raycast(player.position, Vector3.down, out playerHit, groundCheckDistance, groundLayer);

        // They are on the same platform if both are grounded AND the colliders they are standing on are the same object.
        if (aiIsGrounded && playerIsGrounded && aiHit.collider.gameObject == playerHit.collider.gameObject)
        {
            return true;
        }

        return false;
    }
}
