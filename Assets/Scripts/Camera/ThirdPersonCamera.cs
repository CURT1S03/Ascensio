using UnityEngine;
using System.Collections;

// ====================================================================
// This is your main MonoBehaviour class.
// It MUST be in a file named "ThirdPersonCamera.cs"
// ====================================================================
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("General Smoothing")]
    public float positionSmoothTime = 1f;     // Smoothing for X and Z movement
    public float rotationSmoothTime = 1f;
    public float positionMaxSpeed = 50f;      // Max speed camera can move
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

    // --- THIS LINE WAS CHANGED ---
    // Was: protected Quaternion quaternionDeriv;
    // Now: protected Vector4 quaternionDeriv; (to store 4 float velocities)
    protected Vector4 quaternionDeriv;


    void LateUpdate()
    {
        if (desiredPose == null || target == null) return;

        // --- 1. Calculate the Target Position (as if no walls) ---

        // --- 1a. Horizontal (X, Z) Smoothing ---
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
        Vector3 fromTargetToPotential = potentialPosition - target.position;
        Vector3 direction = fromTargetToPotential.normalized;
        float distance = fromTargetToPotential.magnitude;

        Vector3 finalPosition;
        RaycastHit hit;

        // Cast a ray from the player towards the potential camera spot
        if (Physics.Raycast(target.position, direction, out hit, distance, collisionMask))
        {
            // We hit a wall. Snap the camera to the hit point, backed up by the offset.
            finalPosition = hit.point - direction * clipOffset;
        }
        else
        {
            // No collision. The smoothed position is fine.
            finalPosition = potentialPosition;
        }

        // --- 4. Apply Final Position & Rotation ---
        transform.position = finalPosition;

        // --- Rotation ---
        var targForward = desiredPose.forward;

        // This call now works because 'quaternionDeriv' is a Vector4
        // and 'QuaternionUtil' is in the same file.
        transform.rotation = QuaternionUtil.SmoothDamp(transform.rotation,
            Quaternion.LookRotation(targForward, Vector3.up), ref quaternionDeriv, rotationSmoothTime);
    }
}


// ====================================================================
// This is the utility class.
// It's in the same file, so it doesn't need to be "public".
// ====================================================================
internal static class QuaternionUtil
{
    //This is a slerp that's "framerate independent". 
    //Handles "smoothing" a rotation towards a target.
    //It uses a Vector4 to store the 4 velocities for x,y,z,w
    public static Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Vector4 deriv, float time)
    {
        if (Time.deltaTime < Mathf.Epsilon) return rot;

        // account for double-cover
        var Dot = Quaternion.Dot(rot, target);
        var Multi = Dot > 0f ? 1f : -1f;
        target.x *= Multi;
        target.y *= Multi;
        target.z *= Multi;
        target.w *= Multi;

        // smooth damp (nspring-like)
        var Result = new Vector4(
            Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
            Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
            Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
            Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
        ).normalized;

        // ensure deriv is tangent
        var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), Result);
        deriv.x -= derivError.x;
        deriv.y -= derivError.y;
        deriv.z -= derivError.z;
        deriv.w -= derivError.w;

        return new Quaternion(Result.x, Result.y, Result.z, Result.w);
    }
}