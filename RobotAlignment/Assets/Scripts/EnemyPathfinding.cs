using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    private Rigidbody2D rb;
    private Vector2 targetDirection;
    private RaycastHit2D hit;
    [SerializeField] private LayerMask layerMask; // Layer mask for the raycast

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

        // Want to avoid moving diagonally but allow for rotation
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
        RotateEnemy(); // Rotate the enemy based on the target direction
    }

    private void RotateEnemy()
    {
        if (targetDirection.x > 0) // Right
        {
            transform.rotation = Quaternion.Euler(0, 0, 90); // Facing right
        }
        else if (targetDirection.x < 0) // Left
        {
            transform.rotation = Quaternion.Euler(0, 0, -90); // Facing left
        }
        else if (targetDirection.y > 0) // Up
        {
            transform.rotation = Quaternion.Euler(0, 0, 180); // Facing up
        }
        else if (targetDirection.y < 0) // Down
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // Facing down
        }
    }
    public void MoveTo(Vector2 targetPosition)
    {
        targetDirection = targetPosition;
        if(Mathf.Abs(targetPosition.x) > Mathf.Abs(targetPosition.y))
        {
            targetPosition.y = 0;
        }
        else
        {
            targetPosition.x = 0;
        }
    }
 
}
