using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("General Smoothing")]
    public float positionSmoothTime = 1f;      // Smoothing for X and Z movement
    public float rotationSmoothTime = 1f;
    public float positionMaxSpeed = 50f;      // Max speed camera can move
    public float rotationMaxSpeed = 50f;

    [Header("Vertical (Jump) Smoothing")]
    public float yPositionSmoothTime = 0.1f;    // A much faster smoothing time for the Y-axis
    public float yPositionMaxSpeed = 100f;      // Max speed for vertical movement

    [Header("Targets")]
    public Transform desiredPose;             // The ideal spot for the camera
    public Transform target;                  // The player (or a point on the player)

    [Header("Collision")]
    public LayerMask collisionMask;           // Set this in the Inspector to what the camera should collide with
    public float clipOffset = 0.2f;           // How far to back away from a wall to avoid clipping

    protected Vector3 currentPositionCorrectionVelocity;
    protected float currentYVelocity; // Velocity for the Y-axis SmoothDamp
    protected Quaternion quaternionDeriv;


    void LateUpdate()
    {
        if (desiredPose == null || target == null) return;

        // --- 1. Calculate the Target Position (as if no walls) ---
        // We want to smoothly follow the desiredPose.

        // --- 1a. Horizontal (X, Z) Smoothing ---
        // We smooth towards the desiredPose's X/Z, but keep our current Y for now.
        Vector3 targetHorizontalPosition = new Vector3(desiredPose.position.x, transform.position.y, desiredPose.position.z);
        Vector3 smoothedHorizontalPosition = Vector3.SmoothDamp(
            transform.position,
            targetHorizontalPosition,
            ref currentPositionCorrectionVelocity,
            positionSmoothTime,
            positionMaxSpeed,
            Time.deltaTime
        );

        // --- 1b. Vertical (Y) Smoothing ---
        // We smooth towards the desiredPose's Y independently.
        float smoothedYPosition = Mathf.SmoothDamp(
            transform.position.y,
            desiredPose.position.y,
            ref currentYVelocity,
            yPositionSmoothTime,
            yPositionMaxSpeed,
            Time.deltaTime
        );

        // --- 2. Combine Results into our potential new position ---
        Vector3 potentialPosition = new Vector3(smoothedHorizontalPosition.x, smoothedYPosition, smoothedHorizontalPosition.z);

        // --- 3. Check for Collisions ---
        // Now, check if this 'potentialPosition' is blocked by a wall.
        // We cast a ray from the player (target) TO our new potential camera position.

        Vector3 fromTargetToPotential = potentialPosition - target.position;
        Vector3 direction = fromTargetToPotential.normalized;
        float distance = fromTargetToPotential.magnitude;

        Vector3 finalPosition;
        RaycastHit hit;

        // Cast a ray from the player towards the potential camera spot
        if (Physics.Raycast(target.position, direction, out hit, distance, collisionMask))
        {
            // We hit a wall. Snap the camera to the hit point, backed up by the offset.
            // This is an INSTANT correction, not smoothed.
            finalPosition = hit.point - direction * clipOffset;
        }
        else
        {
            // No collision. The smoothed position is fine.
            finalPosition = potentialPosition;
        }

        // --- 4. Apply Final Position & Rotation ---
        transform.position = finalPosition;

        // --- Rotation (Unchanged) ---
        var targForward = desiredPose.forward;
        transform.rotation = QuaternionUtil.SmoothDamp(transform.rotation,
            Quaternion.LookRotation(targForward, Vector3.up), ref quaternionDeriv, rotationSmoothTime);
    }
}

// NOTE: This script still requires the "QuaternionUtil" script to be in your project.