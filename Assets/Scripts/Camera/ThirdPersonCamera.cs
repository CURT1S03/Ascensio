using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("General Smoothing")]
    public float positionSmoothTime = 1f;       // Smoothing for X and Z movement
    public float rotationSmoothTime = 1f;
    public float positionMaxSpeed = 50f;        // Max speed camera can move
    public float rotationMaxSpeed = 50f;

    [Header("Vertical (Jump) Smoothing")]
    public float yPositionSmoothTime = 0.1f;    // A much faster smoothing time for the Y-axis
    public float yPositionMaxSpeed = 100f;      // Max speed for vertical movement

    [Header("Targets")]
    public Transform desiredPose;               // The ideal spot for the camera
    public Transform target;                    // The player (or a point on the player)

    [Header("Collision")]
    public LayerMask collisionMask;             // Set this in the Inspector to what the camera should collide with
    public float clipOffset = 0.2f;             // How far to back away from a wall to avoid clipping

    protected Vector3 currentPositionCorrectionVelocity;
    protected float currentYVelocity; // Velocity for the Y-axis SmoothDamp
    protected Quaternion quaternionDeriv;


    void LateUpdate()
    {
        if (desiredPose == null || target == null) return;

        // --- 1. Calculate Collision-Aware Target Position ---

        // Get the direction and distance from the player to the camera's ideal spot
        Vector3 fromTargetToPose = desiredPose.position - target.position;
        Vector3 direction = fromTargetToPose.normalized;
        float desiredDistance = fromTargetToPose.magnitude;

        Vector3 collisionAwareTargetPosition;
        RaycastHit hit;

        // Cast a ray from the player (target) towards the camera's ideal spot (desiredPose)
        if (Physics.Raycast(target.position, direction, out hit, desiredDistance, collisionMask))
        {
            // We hit something. Move the target position to the hit point.
            // Back it up by 'clipOffset' so the camera doesn't sit *inside* the wall.
            collisionAwareTargetPosition = hit.point - direction * clipOffset;
        }
        else
        {
            // No collision, the ideal spot is fine.
            collisionAwareTargetPosition = desiredPose.position;
        }

        // --- 2. Apply Independent X/Z and Y Smoothing ---
        // We now use our new 'collisionAwareTargetPosition' as the goal.

        // --- 2a. Horizontal (X, Z) Smoothing ---
        Vector3 targetHorizontalPosition = new Vector3(collisionAwareTargetPosition.x, transform.position.y, collisionAwareTargetPosition.z);
        Vector3 smoothedHorizontalPosition = Vector3.SmoothDamp(
            transform.position,
            targetHorizontalPosition,
            ref currentPositionCorrectionVelocity,
            positionSmoothTime,
            positionMaxSpeed,
            Time.deltaTime
        );

        // --- 2b. Vertical (Y) Smoothing ---
        float smoothedYPosition = Mathf.SmoothDamp(
            transform.position.y,
            collisionAwareTargetPosition.y,
            ref currentYVelocity,
            yPositionSmoothTime,
            yPositionMaxSpeed,
            Time.deltaTime
        );

        // --- 3. Combine Results ---
        transform.position = new Vector3(smoothedHorizontalPosition.x, smoothedYPosition, smoothedHorizontalPosition.z);

        // --- 4. Rotation (Unchanged) ---
        var targForward = desiredPose.forward;
        transform.rotation = QuaternionUtil.SmoothDamp(transform.rotation,
            Quaternion.LookRotation(targForward, Vector3.up), ref quaternionDeriv, rotationSmoothTime);
    }
}