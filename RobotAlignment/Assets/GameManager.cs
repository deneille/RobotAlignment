using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Win/Lose Screen UI")]
    public GameObject winScreen;   // Assign your win panel in the Inspector
    public GameObject loseScreen;  // Assign your lose panel in the Inspector
    public TMPro.TextMeshProUGUI winText;   // Optional, display win text
    public TMPro.TextMeshProUGUI loseText;  // Optional, display lose text

    [Header("Level Settings")]
    public int totalQuizzesInLevel = 3;  // Number of quizzes the player must solve
    private int passedQuizCount = 0;
    private int lostQuizCount = 0;

    public static GameManager Instance { get; private set; }
    public GameObject player;
    public GameObject quizPanel; // Assign this reference in the Inspector.
    private Vector3 playerSavedPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this instance alive across scenes.
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances.
        }
    }

    public bool AreAllObstaclesDestroyed()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        return obstacles.Length == 0;
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

        // Now check the conditions.
        CheckGameOutcome();
    }

    private void CheckGameOutcome()
    {
        // Condition: If any obstacles have been destroyed by enemy robot before finishing quizzes, player loses.
        if (AreAllObstaclesDestroyed())
        {
            ShowLoseScreen("Enemies destroyed all obstacles!");
            return;
        }
        
        // Condition: If player has lost any quiz (or if lostQuizCount reaches a threshold, e.g. 1 or 3).
        // Here, we assume if even one quiz is wrong the game is lost. Adapt accordingly.
        if (lostQuizCount > 0)
        {
            ShowLoseScreen("You failed a quiz!");
            return;
        }
        
        // If player has passed all quizzes, win the game.
        if (passedQuizCount == totalQuizzesInLevel)
        {
            ShowWinScreen("All quizzes solved correctly!");
        }
    }
    

    public void SavePlayerPosition()
    {
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
            if(winText != null)
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
            if(loseText != null)
            {
                loseText.text = message;
            }
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Start Scene");
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
        if(uiElement != null)
            uiElement.SetActive(false);
    }

    // This method clears the quiz panel for future use.
    // You might want to remove previous questions, answers, or reset any states.
    public void ClearQuizPanel()
    {
        if (quizPanel != null)
        {
            // Option 1: If your quiz panel contains dynamic UI elements (like text or buttons)
            // you could iterate over them and clear or destroy their content.
            foreach (Transform child in quizPanel.transform)
            {
                // Here we're simply destroying child game objects.
                // Alternatively, you can reset their state instead of destroying.
                Destroy(child.gameObject);
            }
        }
    }
}
