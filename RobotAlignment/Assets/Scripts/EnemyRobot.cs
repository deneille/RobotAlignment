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
}
