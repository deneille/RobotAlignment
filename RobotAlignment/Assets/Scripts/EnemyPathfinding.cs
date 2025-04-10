using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    private Rigidbody2D rb;
    private Vector2 targetDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        MoveTowardsTarget();
    }
    private void MoveTowardsTarget()
    {

        // Want to avoid moving diagonally
        if (Mathf.Abs(targetDirection.x) > Mathf.Abs(targetDirection.y))
        {
            targetDirection.y = 0;
        }
        else
        {
            targetDirection.x = 0;
        }
        if (targetDirection != Vector2.zero)
        {
            rb.MovePosition(rb.position + targetDirection * speed * Time.fixedDeltaTime);
        }
    }
    public void MoveTo(Vector2 targetPosition)
    {
        targetDirection = targetPosition;
    }
}
