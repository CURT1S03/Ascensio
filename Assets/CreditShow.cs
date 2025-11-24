using UnityEngine;

public class CreditShow : MonoBehaviour
{
    public GameObject creditsScreen;
    public GameObject mainMenuScreen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        creditsScreen.SetActive(false);
        mainMenuScreen.SetActive(true);
    }

    // Update is called once per frame
    public void showCreditScreen()
    {
        creditsScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }

    public void showMainMenu()
    {
        creditsScreen.SetActive(false);
        mainMenuScreen.SetActive(true);
    }
}
