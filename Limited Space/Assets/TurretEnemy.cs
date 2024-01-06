using UnityEngine;
using UnityEngine.UI;

public class TurretEnemy : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float detectionRange = 10f;
    public float fireRate = 1f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Slider healthSlider;

    private GameObject player;
    private float currentHealth = 100f;
    private float maxHealth = 100f;
    private float fireRateIncreaseTimer = 0f;
    private float fireTimer = 0f;
    private bool isPlayerDetected = false;

    public Transform rotatablePart;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    private void Update()
    {
        if (isPlayerDetected)
        {
            AimTowardsPlayer();
            CheckLineOfSight();
        }

        UpdateHealthUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerDetected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerDetected = false;
            fireRateIncreaseTimer = 0f;
        }
    }

    private void AimTowardsPlayer()
    {
        Vector3 directionToPlayer = player.transform.position - rotatablePart.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        Vector3 rotation = Quaternion.Lerp(rotatablePart.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        rotatablePart.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    private void CheckLineOfSight()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
        {
            if (hit.collider.gameObject == player)
            {
                fireRateIncreaseTimer += Time.deltaTime;
                if (fireRateIncreaseTimer > 3f)
                {
                    fireRateIncreaseTimer = 3f; // Cap the timer at 3 seconds for max fire rate
                }

                fireTimer -= Time.deltaTime * (fireRate / fireRateIncreaseTimer);
                if (fireTimer <= 0f)
                {
                    FireProjectile();
                    fireTimer = 1f;
                }
            }
            else
            {
                fireRateIncreaseTimer = 0f;
            }
        }
    }

private void FireProjectile()
{
    // Instantiate projectile and set its direction
    GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    EnemyProjectile enemyProjectileScript = projectile.GetComponent<EnemyProjectile>(); // Corrected to EnemyProjectile
    if (enemyProjectileScript != null)
    {
        enemyProjectileScript.SetDirection((player.transform.position - firePoint.position).normalized);
    }
    else
    {
        Debug.LogError("EnemyProjectile component not found on the projectile prefab.");
    }
}

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle turret destruction
        Destroy(gameObject);
    }

    private void UpdateHealthUI()
    {
        healthSlider.value = currentHealth;
    }

    void OnDrawGizmos()
    {
        if (rotatablePart != null)
        {
            Gizmos.color = Color.red;
            Vector3 forward = rotatablePart.forward;

            // Draw detection range line
            Gizmos.DrawLine(rotatablePart.position, rotatablePart.position + forward * detectionRange);

            // Visualize arc (approximate)
            for (int i = -75; i <= 75; i += 15) // Adjust step size for more/less lines
            {
                Vector3 direction = Quaternion.Euler(0, i, 0) * forward * detectionRange;
                Gizmos.DrawLine(rotatablePart.position, rotatablePart.position + direction);
            }
        }
    }
}
