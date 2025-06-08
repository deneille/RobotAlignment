using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float stoppingDistance = 0.3f; // Enemy stops moving if target is this close.
    
    private Rigidbody2D rb;
    private Vector2 targetDirection = Vector2.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("EnemyPathfinding: No Rigidbody2D found on " + gameObject.name);
        }
    }

    private void FixedUpdate()
    {
        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        if (targetDirection != Vector2.zero)
        {
            // Move at full speed without any block impedance
            Vector2 newPosition = rb.position + targetDirection * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
        RotateEnemy();
    }

    // private void MoveTowardsTarget()
    // {
    //     // If a valid direction exists, move the enemy
    //     if (targetDirection != Vector2.zero)
    //     {
    //         rb.MovePosition(rb.position + targetDirection * speed * Time.fixedDeltaTime);
    //     }
    //     RotateEnemy();
    // }

    private void RotateEnemy()
    {
        if (targetDirection != Vector2.zero)
        {
            // Calculate the angle (in degrees) from the horizontal (x-axis) to the movement vector.
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            // Adjust this offset based on your sprite’s default orientation:
            // If your sprite’s “forward” is up, then an offset of -90 helps align it.
            float offset = -90f;
            transform.rotation = Quaternion.Euler(0, 0, angle + offset);
            // Debug.Log("Rotating enemy. Angle (with offset): " + (angle + offset).ToString("F2") + "°.");
        }
    }

    /// Call this method to update the enemy’s movement direction toward a target position.
    public void MoveTo(Vector2 targetPosition)
    {
        Vector2 difference = targetPosition - rb.position;

        if (difference.magnitude < stoppingDistance)
        {
            targetDirection = Vector2.zero;
            return;
        }

        // Grid-based movement with better decision making
        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
        {
            // Move horizontally
            difference.y = 0;
        }
        else if (Mathf.Abs(difference.y) > Mathf.Abs(difference.x))
        {
            // Move vertically
            difference.x = 0;
        }
        else
        {
            // When equal distance, choose the direction that leads to more open space
            Vector2 horizontalDir = new Vector2(Mathf.Sign(difference.x), 0);
            Vector2 verticalDir = new Vector2(0, Mathf.Sign(difference.y));
            
            // Check which direction has fewer blockss/obstacles in the immediate path
            if (IsPathClearer(horizontalDir))
            {
                difference.y = 0;
            }
            else
            {
                difference.x = 0;
            }
        }

        targetDirection = difference.normalized;
    }

    private bool IsPathClearer(Vector2 direction)
    {
        float checkDistance = 2f;

        // Check for blocks in this direction using tag instead of layer
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, checkDistance);

        return hit.collider == null || !hit.collider.CompareTag("Block"); 
        // True if no block is hit (clearer path)
    }

    // public void MoveTo(Vector2 targetPosition)
    // {
    //     // Calculate the difference vector between target and current position.
    //     Vector2 difference = targetPosition - rb.position;

    //     // If the enemy is within the stopping distance, then clear the movement.
    //     if (difference.magnitude < stoppingDistance)
    //     {
    //         targetDirection = Vector2.zero;
    //         // Debug.Log("Within stopping distance, stopping movement.");
    //         return;
    //     }

    //     if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
    //     {
    //         difference.y = 0;
    //     }
    //     else if (Mathf.Abs(difference.y) > Mathf.Abs(difference.x))
    //     {
    //         difference.x = 0;
    //     }
    //     else
    //     {
    //         // Randomly pick x or y when they’re equal
    //         if (Random.value > 0.5f)
    //             difference.y = 0;
    //         else
    //             difference.x = 0;
    //     }


    //     // Normalize the difference to produce a direction vector.
    //     targetDirection = difference.normalized;
    //     // Debug.Log("Setting targetDirection to " + targetDirection);
    // }
}


