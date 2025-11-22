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
        StartCoroutine(LoadScene("WinScene"));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        transitionAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionDuration);
        SceneManager.LoadScene(sceneName);
    }
}
