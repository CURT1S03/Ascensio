using UnityEngine;
using System.Collections;

public class BouncyLeaf : MonoBehaviour
{
    [Header("Lazy Bounce (No Input)")]
    [Tooltip("If player just walks on, use this force.")]
    public float minBounceForce = 15f;

    [Tooltip("Multiplies falling speed for normal bounces.")]
    public float bounceMultiplier = 1.2f;

    [Tooltip("The maximum force allowed for a normal walk-on bounce.")]
    public float normalMaxForce = 30f; // Renamed to ensure it resets to 30

    [Header("Super Bounce (Space Bar)")]
    [Tooltip("If Space is pressed, ignore physics and use EXACTLY this force.")]
    public float superJumpForce = 45f; // Renamed to ensure it resets to 45

    [Tooltip("How long (seconds) to hold player on leaf before launching.")]
    public float bounceDelay = 0.2f;

    [Header("Global Safety")]
    [Tooltip("The absolute maximum force allowed (Caps both Normal and Super jumps).")]
    public float absoluteMaxForce = 45f; // Renamed to ensure it resets to 45

    [Header("Visuals")]
    public float squashAmount = 0.4f;
    public Color boostColor = Color.white;

    private Vector3 originalScale;
    private Color originalColor;
    private Renderer ren;
    private bool isBouncing = false;
    private bool boostQueued = false;

    void Start()
    {
        originalScale = transform.localScale;
        ren = GetComponentInChildren<Renderer>();
        if (ren) originalColor = ren.material.color;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(BufferInput());
        }
    }

    IEnumerator BufferInput()
    {
        boostQueued = true;
        yield return new WaitForSeconds(0.5f);
        if (!isBouncing) boostQueued = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isBouncing) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            ContactPoint contact = collision.GetContact(0);
            if (Vector3.Dot(contact.normal, Vector3.down) > 0.5f)
            {
                Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    StartCoroutine(BounceSequence(playerRb, collision.relativeVelocity));
                }
            }
        }
    }

    IEnumerator BounceSequence(Rigidbody playerRb, Vector3 impactVelocity)
    {
        isBouncing = true;
        float timer = 0f;

        if (!Input.GetKey(KeyCode.Space) && !boostQueued)
        {
            boostQueued = false;
        }

        // --- PHASE 1: SINK ---
        while (timer < bounceDelay)
        {
            timer += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Space))
            {
                boostQueued = true;
                if (ren) ren.material.color = boostColor;
            }

            float t = timer / bounceDelay;
            float scaleY = Mathf.Lerp(originalScale.y, originalScale.y * squashAmount, t);
            float scaleXZ = Mathf.Lerp(originalScale.x, originalScale.x * (1 + (1 - squashAmount)), t);
            transform.localScale = new Vector3(scaleXZ, scaleY, scaleXZ);

            if (playerRb != null)
            {
                playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);
            }

            yield return null;
        }

        // --- PHASE 2: LAUNCH ---
        if (playerRb != null)
        {
            ApplyBounce(playerRb, impactVelocity);
        }

        // --- PHASE 3: RECOIL ---
        if (ren) ren.material.color = originalColor;

        timer = 0f;
        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            float t = timer / 0.2f;
            float scaleY = Mathf.Lerp(originalScale.y * squashAmount, originalScale.y, t);
            float scaleXZ = Mathf.Lerp(originalScale.x * (1 + (1 - squashAmount)), originalScale.x, t);
            transform.localScale = new Vector3(scaleXZ, scaleY, scaleXZ);
            yield return null;
        }

        transform.localScale = originalScale;
        isBouncing = false;
        boostQueued = false;
    }

    private void ApplyBounce(Rigidbody playerRb, Vector3 impactVelocity)
    {
        float finalForce = 0f;

        if (boostQueued)
        {
            // SUPER JUMP (Uses 45)
            finalForce = superJumpForce;
            Debug.Log("SUPER JUMP! Raw Force: " + finalForce);
        }
        else
        {
            // LAZY JUMP (Uses Physics, capped at 30)
            float impactSpeed = Mathf.Abs(impactVelocity.y);
            finalForce = impactSpeed * bounceMultiplier;
            finalForce = Mathf.Max(finalForce, minBounceForce);

            // Apply Normal Cap
            if (finalForce > normalMaxForce)
            {
                finalForce = normalMaxForce;
                Debug.Log("Normal Bounce capped at 30.");
            }
            else
            {
                Debug.Log("Normal Bounce. Force: " + finalForce);
            }
        }

        // GLOBAL SAFETY CLAMP
        if (finalForce > absoluteMaxForce)
        {
            finalForce = absoluteMaxForce;
            Debug.LogWarning("Force clamped by Absolute Safety Limit!");
        }

        Vector3 v = playerRb.linearVelocity;
        playerRb.linearVelocity = new Vector3(v.x, 0, v.z);
        playerRb.AddForce(Vector3.up * finalForce, ForceMode.Impulse);
    }
}