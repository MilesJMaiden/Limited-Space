using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject impactEffectPrefab; // Assign in the Inspector
    public AudioClip impactSound; // Assign in the Inspector
    public float damage = 10f; // Damage value of the projectile
    public float lifetime = 5.0f; // Lifetime of the projectile in seconds

    private float timer; // Internal timer

    private void OnEnable()
    {
        // Reset the timer each time the projectile is enabled
        timer = lifetime;
    }

    private void Update()
    {
        // Count down the timer
        timer -= Time.deltaTime;

        // Check if the timer has reached zero
        if (timer <= 0)
        {
            DestroyProjectile();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        TurretEnemy turret = collision.collider.GetComponent<TurretEnemy>();
        if (turret != null)
        {
            turret.TakeDamage(damage);
        }
        HandleCollision(collision); // Pass the collision parameter
        DestroyProjectile();
    }

    private void HandleCollision(Collision collision)
    {
        // Instantiate impact effect
        if (impactEffectPrefab != null)
        {
            Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
        }

        // Play impact sound
        if (impactSound != null)
        {
            AudioSource.PlayClipAtPoint(impactSound, transform.position);
        }

        // Check if the hit object is a turret and apply damage
        TurretEnemy turret = collision.collider.GetComponent<TurretEnemy>();
        if (turret != null)
        {
            turret.TakeDamage(damage);
        }
    }

    private void DestroyProjectile()
    {
        // If using object pooling
        ObjectPool.Instance.ReturnObjectToPool(gameObject);

        // If not using object pooling
        // Destroy(gameObject);
    }
}
