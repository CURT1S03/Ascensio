using UnityEngine;
using System.Collections;

public class BouncyLeaf : MonoBehaviour
{
    [Header("Bounce Physics")]
    [Tooltip("The minimum force applied if the player just walks onto it")]
    public float minBounceForce = 10f;

    [Tooltip("The maximum force applied. Set this high enough to accommodate the Super Jump!")]
    public float maxBounceForce = 40f; // Increased default slightly to allow for boosts

    [Tooltip("Multiplies the player's falling speed.")]
    public float bounceMultiplier = 1.2f;

    [Header("Jump Boost")]
    [Tooltip("Multiplier applied if the player holds Space while landing")]
    public float jumpBoostMultiplier = 1.5f;

    [Header("Visual Feedback")]
    public float squashAmount = 0.6f; // How flat the leaf gets (0 to 1)
    public float animationDuration = 0.2f;

    private Vector3 originalScale;
    private bool isAnimating = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 1. Check if it is the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // 2. Check if the player hit the TOP of the leaf
            ContactPoint contact = collision.GetContact(0);
            if (Vector3.Dot(contact.normal, Vector3.down) > 0.5f)
            {
                Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

                if (playerRb != null)
                {
                    ApplyBounce(playerRb, collision.relativeVelocity);
                    if (!isAnimating) StartCoroutine(SquashAnimation());
                }
            }
        }
    }

    private void ApplyBounce(Rigidbody playerRb, Vector3 impactVelocity)
    {
        // TRAMPOLINE MATH:
        // collision.relativeVelocity gives us how hard the two objects hit.
        float impactSpeed = Mathf.Abs(impactVelocity.y);

        // Calculate base force
        float calculatedForce = impactSpeed * bounceMultiplier;

        // BOOST LOGIC:
        // Check if Space is held down at the moment of impact
        if (Input.GetKey(KeyCode.Space))
        {
            calculatedForce *= jumpBoostMultiplier;
        }

        // Clamp the force so it respects the safety limits
        float newUpwardForce = Mathf.Clamp(calculatedForce, minBounceForce, maxBounceForce);

        // Reset the player's current Y velocity to 0 for consistent AddForce
        Vector3 currentVelocity = playerRb.linearVelocity;
        playerRb.linearVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);

        // Apply the force
        playerRb.AddForce(Vector3.up * newUpwardForce, ForceMode.Impulse);
    }

    // Squash and Stretch Animation
    IEnumerator SquashAnimation()
    {
        isAnimating = true;
        float timer = 0;

        // Squash down
        while (timer < animationDuration / 2)
        {
            timer += Time.deltaTime;
            float scaleY = Mathf.Lerp(originalScale.y, originalScale.y * squashAmount, timer / (animationDuration / 2));
            float scaleXZ = Mathf.Lerp(originalScale.x, originalScale.x * (1 + (1 - squashAmount)), timer / (animationDuration / 2));

            transform.localScale = new Vector3(scaleXZ, scaleY, scaleXZ);
            yield return null;
        }

        // Spring back up
        timer = 0;
        while (timer < animationDuration / 2)
        {
            timer += Time.deltaTime;
            float scaleY = Mathf.Lerp(originalScale.y * squashAmount, originalScale.y, timer / (animationDuration / 2));
            float scaleXZ = Mathf.Lerp(originalScale.x * (1 + (1 - squashAmount)), originalScale.x, timer / (animationDuration / 2));

            transform.localScale = new Vector3(scaleXZ, scaleY, scaleXZ);
            yield return null;
        }

        transform.localScale = originalScale;
        isAnimating = false;
    }
}