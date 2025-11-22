using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Check if the object bumping into us is the Player
        if (other.CompareTag("Player"))
        {
            // 1. Tell the GameManager we collected a coin
            // Note: This looks for the script in the scene
            FindObjectOfType<GameManager>().AddCoin();

            // 2. Destroy this coin object
            Destroy(gameObject);
        }
    }
}