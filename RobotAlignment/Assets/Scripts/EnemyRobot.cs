using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRobot : MonoBehaviour
{
    private enum State{
        Idle,
        Move,
        Attack,
    }
    private State currentState;
    private EnemyPathfinding pathfinding;

    private void Awake()
    {
        pathfinding = GetComponent<EnemyPathfinding>();
        currentState = State.Idle;
    }
    private void Start()
    {
        StartCoroutine(IdlingRoutine());
    }
    private IEnumerator IdlingRoutine()
    {
        while (currentState == State.Idle)
        {
            Vector2 idlePosition = GetIdlePosition();
            pathfinding.MoveTo(idlePosition);
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
            pathfinding.MoveTo(other.transform.position); // Move towards the player
            pathfinding.enabled = false; // Disable pathfinding to stop movement
            StartCoroutine(AttackRoutine(other.transform));
        }
        else if (other.CompareTag("Obstacle"))
        {
            currentState = State.Attack;
            pathfinding.MoveTo(other.transform.position); // Move towards the obstacle
            //Stop enemy from moving
            pathfinding.enabled = false; // Disable pathfinding to stop movement
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
