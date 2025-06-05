using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class YesNoQuiz : MonoBehaviour
{
    public QuizData quizData;

    [Header("UI References")]
    public GameObject quizPanel;         // Panel covering the screen (set up in a global canvas)
    public TextMeshProUGUI quizText;       // Text element for the question prompt
    public Button yesButton;
    public Button noButton;

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

    private void Start()
    {
        // Ensure the quiz panel is hidden at the start.
        quizPanel.SetActive(false);
    }

    // Call this method to start the quiz.
    public void StartQuiz()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            Debug.Log("YesNoQuiz gameobject activated inside StartQuiz.");
        }
        if(quizData == null) return; // Ensure quizData is assigned before starting the quiz.
        ResetQuizUI(); // Reset the UI before starting a new quiz.
        // Initialize quiz data.
        int questionIndex = Random.Range(0, quizData.questions.Length);
        question = quizData.questions[questionIndex];
        correctAnswer = quizData.answers[questionIndex];
        timeLimit = quizData.timeLimit;

        answerGiven = false;
        timer = timeLimit;

        quizPanel.SetActive(true);
        quizText.text = question;

        // Clear any previous button listeners.
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(() => AnswerQuiz(true));
        noButton.onClick.AddListener(() => AnswerQuiz(false));

        // Start the timed countdown.
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
        answerGiven = false;    // Reset the flag so a new answer can be accepted.
        quizText.text = "";     // Clear out any previous question or feedback text.
        
        // Remove old listeners so you don't get duplicate calls.
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        
        // Reset the timer if you display it on screen.
        timer = timeLimit;
        
        Debug.Log("Quiz UI reset");
    }

    private IEnumerator HidePanelAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        quizPanel.SetActive(false);
        // Now that the UI is hidden, resume the game.
        GameManager.Instance.ResumeGameAfterQuiz();
    }
}