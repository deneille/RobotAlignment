using System.Collections;
using UnityEngine;
using TMPro;

public class GameIntroDialogue : MonoBehaviour
{
    [Header("UI References")]
    // The panel that contains your dialogue/rules text
    public GameObject introDialoguePanel;
    // The text component that displays your rules
    public TextMeshProUGUI introDialogueText;

    [Header("Dialogue Settings")]
    // The rules text that explains how to win and lose.
    [TextArea(3, 10)]
    public string rulesText = "Rules:\n- Use E to interact + duel with robots through time-based quizzes.\n- Answer all quizzes correctly to win.\n- If you fail 3 quizzes or if enemy robots destroy all obstacles, you lose.\n\nGood luck!";
    // How long to keep the dialogue on screen (in seconds)
    public float dialogueDuration = 10f;

    private void Start()
    {
        // Start the intro dialogue when the scene begins.
        StartCoroutine(ShowIntroDialogue());
    }

    private IEnumerator ShowIntroDialogue()
    {
        // Activate the dialogue panel.
        if (introDialoguePanel != null)
        {
            introDialoguePanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Intro Dialogue Panel is not assigned.");
        }

        // Set the text to the rules.
        if (introDialogueText != null)
        {
            introDialogueText.text = rulesText;
        }
        else
        {
            Debug.LogError("Intro Dialogue Text is not assigned.");
        }

        // Pause the game.
        Time.timeScale = 0f;
        Debug.Log("Game paused. Showing intro dialogue...");

        // Wait for the specified duration using real time (ignores Time.timeScale).
        yield return new WaitForSecondsRealtime(dialogueDuration);

        // Hide the dialogue panel.
        if (introDialoguePanel != null)
        {
            introDialoguePanel.SetActive(false);
        }
        // Resume the game.
        Time.timeScale = 1f;
        Debug.Log("Intro dialogue finished. Game resumed.");
    }
}
