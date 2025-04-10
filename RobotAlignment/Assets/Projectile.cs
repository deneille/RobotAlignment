using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private bool hit;
    private Vector2 moveDirection;

    private BoxCollider2D boxCollider;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
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
        hit = true;
        boxCollider.enabled = false;
        animator.SetTrigger("explode");
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        // Set rotation based on direction
        float rotationZ = 0;
        if (direction == Vector2.up)
            rotationZ = 0; // Up
        else if (direction == Vector2.down)
            rotationZ = 180; // Down
        else if (direction == Vector2.right)
            rotationZ = -90; // Right
        else if (direction == Vector2.left)
            rotationZ = 90; // Left
            
        transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }

    private void DeactivateProjectile()
    {
        gameObject.SetActive(false);
        hit = false;
        boxCollider.enabled = true;
    }

    public void DestroyProjectile()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Robot"))
        {
            Destroy(gameObject);
        }
    }
}

