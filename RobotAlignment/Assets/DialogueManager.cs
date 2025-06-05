using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;
using UnityEngine.UI; 

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
            return; // Important: exit early to prevent further execution
        }

        // Hide dialogue panel at start
        if (dialoguePanel != null)
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

        if (dialogueText == null && dialoguePanel != null)
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
        if (dialogueText == null) yield break;
        
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
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        typewriterCoroutine = null;
    }

    public void HideDialogue()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"DialogueManager: Scene loaded: {scene.name}");
        
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
                Debug.Log("DialoguePanel reassigned successfully.");
                break;
            }
        }

        if (dialoguePanel == null)
        {
            Debug.LogError("Dialogue Panel not found in the scene after reload.");
            yield break;
        }

        // Also find the dialogue text inside it (even if it's inactive)
        dialogueText = dialoguePanel.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
        if (dialogueText == null)
        {
            Debug.LogError("Dialogue Text not found in Dialogue Panel after reload.");
        }
        else
        {
            Debug.Log("DialogueText reassigned successfully.");
        }
    }
}