using UnityEngine;
using System.Collections;

public class BouncyLeaf : MonoBehaviour
{
    [Header("Bounce Physics")]
    [Tooltip("The minimum force applied if the player just walks onto it")]
    public float minBounceForce = 10f;

    [Tooltip("The maximum force applied.")]
    public float maxBounceForce = 40f;

    [Tooltip("Multiplies the player's falling speed.")]
    public float bounceMultiplier = 1.2f;

    [Header("Jump Boost Timing")]
    [Tooltip("Multiplier applied if the player times their jump correctly")]
    public float jumpBoostMultiplier = 1.5f;

    [Tooltip("Time window to accept inputs BEFORE hitting the leaf")]
    public float jumpBufferWindow = 0.5f;

    [Header("Visual Feedback")]
    public float squashAmount = 0.6f;
    public float animationDuration = 0.2f;

    private Vector3 originalScale;
    private bool isAnimating = false;
    private float lastJumpPressTime = -100f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Constantly listen for the Jump key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lastJumpPressTime = Time.time;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ContactPoint contact = collision.GetContact(0);

            // Ensure hitting from top (Checking if normal points UP)
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
        float impactSpeed = Mathf.Abs(impactVelocity.y);

        // 1. Calculate Base Force
        float baseForce = impactSpeed * bounceMultiplier;

        // 2. Enforce Minimum Force IMMEDIATELY
        // This ensures that even a small hop gets raised to the minimum standard
        // BEFORE we apply the boost multiplier.
        baseForce = Mathf.Max(baseForce, minBounceForce);

        // --- TIMING LOGIC ---
        bool isHoldingSpace = Input.GetKey(KeyCode.Space);
        bool pressedSpaceRecently = (Time.time - lastJumpPressTime) <= jumpBufferWindow;

        float finalForce = baseForce;

        if (isHoldingSpace || pressedSpaceRecently)
        {
            // 3. Apply Boost to the valid, clamped base force
            finalForce *= jumpBoostMultiplier;

            // Reset buffer
            lastJumpPressTime = -100f;
        }

        // 4. Final Safety Clamp (prevent flying into orbit)
        finalForce = Mathf.Clamp(finalForce, minBounceForce, maxBounceForce);

        // Reset vertical velocity for consistent bounce
        Vector3 currentVelocity = playerRb.linearVelocity;
        playerRb.linearVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);

        playerRb.AddForce(Vector3.up * finalForce, ForceMode.Impulse);
    }

    IEnumerator SquashAnimation()
    {
        isAnimating = true;
        float timer = 0;

        // Squash
        while (timer < animationDuration / 2)
        {
            timer += Time.deltaTime;
            float t = timer / (animationDuration / 2);
            float scaleY = Mathf.Lerp(originalScale.y, originalScale.y * squashAmount, t);
            float scaleXZ = Mathf.Lerp(originalScale.x, originalScale.x * (1 + (1 - squashAmount)), t);
            transform.localScale = new Vector3(scaleXZ, scaleY, scaleXZ);
            yield return null;
        }

        // Stretch back
        timer = 0;
        while (timer < animationDuration / 2)
        {
            timer += Time.deltaTime;
            float t = timer / (animationDuration / 2);
            float scaleY = Mathf.Lerp(originalScale.y * squashAmount, originalScale.y, t);
            float scaleXZ = Mathf.Lerp(originalScale.x * (1 + (1 - squashAmount)), originalScale.x, t);
            transform.localScale = new Vector3(scaleXZ, scaleY, scaleXZ);
            yield return null;
        }

        transform.localScale = originalScale;
        isAnimating = false;
    }
}