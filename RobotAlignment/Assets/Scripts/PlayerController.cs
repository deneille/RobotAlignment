using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float speed = 2f; // Speed of the player

    private bool isMoving; // Flag to check if the player is moving
    private Vector2 movement; // Variable to store the movement input
    private Animator animator; // Reference to the Animator component
    public LayerMask layerMask; // Layer mask for raycasting
    private RaycastHit2D hit; // Variable to store the raycast hit information
    // Start is called before the first frame update

    void Awake(){
        // Get the Animator component attached to the player
        animator = GetComponent<Animator>();
        if(animator == null){
            Debug.LogError("Animator component not found on the player object.");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            // Get the input from the player
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // Normalize the movement vector to ensure consistent speed in all directions
            movement.Normalize();

            if(movement.x != 0){
                movement.y = 0;
            }

            // Check if the player is moving
            if (movement != Vector2.zero){

                animator.SetFloat("moveX", movement.x);
                animator.SetFloat("moveY", movement.y);

                var targetPos = transform.position;
                targetPos.x += movement.x;
                targetPos.y += movement.y;
                if(isWalkable(targetPos)){
                    StartCoroutine(Move(targetPos));
                }
            }
        }
        animator.SetBool("isMoving", isMoving);
    }
    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true; // Set the moving flag to true
        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            // Move the player towards the target position
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }
        transform.position = targetPos; // Set the player position to the target position
        isMoving = false; // Set the moving flag to false   
    }

    private bool isWalkable(Vector3 targetPos){
        hit = Physics2D.Raycast(transform.position, movement, 1f, layerMask);
        if(hit.collider != null){
            return false;
        }
        return true;
    }
    public bool canAttack(){
        return !isMoving;
    }

}
