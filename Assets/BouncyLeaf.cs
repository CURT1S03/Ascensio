using UnityEngine;

public class BouncyLeaf : MonoBehaviour
{
    [Header("Bounce Settings")]
    [Tooltip("The strength of the upward bounce. This is applied as an impulse.")]
    public float bounceForce = 20f;

    [Tooltip("The tag of the object that can be bounced (e.g., 'Player').")]
    public string targetTag = "Player";

    [Header("Animation Settings")]
    [Tooltip("The name of the trigger parameter in the Animator to activate.")]
    public string animationTriggerName = "Bounce";


    private Animator leafAnimator;

    private void Start()
    {

        leafAnimator = GetComponentInParent<Animator>();

        if (leafAnimator == null)
        {
            Debug.LogWarning("BouncyLeaf: Could not find an Animator component on the parent object. Animation will not play.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.name);

        // Check if the object that entered the trigger has the correct tag
        if (other.CompareTag(targetTag))
        {
            Debug.Log(other.name + " has the correct tag!");

            // Get the Rigidbody attached to the collider that entered.
            Rigidbody rb = other.attachedRigidbody;

            // If it has a Rigidbody, apply the bounce
            if (rb != null)
            {
                Debug.Log("Found Rigidbody on " + other.name + ". Applying bounce!");

                // --- The Bounce Logic ---
                Vector3 vel = rb.linearVelocity;
                vel.y = 0f;
                rb.linearVelocity = vel;

                rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);

                // If we found an Animator, tell it to fire the trigger
                if (leafAnimator != null)
                {
                    leafAnimator.SetTrigger(animationTriggerName);
                }
            }
            else
            {
                Debug.LogWarning("Object " + other.name + " has tag '" + targetTag + "' but NO Rigidbody component was found!");
            }
        }
        else
        {
            Debug.Log("Tag mismatch. Object tag is '" + other.tag + "', expected '" + targetTag + "'");
        }
    }
}

