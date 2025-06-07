using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TrueFalseQuiz : MonoBehaviour
{
    public QuizData quizData;

    [Header("UI References")]
    public GameObject quizPanel;         // Panel covering the screen (set up in a global canvas)
    public TextMeshProUGUI quizText;       // Text element for the question prompt
    public Button trueButton;
    public Button falseButton;

    public delegate void QuizResult(bool success);
    public event QuizResult OnQuizResult;

    [Header("Quiz Settings")]
    private string [] questions;
    private bool [] answers;
    private string question;
    private bool correctAnswer;
    private float timeLimit;
    
    private float timer;
    private bool answerGiven = false;

    private void Awake()
    {
        if (trueButton == null)
            trueButton = GameObject.Find("True").GetComponent<Button>();

        if (falseButton == null)
            falseButton = GameObject.Find("False").GetComponent<Button>();

        if (quizText == null)
            quizText = GameObject.Find("Quiz Text").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        // Ensure the quiz panel is hidden at the start.
        quizPanel.SetActive(false);
    }

    // Call this method to start the quiz.
    public void StartQuiz()
    {
        if (trueButton == null || falseButton == null || quizText == null)
        {
            Debug.LogError("Missing UI references! Reinitializing...");
            Awake(); // Reassign references dynamically.
        }

        ResetQuizUI(); // Clear UI for the new quiz.

        // Initialize quiz data.
        int questionIndex = Random.Range(0, quizData.questions.Length);
        question = quizData.questions[questionIndex];
        correctAnswer = quizData.answers[questionIndex];

        quizPanel.SetActive(true);
        quizText.text = question;

        // Assign new listeners for the buttons.
        trueButton.onClick.AddListener(() => AnswerQuiz(true));
        falseButton.onClick.AddListener(() => AnswerQuiz(false));

        // Start the timed countdown.
        timer = quizData.timeLimit;
        StartCoroutine(RunTimer());
    }

    private IEnumerator RunTimer()
    {
        while (timer > 0f && !answerGiven)
        {
            timer -= Time.unscaledDeltaTime;
            yield return null;
        }
        if (!answerGiven)
        {
            quizText.text = "Time expired... directive remains misaligned!";
            OnQuizResult?.Invoke(false);
            yield return new WaitForSecondsRealtime(2f);
            quizPanel.SetActive(false);
        }
    }

    private void AnswerQuiz(bool answer)
    {
        if (answerGiven) return; // Prevent double answers.
        answerGiven = true;
        if (answer == correctAnswer)
        {
            quizText.text = "Calibration complete... directive realigned.";

            // Show correct answer statement *only if* the correct answer is false
            int index = System.Array.IndexOf(quizData.questions, question);
            if (!correctAnswer && index >= 0 && index < quizData.correctAnswers.Length)
            {
                string correctStatement = quizData.correctAnswers[index];
                if (!string.IsNullOrEmpty(correctStatement))
                {
                    quizText.text += $"\nCorrect explanation: {correctStatement}";
                }
            }
            OnQuizResult?.Invoke(true);
        }
        else
        {
            quizText.text = "Error detected... goals remain misaligned!";
            OnQuizResult?.Invoke(false);
        }
        
        // Remove the StopAllCoroutines() call so that coroutine HidePanelAfterDelay can be started.
        // StopAllCoroutines();
        
        // Start the coroutine to hide only the quiz panel after a delay.
        GameManager.Instance.StartCoroutine(GameManager.Instance.HideUIAfterDelay(2f, quizPanel));
    }

    public void ResetQuizUI()
    {
        if (quizText != null)
        {
            quizText.text = ""; // Clear the quiz question text.
        }

        if (trueButton != null)
        {
            trueButton.onClick.RemoveAllListeners(); // Clear previous listeners.
            trueButton.gameObject.SetActive(true); // Ensure the button is active.
        }

        if (falseButton != null)
        {
            falseButton.onClick.RemoveAllListeners(); // Clear previous listeners.
            falseButton.gameObject.SetActive(true); // Ensure the button is active.
        }

        answerGiven = false; // Reset the flag.
        timer = quizData != null ? quizData.timeLimit : 0; // Reset the timer.
        Debug.Log("Quiz UI reset successfully for reuse.");
    }




    private IEnumerator HidePanelAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        quizPanel.SetActive(false);
        // Now that the UI is hidden, resume the game.
        GameManager.Instance.ResumeGameAfterQuiz();
    }
}

