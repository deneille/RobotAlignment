using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 1f; // Cooldown time between attacks
    [SerializeField] private Transform attackPoint; // Point where the attack is performed
    [SerializeField] private GameObject[] rays; // Array of rays for attack detection
    private float lastAttackTime; // Time of the last attack
    private Animator animator; // Reference to the Animator component
    private PlayerController playerController; // Reference to the PlayerController component
    public float attackRange = 1f; // Range of the attack
    private float coolDownTimer = Mathf.Infinity; // Timer for cooldown


    void Awake() {
        // Get the Animator component attached to the player
        animator = GetComponent<Animator>();

        // Get the PlayerController component attached to the player
        playerController = GetComponent<PlayerController>();
    }    

    void Update() {
        // Check if the player is moving and not attacking
        if (Input.GetButtonDown("Fire1") && playerController.canAttack() && coolDownTimer > attackCooldown) {
            Attack();
            coolDownTimer += Time.deltaTime;
            // Reset the cooldown timer
        }
    }

    void Attack() {
        Debug.Log("Attack triggered");
        // Trigger the attack animation
        animator.SetTrigger("attack");
        
        // Debug the current rotation
        Debug.Log("Current rotation: " + transform.rotation.eulerAngles.z);
        
        // Get the direction based on the player's current rotation
        Vector2 direction = transform.up * -1f; // Invert the direction to match the player's rotation
        // Debug the direction
        Debug.Log("Attack direction: " + direction);
        Debug.Log("Attack point position: " + attackPoint.position);

        
        rays[0].transform.position = attackPoint.position;
        rays[0].GetComponent<Projectile>().SetDirection(direction);
        
        coolDownTimer = 0f; // Reset the cooldown timer
    }
    

    IEnumerator PerformAttackAfterDelay(float delay) {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        // Perform the attack logic here (e.g., check for enemies in range)
    }
    void OnDrawGizmos()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(attackPoint.position, transform.up * 1.5f); // Adjust direction as needed
        }
    }
}
