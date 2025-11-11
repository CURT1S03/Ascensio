using UnityEngine;

/// <summary>
/// This script creates a moving platform (an elevator).
/// 
/// --- REQUIRED SETUP ---
/// 1. Place this script on the platform object you want to move.
/// 
/// 2. On this same platform object, add two colliders:
///    - Collider 1 (Physical): A MeshCollider or BoxCollider.
///      This is what the player stands on.
///      *** "Is Trigger" must be UNCHECKED. ***
/// 
///    - Collider 2 (Activation): A BoxCollider.
///      This will detect the player.
///      *** "Is Trigger" must be CHECKED. ***
///      Adjust this trigger's 'Center' and 'Size' to cover the top
///      surface of your platform.
/// 
/// 3. Create an empty GameObject.
///    - Name it "Leaf_Up_Position" (or similar).
///    - Move it to the exact spot where you want the elevator to stop.
/// 
/// 4. Select your ElevatorLeaf object.
///    - Drag the "Leaf_Up_Position" object into the "Target Position Up" slot.
/// 
/// 5. Your Player object must have the tag "Player" and a Rigidbody.
/// </summary>
public class ElevatorLeaf : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The empty GameObject that marks the 'up' position.")]
    public Transform targetPositionUp;

    [Tooltip("How fast the platform moves.")]
    public float speed = 3.0f;

    [Header("Setup")]
    [Tooltip("The tag of the player object.")]
    public string playerTag = "Player";

    // --- Private State Variables ---
    private Vector3 startPosition;
    private Vector3 destinationPosition;
    private bool isUp = false;
    private bool isMoving = false;

    void Start()
    {
        // Remember our starting (down) position
        startPosition = transform.position;
        // Set the initial destination (it's not moving yet)
        destinationPosition = startPosition;
    }

    void Update()
    {
        // We only care about moving if 'isMoving' is true
        if (isMoving)
        {
            // Move towards the destination
            transform.position = Vector3.MoveTowards(transform.position, destinationPosition, speed * Time.deltaTime);

            // Check if we've arrived (or are very close)
            if (Vector3.Distance(transform.position, destinationPosition) < 0.01f)
            {
                // Snap to the final position to be precise
                transform.position = destinationPosition;
                // Stop moving
                isMoving = false;
            }
        }
    }

    /// <summary>
    /// This function is called by OnTriggerEnter.
    /// It contains the logic to start the elevator.
    /// </summary>
    private void ActivateElevator()
    {
        // Only activate if we are NOT already moving
        if (!isMoving)
        {
            // We are at the top, so let's go down
            if (isUp)
            {
                destinationPosition = startPosition;
                isUp = false;
            }
            // We are at the bottom, so let's go up
            else
            {
                destinationPosition = targetPositionUp.position;
                isUp = true;
            }

            // Start moving!
            isMoving = true;
        }
    }

    /// <summary>
    /// This is called when the player enters the trigger zone.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // --- Parent the player ---
            // This makes the player "stick" to the leaf and move with it.
            other.transform.SetParent(transform);

            // --- Start the elevator ---
            // We call this function to handle the logic.
            ActivateElevator();
        }
    }

    /// <summary>
    /// This is called when the player leaves the trigger zone (jumps off).
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // --- Un-parent the player ---
            // This gives control back to the player.
            other.transform.SetParent(null);
        }
    }
}