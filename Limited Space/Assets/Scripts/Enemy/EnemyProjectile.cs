using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;

    private Vector3 direction;
    private bool hasHit = false;

    void Update()
    {
        if (!hasHit)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Assuming the player has a method called 'TakeDamage'
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }

        // Destroy the projectile when it hits any surface or the player
        hasHit = true;
        Destroy(gameObject);
    }
}
