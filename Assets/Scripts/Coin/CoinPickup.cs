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
            GameManager gameManager = FindAnyObjectByType(typeof(GameManager)) as GameManager;
            if(!gameManager) Debug.Log("There needs to be one active GameManager script on a GameObject in your scene.");
            else gameManager.AddCoin();

            // 2. Destroy this coin object
            Destroy(gameObject);
        }
    }
}