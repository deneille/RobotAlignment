using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private bool hit;
    private Vector2 moveDirection;

    private BoxCollider2D boxCollider;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>(); // Get the Animator component attached to the projectile
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the projectile object.");
        }
        boxCollider = GetComponent<BoxCollider2D>(); // Get the BoxCollider2D component attached to the projectile
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider2D component not found on the projectile object.");
        }
    }

    void Update()
    {
        if (!hit)
        {
            // Move the projectile in the given direction
            transform.Translate(moveDirection * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        hit = true; // Set hit to true when the projectile collides with something
        boxCollider.enabled = false; // Disable the collider to prevent further collisions
        animator.SetTrigger("explode"); // Trigger the explosion animation
    }

    public void SetDirection(Vector2 direction) 
    {
        // the direction of the projectile should be the same as the direction the player is rotated towards
        // Set the moveDirection to the provided direction
        // Normalize the direction vector to ensure consistent speed
        Debug.Log("Projectile activated in direction: " + direction);
        moveDirection = direction.normalized; // Normalize the direction vector to ensure consistent speed
        gameObject.SetActive(true); // Activate the projectile
        //need to set the direction
        hit = false; // Reset hit to false
        boxCollider.enabled = true; // Enable the collider for the projectile  


        // Set rotation based on direction
        float rotationZ = 0;
        if (direction == Vector2.down)
            rotationZ = 0; // Up
        else if (direction == Vector2.up)
            rotationZ = 180; // Down
        else if (direction == Vector2.right)
            rotationZ = 90; // Right
        else if (direction == Vector2.left)
            rotationZ = 270; // Left
            
        transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }

    private void DeactivateProjectile()
    {
        gameObject.SetActive(false);
    }
}

