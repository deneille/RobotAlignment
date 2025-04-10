using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Assuming you are using TextMeshPro for UI text
using UnityEngine.UI; // Assuming you are using Unity's UI system for images

public class EnemyRobot : MonoBehaviour, IInteractable
{
    public bool isEnemy {get; set; } // Flag to indicate if the object is an enemy
    public string enemyId {get; set; } // Unique ID for the enemy

    public CorruptedRobotDialogue enemyRobotDialogue; // Dialogue for the robot
    public GameObject enemyRobotPrefab; // Prefab for the enemy robot
    public TMP_Text dialogueText, enemyName; // Text component for displaying dialogue
    public Sprite fixedRobotSprite; // Fixed sprite for the robot

    private int dialogueIndex; // Index for the current dialogue line
    private bool isTyping, isDialogueActive; // Flag to check if dialogue is active
    private enum State{
        Idle,
        Move,
        Attack,
    }
    private State currentState;
    // private EnemyPathfinding pathfinding;

    private void Awake()
    {
        // pathfinding = GetComponent<EnemyPathfinding>();
        currentState = State.Idle;
    }
    private void Start()
    {
        enemyId ??= GlobalHelper.GetUniqueId(gameObject); // Set the enemy ID to the name of the GameObject
        isEnemy = true; // Set the enemy state to true
        // StartCoroutine(IdlingRoutine());
    }
    public void Interact(){
        // if(!CanInteract()){
        //     Debug.Log("Cannot interact with this object");
        //     return;
        // }
        
        // Debug.Log($"Challenging {enemyId} to a duel!");
        // ChallengeEnemy();
        if(enemyRobotDialogue == null || isDialogueActive){
            
            Debug.Log("Dialogue is either null or currently active, cannot interact.");
            return;
        }
        else if(isDialogueActive){
            NextDialogue();
        }
        else{
            StartDialogue(); // Start the dialogue if it's not already active
            Debug.Log($"Starting dialogue with {enemyId}");
        }
    }
    void StartDialogue(){
        isDialogueActive = true; // Set the dialogue state to active
        dialogueIndex = 0; // Reset the dialogue index
        enemyName.text = enemyRobotDialogue.robotName; // Set the robot name in the UI
        enemyRobotPrefab.SetActive(true); // Activate the robot prefab
        //need to pause game

        StartCoroutine(TypeDialogue()); // Start typing the dialogue
    }
    IEnumerator TypeDialogue(){
        isTyping = true;
        dialogueText.text = ""; // Clear the text
        foreach(char letter in enemyRobotDialogue.dialogueLines[dialogueIndex].ToCharArray()){
            dialogueText.text += letter;
            yield return new WaitForSeconds(enemyRobotDialogue.typingSpeed); // Wait for the typing speed  
        }
        isTyping = false;

        if(enemyRobotDialogue.autoProgressLines[dialogueIndex] && enemyRobotDialogue.autoProgressLines.Length > dialogueIndex){
            // If the current line is set to auto-progress, wait for the delay
            yield return new WaitForSeconds(enemyRobotDialogue.autoProgressDelay); // Wait for the auto-progress delay
            NextDialogue(); // Move to the next dialogue line
        }
    }
    public void NextDialogue(){
        if(isTyping){
            StopAllCoroutines(); // Stop typing if the player clicks
            dialogueText.text = enemyRobotDialogue.dialogueLines[dialogueIndex]; // Show the full text immediately
            isTyping = false;
        }
        if(dialogueIndex < enemyRobotDialogue.dialogueLines.Length - 1){
            dialogueIndex++; // Move to the next dialogue line
            StartCoroutine(TypeDialogue()); // Start typing the next line
        } else {
            EndDialogue(); // End the dialogue if there are no more lines
        }
    }
    public void EndDialogue(){
        StopAllCoroutines(); // Stop all coroutines
        isDialogueActive = false; // Set the dialogue state to inactive
        enemyRobotPrefab.SetActive(false); // Deactivate the robot prefab
        dialogueText.text = ""; // Clear the text
        enemyName.text = ""; // Clear the name text
        //Need to unpaise the game.
        Debug.Log("Dialogue ended");
    }

    public bool CanInteract(){
        if(isEnemy == true){
            Debug.Log("Object is an enemy");
        }
        return isEnemy && !isDialogueActive; // Check if the object is an enemy
    }

    private void ChallengeEnemy()
    {
        // Logic to challenge the player to a duel
        Debug.Log("Starting puzzle gameplay...");
        
        // Set isEnemy to false AFTER the interaction
        SetIsEnemy(false);
    }

    public void SetIsEnemy(bool _isEnemy)
    {
        // Only update if the state is different (prevents recursion)
        if(isEnemy != _isEnemy){
            isEnemy = _isEnemy;
            
            if(!isEnemy) {
                // When changing to non-enemy (fixed) state
                GetComponent<SpriteRenderer>().sprite = fixedRobotSprite;
                Debug.Log("Robot has been fixed and is no longer hostile");
            } else {
                // If you have an enemy sprite, you could set it here
                // GetComponent<SpriteRenderer>().sprite = enemyRobotSprite;
                Debug.Log("Robot is now hostile");
            }
        }
    }
    private IEnumerator IdlingRoutine()
    {
        while (currentState == State.Idle)
        {
            Vector2 idlePosition = GetIdlePosition();
            // pathfinding.MoveTo(idlePosition);
            yield return new WaitForSeconds(2f);
        }
    }
    private Vector2 GetIdlePosition()
    {
        // Logic to get a random idle position within the patrol area
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentState = State.Attack;
            // pathfinding.MoveTo(other.transform.position); // Move towards the player
            // pathfinding.enabled = false; // Disable pathfinding to stop movement
            StartCoroutine(AttackRoutine(other.transform));
        }
        else if (other.CompareTag("Obstacle"))
        {
            currentState = State.Attack;
            // pathfinding.MoveTo(other.transform.position); // Move towards the obstacle
            //Stop enemy from moving
            // pathfinding.enabled = false; // Disable pathfinding to stop movement
            // Start attacking the obstacle or player
            StartCoroutine(AttackRoutine(other.transform));
        }
    }
    private IEnumerator AttackRoutine(Transform target)
    {
        while (currentState == State.Attack)
        {
            // Logic to attack the player
            // For example, shoot a projectile or deal damage
            yield return new WaitForSeconds(1f); // Attack cooldown
        }
    }
}
