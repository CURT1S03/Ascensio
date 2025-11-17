using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    private Animator animator;

    // We "hash" the parameter name for better performance
    private int isPlayerOnHash;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("ElevatorTrigger: No Animator component found on this object!");
        }

        // Convert the string "isPlayerOn" to an ID
        isPlayerOnHash = Animator.StringToHash("isPlayerOn");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Set the boolean to TRUE
            animator.SetBool(isPlayerOnHash, true);

            // Make the player a child of the elevator
            other.transform.parent = this.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Set the boolean to FALSE
            animator.SetBool(isPlayerOnHash, false);

            // Un-parent the player
            other.transform.parent = null;
        }
    }
}