using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    // Adjust this to make the box feel heavier or lighter
    public float pushPower = 2.0f;

    // This function is automatically called when the CharacterController touches a collider
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic)
        {
            return;
        }

        // 3. Prevent pushing the box down when standing on top of it
        // If the hit direction is mostly down (Y is negative), don't push.
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        // 4. Calculate push direction
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // 5. Apply the velocity to the box
        body.linearVelocity = pushDir * pushPower;
    }
}