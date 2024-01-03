using UnityEngine;

public class TrampolineSurface : MonoBehaviour
{
    public float bounceMultiplier = 2.0f; // Multiplier for the jump force

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var playerMovement = collision.gameObject.GetComponent<AdvancedPlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ApplyBounce(bounceMultiplier);
            }
        }
    }
}
