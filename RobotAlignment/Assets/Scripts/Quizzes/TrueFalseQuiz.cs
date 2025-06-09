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

    private int questionIndex; // Store the index of the current question

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
        questionIndex = Random.Range(0, quizData.questions.Length);
        question = quizData.questions[questionIndex];
        correctAnswer = quizData.answers[questionIndex];

        quizPanel.SetActive(true);
        quizText.text = question;

        OnQuizResult += GameManager.Instance.RecordQuizResult;


        Debug.Log($"{gameObject.name} is using QuizData: {quizData.name}");


        // Assign new listeners for the buttons.
        trueButton.onClick.AddListener(() => AnswerQuiz(true));
        falseButton.onClick.AddListener(() => AnswerQuiz(false));

        // Start the timed countdown.
        timer = quizData.timeLimit;
        OnQuizResult -= GameManager.Instance.RecordQuizResult;
        OnQuizResult += GameManager.Instance.RecordQuizResult;

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
            // Time expired—show explanation and correct answer
            string correctAnswerText = correctAnswer ? "True" : "False";
            quizText.text = $"Time expired... Correct answer: {correctAnswerText}\nReason: {quizData.correctAnswers[questionIndex]}";

            OnQuizResult?.Invoke(false);
            GameManager.Instance.RecordQuizInteraction(quizData.name); // Count timeout as interaction

            // ✅ Now show explanation AND handle outcome properly
            StartCoroutine(ShowExplanation());
        }
    }

    private void AnswerQuiz(bool answer)
    {
        if (answerGiven) return; // Prevent double answers.
        answerGiven = true;

        bool isCorrect = (answer == correctAnswer);
        OnQuizResult?.Invoke(isCorrect);

        if (isCorrect)
        {
            if (correctAnswer == true)
            {
                quizText.text = "Directive realigned... directive confirmed.";
                quizPanel.SetActive(false);
                Time.timeScale = 1f;

                GameManager.Instance.RecordQuizInteraction(quizData.name); 

                if (GameManager.Instance.AllQuizzesInteracted())
                {
                    GameManager.Instance.CheckGameOutcome();
                }
                else
                {
                    GameManager.Instance.ResumeGameAfterQuiz();
                }
            }
            else
            {
                GameManager.Instance.ClearQuizPanel();
                trueButton.gameObject.SetActive(false);
                falseButton.gameObject.SetActive(false);
                quizText.text = $"Directive confirmed... Reason: {quizData.correctAnswers[questionIndex]}";

                GameManager.Instance.RecordQuizInteraction(quizData.name); 
                StartCoroutine(ShowExplanation());
            }
        }
        else
        {
            GameManager.Instance.ClearQuizPanel();
            trueButton.gameObject.SetActive(false);
            falseButton.gameObject.SetActive(false);
            quizText.text = $"Error detected... Reason: {quizData.correctAnswers[questionIndex]}";

            GameManager.Instance.RecordQuizInteraction(quizData.name); 
            StartCoroutine(ShowExplanation());
        }
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

    private IEnumerator ShowExplanation()
    {
        yield return new WaitForSecondsRealtime(10f); // Wait for 10 seconds to show the explanation.
        quizPanel.SetActive(false); // Hide the quiz panel.
        if (GameManager.Instance.AllQuizzesInteracted())
        {
            GameManager.Instance.CheckGameOutcome(); // All quizzes done, evaluate result.
        }
        else
        {
            GameManager.Instance.ResumeGameAfterQuiz(); // Resume game flow.
        }

        Debug.Log("Explanation closed. Game resumed or outcome evaluated.");
        }
}

