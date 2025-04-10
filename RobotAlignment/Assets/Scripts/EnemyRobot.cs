using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRobot : MonoBehaviour, IInteractable
{
    public bool isEnemy {get; set; } // Flag to indicate if the object is an enemy
    public string enemyId {get; set; } // Unique ID for the enemy
    public Sprite fixedRobotSprite; // Fixed sprite for the robot
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
        if(!CanInteract()){
            Debug.Log("Cannot interact with this object");
            return;
        }
        
        Debug.Log($"Challenging {enemyId} to a duel!");
        ChallengeEnemy();
    }

    public bool CanInteract(){
        if(isEnemy == true){
            Debug.Log("Object is an enemy");
        }
        return isEnemy; // Check if the object is an enemy
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
