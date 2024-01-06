using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public delegate void HealthChanged(float currentHealth, float maxHealth);
    public event HealthChanged OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth; // Initialize health
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // Trigger the health changed event
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Implement death logic here
        Debug.Log("Player Died");

        // For example, reload the current scene or transition to a game over screen
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Trigger the health changed event
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
