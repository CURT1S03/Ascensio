using UnityEngine;

public class JumpingBeanController : MonoBehaviour
{
    public float minJumpInterval = 1.0f;
    public float maxJumpInterval = 3.0f;
    public float minJumpForce = 5.0f;
    public float maxJumpForce = 10.0f;
    public float minTorque = 1.0f;
    public float maxTorque = 5.0f;

    private Rigidbody rbody;
    private int groundContactCount = 0;
    private float timeSinceLastJump = 0f;
    private float nextJumpTime = 0f;

    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        nextJumpTime = Random.Range(minJumpInterval, maxJumpInterval);
    }
    
    void Update()
    {
        if (groundContactCount > 0)
        {
            timeSinceLastJump += Time.fixedDeltaTime;

            if (timeSinceLastJump >= nextJumpTime)
            {
                Jump();
            }
        }
    }

    private void Jump()
    {
        Vector3 jumpDirection = new Vector3(
            Random.Range(-0.7f, 0.7f),
            Random.Range(0.5f, 1.0f),
            Random.Range(-0.7f, 0.7f)
        ).normalized;

        float jumpMagnitude = Random.Range(minJumpForce, maxJumpForce);
        rbody.AddForce(jumpDirection * jumpMagnitude, ForceMode.Impulse);
        Vector3 torqueDirection = Random.insideUnitSphere;

        float torqueMagnitude = Random.Range(minTorque, maxTorque);
        rbody.AddTorque(torqueDirection * torqueMagnitude, ForceMode.Impulse);
        
        timeSinceLastJump = 0f;
        nextJumpTime = Random.Range(minJumpInterval, maxJumpInterval);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            groundContactCount++;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            groundContactCount--;
        }
    }
}