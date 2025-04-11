using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Assuming you are using TextMeshPro for UI text
using UnityEngine.UI; // Assuming you are using Unity's UI system for images

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMPro.TextMeshProUGUI dialogueText;

    // Store a reference to the active typewriter coroutine.
    private Coroutine typewriterCoroutine;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Hide dialogue panel at start
        dialoguePanel.SetActive(false);
    }

    public IEnumerator TypeSentence(string sentence, float typingSpeed)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    
    public void ShowDialogue(string message, float duration = 5f)
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }
        
        dialoguePanel.SetActive(true);
        // Start the typewriter effect and store the coroutine reference.
        typewriterCoroutine = StartCoroutine(TypeSentenceAndAutoHide(message, duration));
    }

    private IEnumerator TypeSentenceAndAutoHide(string sentence, float duration)
    {
        dialogueText.text = "";
        // Typewriter effect: reveal sentence character by character.
        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            // Use WaitForSecondsRealtime if using paused time (or adjust to a field for typingSpeed)
            yield return new WaitForSecondsRealtime(0.05f);
        }
        
        // Wait for the specified duration after the full sentence is displayed.
        yield return new WaitForSecondsRealtime(duration);
        dialoguePanel.SetActive(false);
        typewriterCoroutine = null;
    }

    
    // private IEnumerator HideDialogueAfterDelay(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     dialoguePanel.SetActive(false);
    // }
    
    public void HideDialogue()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }
        dialoguePanel.SetActive(false);
    }
}