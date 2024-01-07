using UnityEngine;
using UnityEngine.UI;

public class TurretEnemy : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float detectionRange = 10f;
    public float fireRate = 1f;
    private float fireRateAcceleration = 0.05f;
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

    public GameObject vfxPrefab; // Visual effect prefab
    public AudioSource audioSourcePrefab;

    public Transform playerCameraTransform;

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
        if (!playerCameraTransform)
            return;

        Vector3 directionToCamera = playerCameraTransform.position - rotatablePart.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);
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

                // Decrease fire timer at an accelerated rate
                fireTimer -= Time.deltaTime * (fireRate / fireRateIncreaseTimer) * (1 + fireRateIncreaseTimer * fireRateAcceleration);
                if (fireTimer <= 0f)
                {
                    FireProjectile();
                    ResetFireTimer();
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
            enemyProjectileScript.SetDirection((playerCameraTransform.transform.position - firePoint.position).normalized);
        }
        else
        {
            Debug.LogError("EnemyProjectile component not found on the projectile prefab.");
        }
    }
    private void ResetFireTimer()
    {
        fireTimer = 1f / (1 + fireRateIncreaseTimer * fireRateAcceleration); // Reset fire timer based on the increased rate
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthUI(); // Make sure the health is updated here
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Instantiate VFX and play audio if assigned
        if (vfxPrefab != null)
            Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        if (audioSourcePrefab != null)
            Instantiate(audioSourcePrefab, transform.position, Quaternion.identity);

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
