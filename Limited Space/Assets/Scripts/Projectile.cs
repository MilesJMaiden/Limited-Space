using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject impactEffectPrefab; // Assign in the Inspector
    public AudioClip impactSound; // Assign in the Inspector
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
        // Handle the collision
        HandleCollision();

        // Disable or return the projectile to the object pool
        DestroyProjectile();
    }

    private void HandleCollision()
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

        // Additional collision handling, like dealing damage
    }

    private void DestroyProjectile()
    {
        // If using object pooling
        ObjectPool.Instance.ReturnObjectToPool(gameObject);

        // If not using object pooling
        // Destroy(gameObject);
    }
}
