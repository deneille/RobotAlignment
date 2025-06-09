using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Win/Lose Screen UI")]
    public GameObject winScreen;   // Assign your win panel in the Inspector
    public GameObject loseScreen;  // Assign your lose panel in the Inspector
    public TMPro.TextMeshProUGUI winText;   // Optional, display win text
    public TMPro.TextMeshProUGUI loseText;  // Optional, display lose text

    [Header("Restart Button UI")]

    public Button restartLoseButton;
    public Button restartWinButton;

    [Header("Level Settings")]
    public int totalQuizzesInLevel = 3;  // Number of quizzes the player must solve
    private int passedQuizCount = 0;
    private int lostQuizCount = 0;

    public static GameManager Instance { get; private set; }
    public GameObject player;
    public GameObject quizPanel; // Assign this reference in the Inspector.
    private Vector3 playerSavedPosition;
    private bool hasShownFirstRobotDialogue = false;

   private HashSet<string> interactedQuizData = new HashSet<string>(); 



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes

            SceneManager.sceneLoaded += OnSceneLoaded; //Reassign references after reload
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
            return;
        }
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        FindAndAssignPlayerAndUIElements();
        ResetQuizCounters();
    }
    public void FindAndAssignPlayerAndUIElements()
    {
        // Reassign Player
        player = GameObject.FindWithTag("Player");

        if (player == null)
            Debug.LogError("Player not found! Ensure player has tag 'Player'.");

        // Find panels (even if inactive)
        quizPanel = Resources.FindObjectsOfTypeAll<GameObject>()
                    .FirstOrDefault(obj => obj.name == "QuizPanel");

        winScreen = Resources.FindObjectsOfTypeAll<GameObject>()
                    .FirstOrDefault(obj => obj.name == "WinPanel");

        loseScreen = Resources.FindObjectsOfTypeAll<GameObject>()
                    .FirstOrDefault(obj => obj.name == "LosePanel");

        // Assign Restart Buttons inside panels
        if (winScreen != null)
        {
            restartWinButton = winScreen.GetComponentsInChildren<Button>(true)
                                .FirstOrDefault(btn => btn.name == "Restart Button");

            if (restartWinButton != null)
            {
                restartWinButton.onClick.RemoveAllListeners();
                restartWinButton.onClick.AddListener(RestartGame);
                Debug.Log("RestartWinButton successfully assigned.");
            }
            else
            {
                Debug.LogWarning("RestartWinButton not found inside WinPanel.");
            }
        }

        if (loseScreen != null)
        {
            restartLoseButton = loseScreen.GetComponentsInChildren<Button>(true)
                                .FirstOrDefault(btn => btn.name == "Restart Button");

            if (restartLoseButton != null)
            {
                restartLoseButton.onClick.RemoveAllListeners();
                restartLoseButton.onClick.AddListener(RestartGame);
                Debug.Log("RestartLoseButton successfully assigned.");
            }
            else
            {
                Debug.LogWarning("RestartLoseButton not found inside LosePanel.");
            }
        }

        Debug.Log("UI elements reassigned.");
    }



    void Start()
    {
        FindAndAssignPlayerAndUIElements(); // Ensure player and UI elements are assigned at the start.
        if (quizPanel != null)
        {
            quizPanel.SetActive(false); // Hide the quiz panel at the start.
        }
        else
        {
            Debug.LogError("Quiz Panel is not assigned. Make sure to assign it in the Inspector.");
        }
        if (restartWinButton != null && restartLoseButton != null)
        {
            restartWinButton.onClick.AddListener(RestartGame); // Add listener to restart button.
            restartLoseButton.onClick.AddListener(RestartGame); // Add listener to restart button.
        }
        else if (restartWinButton != null)
        {
            restartWinButton.onClick.AddListener(RestartGame); // Add listener to restart button.
        }
        else if (restartLoseButton != null)
        {
            restartLoseButton.onClick.AddListener(RestartGame); // Add listener to restart button.
        }
        else
        {
            Debug.LogError("Restart Button is not assigned. Make sure to assign it in the Inspector.");
        }
    }

    public bool AreAllObstaclesDestroyed()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        return obstacles.Length == 0;
    }

    public void CheckObstaclesAndForceLose()
    {
        if (AreAllObstaclesDestroyed() && !AllQuizzesInteracted())
        {
            ShowLoseScreen("All obstacles were destroyed before all directives were followed. Factory meltdown.");
        }
    }

    public void RecordQuizInteraction(string quizName)
    {
        if (!interactedQuizData.Contains(quizName))
        {
            interactedQuizData.Add(quizName);
            Debug.Log("Quiz interacted: " + quizName);
        }
    }

    public bool AllQuizzesInteracted()
    {
        return interactedQuizData.Count >= totalQuizzesInLevel;
    }

    public void ResetQuizCounters()
    {
        passedQuizCount = 0;
        lostQuizCount = 0;
        hasShownFirstRobotDialogue = false;
        interactedQuizData.Clear(); 
        Debug.Log("Quiz counters reset.");
    }

    public void RecordQuizResult(bool success)
    {
        // If you want to be strict: all three quizzes must be passed to win.
        // Here we assume:
        // - Player wins if passedQuizCount == totalQuizzesInLevel.
        // - Player loses if any quiz fails (or if lostQuizCount reaches 1 or 3 — based on your design).
        if (success)
        {
            passedQuizCount++;
            Debug.Log("Quiz Passed. Total Passed: " + passedQuizCount);
        }
        else
        {
            lostQuizCount++;
            Debug.Log("Quiz Failed. Total Failed: " + lostQuizCount);
        }
    }


    public void CheckGameOutcome()
    {
        Debug.Log($"[CheckGameOutcome] Interacted: {interactedQuizData.Count}, Passed: {passedQuizCount}, Lost: {lostQuizCount}, Total Required: {totalQuizzesInLevel}");
        // Only evaluate outcome after all quizzes have been attempted
        if (!AllQuizzesInteracted())
        {
            return; // Still more quizzes left to answer
        }

        // If at least one quiz was failed, but not all
        if (lostQuizCount > 0)
        {
            ShowLoseScreen("Not all directives followed. Factory chaos initiated! Factory meltdown.");
        }
        else
        {
            // All quizzes were passed
            ShowWinScreen("All directives correctly executed. Factory saved!");
        }
       
    }

    public void ResumeGameOrCheckOutcome()
    {
        if (AllQuizzesInteracted())
        {
            // This was the last quiz, check final outcome
            CheckGameOutcome();
        }
        else
        {
            // More quizzes remain, resume game
            ResumeGameAfterQuiz();
        }
    }


    public void SavePlayerPosition()
    {
        if (player == null)
        {
            Debug.LogError("Cannot save position — player is not assigned!");
            return;
        }
        playerSavedPosition = player.transform.position; // Save the player's position.
    }

    public void ResumeGameAfterQuiz()
    {
        // Hide and clear the quiz panel before resuming.
        HideQuizPanel();
        ClearQuizPanel();

        Time.timeScale = 1f;
        player.transform.position = playerSavedPosition; // Move the player back to the saved position.
        Debug.Log("Game resumed after quiz completion.");
    }

    public void ShowWinScreen(string message)
    {
        Debug.Log("Player Wins!");
        Time.timeScale = 0f;  // Pause the game.
        if (winScreen != null)
        {
            winScreen.SetActive(true);
            if (winText != null)
            {
                winText.text = message;
            }
        }
    }

    public void ShowLoseScreen(string message)
    {
        Debug.Log("Player Loses!");
        Time.timeScale = 0f;  // Pause the game.
        if (loseScreen != null)
        {
            loseScreen.SetActive(true);
            if (loseText != null)
            {
                loseText.text = message;
            }
        }
    }

    public void RestartGame()
    {
        ResetQuizCounters();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameObject introPanel = GameObject.Find("IntroDialoguePanel");
        if (introPanel != null)
        {
            introPanel.SetActive(false);
        }
    }

    // This method activates the quiz panel.
    public void ShowQuizPanel()
    {
        if (quizPanel != null)
        {
            quizPanel.SetActive(true);
        }
    }

    // This method hides the quiz panel.
    public void HideQuizPanel()
    {
        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }
    }

    public IEnumerator HideUIAfterDelay(float delay, GameObject uiElement)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (uiElement != null)
            uiElement.SetActive(false);
    }

    // This method clears the quiz panel for future use.
    // You might want to remove previous questions, answers, or reset any states.
    public void ClearQuizPanel()
    {
        if (quizPanel != null)
        {
            foreach (Transform child in quizPanel.transform)
            {
                var textComponent = child.GetComponent<TMPro.TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = ""; // Clear text content.
                }

                var buttonComponent = child.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.RemoveAllListeners(); // Clear button listeners.
                    buttonComponent.interactable = true; // Ensure buttons are active and functional.
                }
            }
            Debug.Log("Quiz panel cleared for reuse.");
        }
    }
    public bool HasShownFirstRobotDialogue()
    {
        return hasShownFirstRobotDialogue;
    }

    //Marks that the first robot dialogue has been shown
    public void SetFirstRobotDialogueShown()
    {
        hasShownFirstRobotDialogue = true;
        Debug.Log("First robot dialogue marked as shown.");
    }

    // Resets the first robot dialogue flag (useful for scene reloads/restarts)
    public void ResetFirstRobotDialogueFlag()
    {
        hasShownFirstRobotDialogue = false;
        Debug.Log("First robot dialogue flag reset.");
    }

}
