using UnityEngine;

public class SafetyNet : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode cheatKey = KeyCode.R;
    public float saveInterval = 0.5f;

    public string[] safeTags = { "Ground", "Wood" };

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

    // METHOD B: Instant Touch (For Bouncy Leaves/Wood)
    private void OnCollisionEnter(Collision collision)
    {
        if (IsSafeTag(collision.gameObject))
        {
            ContactPoint contact = collision.GetContact(0);
            // Check if we landed ON TOP
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
        // CHANGED: Checks if the object matches ANY of our safe tags
        if (IsSafeTag(hit.gameObject))
        {
            if (hit.normal.y > 0.5f)
            {
                if (Vector3.Distance(transform.position, lastSafePosition) > 0.2f)
                {
                    SavePosition();
                }
            }
        }
    }

    void SavePosition()
    {
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
        if (Physics.SphereCast(transform.position + Vector3.up * 0.5f, 0.3f, Vector3.down, out hit, 2.0f))
        {
            if (IsSafeTag(hit.collider.gameObject)) return true;
        }
        return false;
    }

    bool IsSafeTag(GameObject obj)
    {
        foreach (string tag in safeTags)
        {
            if (obj.CompareTag(tag)) return true;
        }
        return false;
    }
}