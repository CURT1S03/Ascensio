using UnityEngine;

public class CoinVFX : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.5f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // 1. Rotate the coin
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        // 2. Float up and down using a Sine wave
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}