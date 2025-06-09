using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Image healthBarFill; // Drag your UI Image (set to Filled) here in the Inspector
    public Image totalHealthBar; // Drag your total health bar Image here in the Inspector

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (healthBarFill == null)
        {
            Debug.LogError("Health Bar Fill Image is not assigned!");
            FindHealthBarFill();
        }
    }

    private void FindHealthBarFill()
    {
        // Try to find the health bar fill image in the scene if not assigned
        GameObject healthBarObject = GameObject.Find("HealthBar");
        if (healthBarObject != null)
        {
            healthBarFill = healthBarObject.GetComponentInChildren<Image>();
        }
        GameObject totalHealthBarObject = GameObject.Find("TotalHealthBar");
        if (totalHealthBarObject != null)
        {
            totalHealthBar = totalHealthBarObject.GetComponentInChildren<Image>();
        }
        else
        {
            Debug.LogError("Total Health Bar Image is not assigned and could not be found in the scene!");
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
        {
            Die();
        }
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthUI();
    }

    public void Heal(int amount)
    {
        if (currentHealth <= 0)
        {
            Debug.LogWarning("Cannot heal, player is dead.");
            return;
        }
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        // Update the fill amount based on health ratio
        if (healthBarFill != null)
        {
            float healthRatio = (float)currentHealth / maxHealth;
            healthBarFill.fillAmount = healthRatio;
        }
        if (totalHealthBar != null)
        {
            totalHealthBar.fillAmount = 1f;
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        // Implement death logic here, such as playing an animation or reloading the scene
        // For now, we will just reset health
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowLoseScreen("Your health reached zero! The misaligned robots have overwhelmed you!");
        }
        else
        {
            Debug.LogWarning("GameManager instance not found. Cannot restart game.");
        }
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public float GetHealthRatio()
    {
        return (float)currentHealth / maxHealth;
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        Debug.Log("Player health has been reset.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Take damage from the enemy
            TakeDamage(33); // Example damage value, adjust as needed
        }
    }
}

