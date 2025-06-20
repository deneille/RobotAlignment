using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyRobot : MonoBehaviour, IInteractable
{
    public bool isEnemy { get; private set; }
    public string enemyId;
    public Sprite fixedRobotSprite;

    [Header("Dialogue")]
    [SerializeField] private RobotDialogue robotDialogue; // Reference to dialogue scriptable object
    private int currentDialogueIndex = 0;
    private bool isInDialogue = false;

    [Header("Quizzes")]
    public TrueFalseQuiz trueFalseQuiz;
    public YesNoQuiz yesNoQuiz;

    [Header("Movement & Targeting")]
    [SerializeField] private EnemyPathfinding pathfinder; // Reference to the pathfinding script
    [SerializeField] private LayerMask targetLayerMask;

    // For simplicity, we only use two states here.
    private enum State { Idle, Move }
    private State currentState;

    // Flag indicating whether the quiz has been completed.
    private bool completedQuiz = false;

    private void Awake()
    {
        currentState = State.Idle;
    }

    private void Start()
    {
        // Assign a unique ID if not already set.
        if (string.IsNullOrEmpty(enemyId))
            enemyId = GlobalHelper.GetUniqueId(gameObject);

        isEnemy = true;
        currentState = State.Idle;
        // want to wait 20 seconds before switching states to move
        StartCoroutine(WaitAndSwitchState(20f));

        // (Optional) Start any movement/hostile behavior here.
        StartCoroutine(RunAndDestroyTarget());
    }

    private IEnumerator WaitAndSwitchState(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        currentState = State.Move;
    }

    // Called by your interaction system when the player interacts with this enemy.
    public void Interact()
    {
        if (isInDialogue)
        {
            Debug.Log("Already in dialogue, cannot interact again.");
            return;
        }
        if (!CanInteract())
        {
            Debug.Log("Cannot interact with this object.");
            return;
        }

        GameManager.Instance.SavePlayerPosition();

        // Check if this is the first robot interaction in the current game session
        if (!GameManager.Instance.HasShownFirstRobotDialogue())
        {
            Debug.Log($"Showing first robot dialogue from: {enemyId}");
            GameManager.Instance.SetFirstRobotDialogueShown(); // Mark as shown
            Time.timeScale = 0f;
            StartCoroutine(PlayDialogueSequence()); // includes quiz at end
        }
        else
        {
            Debug.Log($"Skipping dialogue for {enemyId}, going straight to quiz.");
            Time.timeScale = 0f;
            StartQuizOnly(); // skip dialogue and go straight to quiz
        }
    }

    private void StartQuizOnly()
    {
        if (trueFalseQuiz != null)
        {
            trueFalseQuiz.ResetQuizUI();
            trueFalseQuiz.OnQuizResult += HandleQuizResult;
            if (!trueFalseQuiz.gameObject.activeInHierarchy)
                trueFalseQuiz.gameObject.SetActive(true);
            trueFalseQuiz.StartQuiz();
        }
        else if (yesNoQuiz != null)
        {
            yesNoQuiz.ResetQuizUI();
            yesNoQuiz.OnQuizResult += HandleQuizResult;
            if (!yesNoQuiz.gameObject.activeInHierarchy)
                yesNoQuiz.gameObject.SetActive(true);
            yesNoQuiz.StartQuiz();
        }

        completedQuiz = true;
    }

    // Determines if the enemy can currently be interacted with.
    public bool CanInteract() => isEnemy && !completedQuiz;

    // Handles the dialogue sequence then starts the quiz.
    private IEnumerator PlayDialogueSequence()
    {
        if (robotDialogue == null || robotDialogue.dialogueLines.Length == 0)
        {
            Debug.LogWarning("No dialogue found for robot: " + enemyId);
            DialogueManager.Instance.ShowDialogue($"Robot {enemyId} challenges you: 'Error... undefined directive...'");
            yield return new WaitForSecondsRealtime(1f);
            EndInteraction();
            yield break;
        }

        isInDialogue = true;
        currentDialogueIndex = 0;

        while (currentDialogueIndex < robotDialogue.dialogueLines.Length)
        {
            string currentLine = robotDialogue.dialogueLines[currentDialogueIndex];
            // Display the current dialogue line.
            DialogueManager.Instance.ShowDialogue(currentLine, robotDialogue.autoProgressDelay);

            if (currentDialogueIndex < robotDialogue.autoProgressLines.Length &&
                robotDialogue.autoProgressLines[currentDialogueIndex])
            {
                // Auto-progress after delay (using realtime because the game is paused)
                yield return new WaitForSecondsRealtime(robotDialogue.autoProgressDelay);
                currentDialogueIndex++;
            }
            else
            {
                // Wait until the player presses Space/Return/E to advance.
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) ||
                                                 Input.GetKeyDown(KeyCode.Return) ||
                                                 Input.GetKeyDown(KeyCode.E));
                yield return new WaitForEndOfFrame(); // small delay to prevent immediate progression.
                currentDialogueIndex++;
            }
        }

        isInDialogue = false;
        DialogueManager.Instance.HideDialogue();

        // --- Start the Quiz ---
        // Make sure the quiz object is active before starting its coroutine.
        if (trueFalseQuiz != null)
        {
            trueFalseQuiz.ResetQuizUI(); // Reset the quiz UI before starting.
            trueFalseQuiz.OnQuizResult += HandleQuizResult; // Subscribe to the quiz result event.
            if (!trueFalseQuiz.gameObject.activeInHierarchy)
                trueFalseQuiz.gameObject.SetActive(true);

            trueFalseQuiz.StartQuiz();
        }
        else if (yesNoQuiz != null)
        {
            yesNoQuiz.ResetQuizUI();
            yesNoQuiz.OnQuizResult += HandleQuizResult; // Subscribe to the quiz result event.
            if (!yesNoQuiz.gameObject.activeInHierarchy)
                yesNoQuiz.gameObject.SetActive(true);

            yesNoQuiz.StartQuiz();
        }

        completedQuiz = true;
    }

    // Called after dialogue (and quiz start) to unpause the game.
    private void EndInteraction()
    {
        Debug.Log("Ending interaction with robot: " + enemyId);
        GameManager.Instance.ResumeGameAfterQuiz();
    }

    private IEnumerator RunAndDestroyTarget()
    {
        while (true)
        {
            // If no obstacles remain, end the game.
            GameObject[] remainingObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
            if (remainingObstacles.Length == 0)
            {
                Debug.Log("No obstacles remain. Ending game.");
                EndGame();
                yield break;
            }

            // Find the nearest obstacle using your filtering routine.
            Collider2D targetCollider = FindClosestTarget();
            if (targetCollider == null)
            {
                Debug.Log("No target object found.");
                yield return new WaitForSeconds(1.0f);
                continue;
            }

            // Use the obstacle's transform as the starting target.
            Vector2 targetPosition = targetCollider.transform.position;
            Debug.Log($"Running towards target at {targetPosition}");
            currentState = State.Move;
            pathfinder.MoveTo(targetPosition);

            // Timers and thresholds.
            float maxWaitTime = 8f;
            float elapsedTime = 0f;
            float stuckTime = 0f;
            float lastDistance = float.MaxValue;
            float proximityThreshold = 0.8f; // "Close enough" threshold.
            bool reached = false;

            while (targetCollider != null && elapsedTime < maxWaitTime)
            {
                // Instead of comparing transform positions (which are grid-centered),
                // we use the actual collider's geometry.
                Vector2 closestPoint = targetCollider.ClosestPoint(transform.position);
                float currentDistance = Vector2.Distance(transform.position, closestPoint);
                Debug.Log($"Distance to target (closest point): {currentDistance}, Enemy pos: {transform.position}");

                if (currentDistance <= proximityThreshold)
                {
                    reached = true;
                    break;
                }

                // Check for progress; if little progress is made, increase the stuck timer.
                if (Mathf.Abs(currentDistance - lastDistance) < 0.01f)
                {
                    stuckTime += Time.deltaTime;
                }
                else
                {
                    stuckTime = 0f;
                    lastDistance = currentDistance;
                }

                // If stuck for over 2 seconds, change direction.
                if (stuckTime > 2f)
                {
                    Debug.LogWarning("Enemy stuck for over 2 seconds. Changing direction...");
                    // Instead of basing the new target off the obstacle,
                    // use the enemy's current position plus a random offset.
                    Vector2 randomOffset = Random.insideUnitCircle.normalized;
                    float offsetDistance = 3.0f; // Adjust this to force a more pronounced turn.
                    Vector2 newTargetPosition = (Vector2)transform.position + randomOffset * offsetDistance;
                    Debug.Log($"New target position (relative to enemy): {newTargetPosition}");
                    pathfinder.MoveTo(newTargetPosition);

                    // Reset timers after changing direction.
                    stuckTime = 0f;
                    elapsedTime = 0f;
                    lastDistance = currentDistance;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // If within threshold, register a hit.
            if (targetCollider != null && reached && targetCollider.CompareTag("Obstacle"))
            {
                FactoryInventory inventory = targetCollider.GetComponent<FactoryInventory>();
                if (inventory != null)
                {
                    Debug.Log($"Before hit: {targetCollider.gameObject.name} has {inventory.GetHitCount()} hit(s).");
                    inventory.AddHit();
                    Debug.Log($"After hit: {targetCollider.gameObject.name} now has {inventory.GetHitCount()} hit(s).");

                    if (inventory.GetHitCount() >= 2)
                    {
                        Debug.Log("Obstacle destroyed after two hits.");
                        Destroy(targetCollider.gameObject);
                        GameManager.Instance.CheckObstaclesAndForceLose();

                    }
                    else
                    {
                        Debug.Log("Obstacle has been hit once, awaiting second hit.");
                    }
                }
                else
                {
                    Destroy(targetCollider.gameObject);
                    Debug.Log("Obstacle destroyed (no inventory component).");
                    GameManager.Instance.CheckObstaclesAndForceLose();
                }
            }
            else
            {
                Debug.Log("Target not reached or no longer valid.");
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    // private IEnumerator RunAndDestroyTarget()
    // {
    //     while (true)
    //     {
    //         // If no obstacles remain, end the game.
    //         GameObject[] remainingObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
    //         if (remainingObstacles.Length == 0)
    //         {
    //             Debug.Log("No obstacles remain. Ending game.");
    //             EndGame();
    //             yield break;
    //         }

    //         // Find the nearest obstacle using your filtering routine.
    //         Collider2D targetCollider = FindClosestTarget();
    //         if (targetCollider == null)
    //         {
    //             Debug.Log("No target object found in layer mask.");
    //             yield return new WaitForSeconds(0.5f);
    //             continue;
    //         }

    //         // Use the obstacle's transform as the starting target.
    //         Vector2 targetPosition = targetCollider.transform.position;
    //         Debug.Log($"Running towards target at {targetPosition}");
    //         currentState = State.Move;
    //         pathfinder.MoveTo(targetPosition);

    //         // Timers and thresholds.
    //         float maxWaitTime = 10f;
    //         float elapsedTime = 0f;
    //         float stuckTime = 0f;
    //         float lastDistance = float.MaxValue;
    //         float proximityThreshold = 1.0f; // "Close enough" threshold.
    //         bool reached = false;

    //         while (targetCollider != null && elapsedTime < maxWaitTime)
    //         {
    //             // Instead of comparing transform positions (which are grid-centered),
    //             // we use the actual collider's geometry.
    //             Vector2 closestPoint = targetCollider.ClosestPoint(transform.position);
    //             float currentDistance = Vector2.Distance(transform.position, closestPoint);
    //             Debug.Log($"Distance to target (closest point): {currentDistance}, Enemy pos: {transform.position}");

    //             if (currentDistance <= proximityThreshold)
    //             {
    //                 reached = true;
    //                 break;
    //             }

    //             // Check for progress; if little progress is made, increase the stuck timer.
    //             if (Mathf.Abs(currentDistance - lastDistance) < 0.01f)
    //             {
    //                 stuckTime += Time.deltaTime;
    //             }
    //             else
    //             {
    //                 stuckTime = 0f;
    //                 lastDistance = currentDistance;
    //             }

    //             // If stuck for over 5 seconds, change direction.
    //             if (stuckTime > 5f)
    //             {
    //                 Debug.LogWarning("Enemy stuck for over 5 seconds. Changing direction...");
    //                 // Instead of basing the new target off the obstacle,
    //                 // use the enemy's current position plus a random offset.
    //                 Vector2 randomOffset = Random.insideUnitCircle.normalized;
    //                 float offsetDistance = 3.0f; // Adjust this to force a more pronounced turn.
    //                 Vector2 newTargetPosition = (Vector2)transform.position + randomOffset * offsetDistance;
    //                 Debug.Log($"New target position (relative to enemy): {newTargetPosition}");
    //                 pathfinder.MoveTo(newTargetPosition);

    //                 // Reset timers after changing direction.
    //                 stuckTime = 0f;
    //                 elapsedTime = 0f;
    //                 lastDistance = currentDistance;
    //             }

    //             elapsedTime += Time.deltaTime;
    //             yield return null;
    //         }

    //         // If within threshold, register a hit.
    //         if (targetCollider != null && reached && targetCollider.CompareTag("Obstacle"))
    //         {
    //             FactoryInventory inventory = targetCollider.GetComponent<FactoryInventory>();
    //             if (inventory != null)
    //             {
    //                 Debug.Log($"Before hit: {targetCollider.gameObject.name} has {inventory.GetHitCount()} hit(s).");
    //                 inventory.AddHit();
    //                 Debug.Log($"After hit: {targetCollider.gameObject.name} now has {inventory.GetHitCount()} hit(s).");

    //                 if (inventory.GetHitCount() >= 2)
    //                 {
    //                     Debug.Log("Obstacle destroyed after two hits.");
    //                     Destroy(targetCollider.gameObject);
    //                 }
    //                 else
    //                 {
    //                     Debug.Log("Obstacle has been hit once, awaiting second hit.");
    //                 }
    //             }
    //             else
    //             {
    //                 Destroy(targetCollider.gameObject);
    //                 Debug.Log("Obstacle destroyed (no inventory component).");
    //             }
    //         }
    //         else
    //         {
    //             Debug.Log("Target not reached or no longer valid.");
    //         }

    //         yield return new WaitForSeconds(0.5f);
    //     }
    // }

    private void EndGame()
    {
        Debug.Log("Game Over! All obstacles have been destroyed.");
        // Place your game-ending logic here (for example: load a Game Over screen).
        GameManager.Instance.CheckGameOutcome(); // Assuming you have a method to handle game over logic.
    }

    /// Finds the closest obstacle using an overlap circle and filtering by tag/layer.
    private Collider2D FindClosestTarget()
    {
        // Much larger search radius since we're prioritizing factories
        float searchRadius = 50f; 
        Collider2D[] allColliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        
        List<Collider2D> factories = new List<Collider2D>();
        
        // Only collect FactoryInventory objects, completely ignore blocks
        foreach (Collider2D col in allColliders)
        {
            if (col.CompareTag("Obstacle") && col.GetComponent<FactoryInventory>() != null)
            {
                factories.Add(col);
            }
        }
        
        if (factories.Count == 0)
        {
            Debug.Log("No FactoryInventory objects found in search radius!");
            return null;
        }
        
        Debug.Log($"Found {factories.Count} FactoryInventory objects to target");
        
        // Find the closest factory using grid-based distance (Manhattan distance)
        Collider2D closestFactory = null;
        float shortestGridDistance = float.MaxValue;
        
        foreach (Collider2D factory in factories)
        {
            // Use Manhattan distance for grid-based pathfinding
            Vector2 diff = factory.transform.position - transform.position;
            float gridDistance = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);
            
            // Prioritize factories with fewer hits (easier to destroy)
            FactoryInventory inventory = factory.GetComponent<FactoryInventory>();
            float hitPenalty = inventory.GetHitCount() * 2f; // Less damaged = higher priority
            float totalScore = gridDistance + hitPenalty;
            
            if (totalScore < shortestGridDistance)
            {
                shortestGridDistance = totalScore;
                closestFactory = factory;
            }
        }
        
        if (closestFactory != null)
        {
            Debug.Log($"Targeting factory: {closestFactory.name} at position {closestFactory.transform.position}");
        }
        
        return closestFactory;
    }

    /// Handles the quiz result.
    /// If the quiz is passed, fix the enemy (change sprite/color, set to idle) and stop hostile routines.
    /// Otherwise, resume hostile behavior.
    private void HandleQuizResult(bool success)
    {
        if (success)
        {
            // Fix the enemy (change its appearance and set to idle).
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.green; // Or swap out the sprite:
                // sr.sprite = fixedRobotSprite;
            }
            currentState = State.Idle;
            StopAllCoroutines(); // Stop the run and destroy routine.
            GameManager.Instance.ResumeGameAfterQuiz();
            GameManager.Instance.RecordQuizResult(success);
        }
        else
        {
            Debug.Log("Quiz failed! The misalignment persists.");
            // Only resume hostility if the quiz failed.
            StartCoroutine(RunAndDestroyTarget());
            GameManager.Instance.ResumeGameAfterQuiz();
            GameManager.Instance.RecordQuizResult(success);
        }

        // Unsubscribe from quiz events to avoid duplicate calls.
        if (trueFalseQuiz != null)
            trueFalseQuiz.OnQuizResult -= HandleQuizResult;
        if (yesNoQuiz != null)
            yesNoQuiz.OnQuizResult -= HandleQuizResult;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isInDialogue = false;
        currentDialogueIndex = 0;
    }
}

 


