using UnityEngine;
using TMPro;
using System.Collections;

public class TypingText : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    public float typingSpeed = 0.05f; 
    private bool isTyping = false;
    private bool isTextFullyShown = false;
    private Coroutine typingCoroutine;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }
    
    void Start()
    {
        StartDialogue();
    }

    void Update()
    {
        // Check for any key press to interact with the text state
        if (Input.anyKeyDown)
        {
            if (isTyping)
            {
                // If currently typing, skip to the end
                SkipToTheEnd();
            }
            else if (isTextFullyShown)
            {
                // If text is fully shown, clear it and prepare for next action/dialogue
                ClearText();
            }
        }
    }

    public void StartDialogue()
    {
        if (isTyping || isTextFullyShown) return;
        
        // CRITICAL: Force update BEFORE setting maxVisibleCharacters to 0
        tmp.ForceMeshUpdate();
        tmp.maxVisibleCharacters = 0;
        
        Time.timeScale = 0f;
        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        isTyping = true;
        isTextFullyShown = false;
        
        // Get the character count (should already be updated from StartDialogue)
        int totalCharacters = tmp.textInfo.characterCount;
        
        Debug.Log("Total characters to type: " + totalCharacters);
        print(tmp.text);
        for (int i = 0; i <= totalCharacters; i++)
        {
            tmp.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        // Typing finished
        isTyping = false;
        isTextFullyShown = true;
    }

    private void SkipToTheEnd()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        tmp.maxVisibleCharacters = tmp.textInfo.characterCount;
        isTyping = false;
        isTextFullyShown = true;
    }
    
    private void ClearText()
    {
        tmp.maxVisibleCharacters = 0;
        isTextFullyShown = false;
        Time.timeScale = 1f; 
    }
}