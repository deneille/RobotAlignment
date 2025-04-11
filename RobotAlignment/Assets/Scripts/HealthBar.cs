using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import the UI namespace to use Image

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth; // Reference to the PlayerHealth script
    [SerializeField] private Image totalHealthBar; // Reference to the total health bar image
    [SerializeField] private Image currentHealthBar; // Reference to the current health bar image
    // Start is called before the first frame update
    void Start()
    {
        totalHealthBar.fillAmount = playerHealth.currentHealth / 10; // Set the total health bar to full
        currentHealthBar.fillAmount = playerHealth.currentHealth / 10; // Set the current health bar to full
    }

    // Update is called once per frame
    void Update()
    {
        currentHealthBar.fillAmount = playerHealth.currentHealth / 10; // Update the current health bar based on the player's current health
    }
}
