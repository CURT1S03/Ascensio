using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenuToggle : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Pause Menu active: {canvasGroup.interactable}, Raycasts: {canvasGroup.blocksRaycasts}, Alpha: {canvasGroup.alpha}");
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (canvasGroup.interactable)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0f;
                Time.timeScale = 1f;
            }
            else
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1f;
                Time.timeScale = 0f;
            }
        }
    }
}