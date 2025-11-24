using UnityEngine;

public class CloudElevator : MonoBehaviour
{
    [Header("Movement Settings")]
    public float heightDistance = 5f;
    public float speed = 2f;
    public float timeOffset = 0f;

    private Vector3 startPos;
    private Vector3 lastFramePos;
    private Rigidbody playerRb; // Track the player's Rigidbody

    void Start()
    {
        startPos = transform.position;
        lastFramePos = transform.position;
    }

    void FixedUpdate()
    {
        // 1. Calculate where the cloud should be
        float newY = startPos.y + Mathf.PingPong((Time.time + timeOffset) * speed, heightDistance);
        Vector3 targetPos = new Vector3(startPos.x, newY, startPos.z);

        // 2. Move the Cloud
        Rigidbody cloudRb = GetComponent<Rigidbody>();
        if (cloudRb)
        {
            cloudRb.MovePosition(targetPos);
        }
        else
        {
            transform.position = targetPos;
        }

        // 3. Calculate how much we moved since last frame
        Vector3 platformMovement = targetPos - lastFramePos;

        // 4. If player is on board, move them by that SAME amount
        if (playerRb != null)
        {
            // We use MovePosition to respect collision physics
            playerRb.MovePosition(playerRb.position + platformMovement);
        }

        lastFramePos = targetPos;
    }

    // When Player steps on
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = collision.gameObject.GetComponent<Rigidbody>();
        }
    }

    // When Player steps off
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = null;
        }
    }
}