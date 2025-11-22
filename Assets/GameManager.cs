using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject prizeObject; // Drag your prize/win text here
    private int totalCoins;
    private int collectedCoins = 0;

    void Start()
    {
        // Hide the prize at the start
        if (prizeObject != null) prizeObject.SetActive(false);

        // Automatically count how many objects have the "Coin" tag
        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
        Debug.Log("Total Coins to collect: " + totalCoins);
    }

    public void AddCoin()
    {
        collectedCoins++;
        Debug.Log("Coins: " + collectedCoins + "/" + totalCoins);

        if (collectedCoins >= totalCoins)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        Debug.Log("YOU WIN!");
        // Activate the prize
        if (prizeObject != null) prizeObject.SetActive(true);
    }
}