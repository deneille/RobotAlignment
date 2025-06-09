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
    private string[] questions;
    private bool[] answers;
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


        Debug.Log($"{gameObject.name} is using QuizData: {quizData.name}");


        // Assign new listeners for the buttons.
        trueButton.onClick.AddListener(() => AnswerQuiz(true));
        falseButton.onClick.AddListener(() => AnswerQuiz(false));



        // trueButton.onClick.RemoveAllListeners();
        // falseButton.onClick.RemoveAllListeners();

        OnQuizResult -= GameManager.Instance.RecordQuizResult;
        OnQuizResult += GameManager.Instance.RecordQuizResult;

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
            answerGiven = true; // Set the flag to prevent further answers.
            // Record interaction and failure
            GameManager.Instance.RecordQuizInteraction(quizData.name);
            OnQuizResult?.Invoke(false); // Timer expired = failed

            // Hide buttons and show timeout explanation
            trueButton.gameObject.SetActive(false);
            falseButton.gameObject.SetActive(false);

            string correctAnswerText = correctAnswer ? "True" : "False";
            quizText.text = $"Time expired! Correct answer: {correctAnswerText}\nReason: {quizData.correctAnswers[questionIndex]}";

            StartCoroutine(ShowExplanationThenContinue()); // Keep delay if more quizzes remain
        }
    }

    private void AnswerQuiz(bool answer)
    {
        if (answerGiven) return; // Prevent double answers.
        answerGiven = true;

        bool isCorrect = (answer == correctAnswer);

        GameManager.Instance.RecordQuizInteraction(quizData.name); // Record interaction regardless of correctness

        OnQuizResult?.Invoke(isCorrect);

        bool showExplanation = !isCorrect || (isCorrect && correctAnswer == false); // Flag to show explanation after answering

        if (showExplanation)
        {
            trueButton.gameObject.SetActive(false); // Hide the true button
            falseButton.gameObject.SetActive(false); // Hide the false button

            string explanationText;
            if (isCorrect && correctAnswer == false)
            {
                explanationText = $"Directive confirmed...Reason: {quizData.correctAnswers[questionIndex]}";
            }
            else
            {
                explanationText = $"Error detected...Reason: {quizData.correctAnswers[questionIndex]}";
            }
            quizText.text = explanationText;

            StartCoroutine(ShowExplanation()); // Only delay if more quizzes left
            
        }
        else
        {
            // Correct answer was True - no explanation needed
            quizText.text = "Directive realigned... directive confirmed.";
            StartCoroutine(HideQuizAndContinue(1f)); // Brief pause then continue
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
        Time.timeScale = 0f;
        quizPanel.SetActive(true); // Ensure the quiz panel is active

        float elapsed = 0f;
        while (elapsed < 10f)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        quizPanel.SetActive(false);
        Time.timeScale = 1f;

        // Always resume game after explanation - win/lose decision happens only after ALL quizzes
        if (GameManager.Instance.AllQuizzesInteracted())
        {
            // This was the last quiz - now check final outcome
            GameManager.Instance.CheckGameOutcome();
        }
        else
        {
            // More quizzes remain - continue playing
            GameManager.Instance.ResumeGameAfterQuiz();
        }
    }

    private IEnumerator ShowExplanationThenContinue()
    {
        Time.timeScale = 0f;
        quizPanel.SetActive(true);

        float elapsed = 0f;
        while (elapsed < 10f)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        quizPanel.SetActive(false);
        Time.timeScale = 1f;
        if (GameManager.Instance.AllQuizzesInteracted())
        {
            GameManager.Instance.CheckGameOutcome(); // All quizzes done, evaluate result.
        }
        else
        {
            GameManager.Instance.ResumeGameAfterQuiz(); // Resume game flow.
        }

        Debug.Log("Explanation shown. Game resumed or outcome evaluated.");
    }

    private IEnumerator HideQuizAndContinue(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        quizPanel.SetActive(false);
        Time.timeScale = 1f;

        // Always resume game - win/lose decision happens only after ALL quizzes
        if (GameManager.Instance.AllQuizzesInteracted())
        {
            // This was the last quiz - now check final outcome
            GameManager.Instance.CheckGameOutcome();
        }
        else
        {
            // More quizzes remain - continue playing
            GameManager.Instance.ResumeGameAfterQuiz();
        }
    }
}
