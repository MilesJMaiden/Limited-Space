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
        currentHealth = maxHealth;
        HUDManager.Instance.UpdatePlayerHealthDisplay(currentHealth, maxHealth); // Initialize the health display
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        HUDManager.Instance.UpdatePlayerHealthDisplay(currentHealth, maxHealth);


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
        HUDManager.Instance.UpdatePlayerHealthDisplay(currentHealth, maxHealth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnemyProjectile"))
        {
            EnemyProjectile projectile = collision.gameObject.GetComponent<EnemyProjectile>();
            if (projectile != null)
            {
                TakeDamage(projectile.damage);
            }
        }
    }
}
