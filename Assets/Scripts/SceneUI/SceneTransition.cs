using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;



public class SceneTransition : MonoBehaviour
{
    public Animator transitionAnimator;
    public float transitionDuration = 1f;

    public void LoadNextScene()
    {
        GameManager gameManager = FindAnyObjectByType(typeof(GameManager)) as GameManager;
        Debug.Log("hello: " + gameManager.GetTotalCoins());
        Debug.Log("bye: " + gameManager.GetCollectedCoins());
        if (gameManager.GetTotalCoins() <= gameManager.GetCollectedCoins())
        {
            StartCoroutine(LoadScene("FishWinScene"));
        }
        else
        {
            StartCoroutine(LoadScene("BoatWinScene"));
        }
    }

    private IEnumerator LoadScene(string sceneName)
    {
        transitionAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionDuration);
        SceneManager.LoadScene(sceneName);
    }
}
