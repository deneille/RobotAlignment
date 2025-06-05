using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro; // Assuming you are using TextMeshPro for UI text
using UnityEngine.UI; // Assuming you are using Unity's UI system for images

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMPro.TextMeshProUGUI dialogueText;

    // Store a reference to the active typewriter coroutine.
    private Coroutine typewriterCoroutine;
    private bool hasInitializedOnce = false;


    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        // Hide dialogue panel at start
        dialoguePanel.SetActive(false);
    }

    void Start()
    {
        if (dialoguePanel == null)
        {
            dialoguePanel = GameObject.Find("DialoguePanel"); // Adjust the name as per your hierarchy
            if (dialoguePanel == null)
            {
                Debug.LogError("Dialogue Panel not found in the scene. Make sure it exists.");
            }
        }

        if (dialogueText == null)
        {
            dialogueText = dialoguePanel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (dialogueText == null)
            {
                Debug.LogError("Dialogue Text not found in the Dialogue Panel. Make sure it exists.");
            }
        }
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
        if (dialoguePanel == null)
        {
            Debug.LogError("dialoguePanel is null. Did you forget to include it in the scene?");
            return;
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
    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (hasInitializedOnce)
        {
            Debug.Log("Reassigning DialoguePanel after scene reload.");
            StartCoroutine(DelayedReassignDialoguePanel());
        }
        else
        {
            hasInitializedOnce = true;
            Debug.Log("First scene load – keeping initial DialoguePanel reference.");
        }
    }

    private IEnumerator DelayedReassignDialoguePanel()
    {
        yield return null; // Wait one frame to allow scene to fully load

        // Search for inactive DialoguePanel in the entire scene
        var allRects = Resources.FindObjectsOfTypeAll<RectTransform>();
        foreach (var rect in allRects)
        {
            if (rect.name == "DialoguePanel")
            {
                dialoguePanel = rect.gameObject;
                break;
            }
        }

        if (dialoguePanel == null)
        {
            Debug.LogError("Dialogue Panel not found in the scene.");
            yield break;
        }

        // Also find the dialogue text inside it (even if it's inactive)
        dialogueText = dialoguePanel.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
        if (dialogueText == null)
        {
            Debug.LogError("Dialogue Text not found in Dialogue Panel.");
        }
    }


    private void ReassignDialoguePanel()
    {
        dialoguePanel = GameObject.Find("DialoguePanel");
        if (dialoguePanel == null)
        {
            Debug.LogError("Dialogue Panel not found in the scene.");
            return;
        }

        dialogueText = dialoguePanel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (dialogueText == null)
        {
            Debug.LogError("Dialogue Text not found in Dialogue Panel.");
        }
    }

}