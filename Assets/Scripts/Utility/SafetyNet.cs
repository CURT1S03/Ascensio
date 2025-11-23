using UnityEngine;

public class SafetyNet : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode cheatKey = KeyCode.R;
    public float saveInterval = 0.5f;
    public string safeTag = "Ground";

    private Vector3 lastSafePosition;
    private CharacterController characterController;
    private Rigidbody rb;
    private float timer;
    private bool hasSavedPosition = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        lastSafePosition = transform.position;
        hasSavedPosition = true;
    }

    void Update()
    {
        // METHOD A: Standing Still (For normal floors)
        if (IsOnSafeSurface())
        {
            timer += Time.deltaTime;
            Debug.DrawRay(transform.position, Vector3.down * 2f, Color.green);

            if (timer >= saveInterval)
            {
                SavePosition();
                timer = 0f;
            }
        }
        else
        {
            Debug.DrawRay(transform.position, Vector3.down * 2f, Color.red);
            timer = 0f;
        }

        // Cheat Input
        if (Input.GetKeyDown(cheatKey) && hasSavedPosition)
        {
            TeleportToSafeSpot();
        }
    }

    // METHOD B: Instant Touch (For Bouncy Leaves)
    // This detects the collision the MOMENT it happens, bypassing the timer.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(safeTag))
        {
            // Check if we landed ON TOP of it (not hitting the side)
            ContactPoint contact = collision.GetContact(0);
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                Debug.Log("Instant Save: Touched " + collision.gameObject.name);
                SavePosition();
            }
        }
    }

    // Compatible with CharacterController collisions too
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag(safeTag))
        {
            // Check if the surface is relatively flat (floor)
            if (hit.normal.y > 0.5f)
            {
                // We force a save, but maybe limit it slightly to prevent spam
                if (Vector3.Distance(transform.position, lastSafePosition) > 0.2f)
                {
                    SavePosition();
                }
            }
        }
    }

    void SavePosition()
    {
        // Check distance to avoid spamming variables
        if (Vector3.Distance(transform.position, lastSafePosition) > 0.1f)
        {
            lastSafePosition = transform.position;
            hasSavedPosition = true;
        }
    }

    void TeleportToSafeSpot()
    {
        if (characterController != null) characterController.enabled = false;
        transform.position = lastSafePosition;
        if (characterController != null) characterController.enabled = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        Debug.Log("Returned to last safe point!");
    }

    bool IsOnSafeSurface()
    {
        RaycastHit hit;
        // SphereCast for reliable ground detection
        if (Physics.SphereCast(transform.position + Vector3.up * 0.5f, 0.3f, Vector3.down, out hit, 2.0f))
        {
            if (hit.collider.CompareTag(safeTag)) return true;
        }
        return false;
    }
}