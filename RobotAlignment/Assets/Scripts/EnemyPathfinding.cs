using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stoppingDistance = 0.5f; // Enemy stops moving if target is this close.
    
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
        // If a valid direction exists, move the enemy
        if (targetDirection != Vector2.zero)
        {
            rb.MovePosition(rb.position + targetDirection * speed * Time.fixedDeltaTime);
        }
        RotateEnemy();
    }

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

    /// <summary>
    /// Call this method to update the enemy’s movement direction toward a target position.
    /// </summary>
    /// <param name="targetPosition">The world position to move toward.</param>
    public void MoveTo(Vector2 targetPosition)
    {
        // Calculate the difference vector between target and current position.
        Vector2 difference = targetPosition - rb.position;

        // If the enemy is within the stopping distance, then clear the movement.
        if (difference.magnitude < stoppingDistance)
        {
            targetDirection = Vector2.zero;
            // Debug.Log("Within stopping distance, stopping movement.");
            return;
        }

        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
        {
            difference.y = 0;
        }
        else if (Mathf.Abs(difference.y) > Mathf.Abs(difference.x))
        {
            difference.x = 0;
        }
        else
        {
            // Randomly pick x or y when they’re equal
            if (Random.value > 0.5f)
                difference.y = 0;
            else
                difference.x = 0;
        }


        // Normalize the difference to produce a direction vector.
        targetDirection = difference.normalized;
        // Debug.Log("Setting targetDirection to " + targetDirection);
    }
}


