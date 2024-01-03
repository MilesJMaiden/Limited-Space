using UnityEngine;

public class ClimbableSurface : MonoBehaviour
{
    // Additional properties can be added if needed

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Enable climbing for the player
            var playerMovement = other.GetComponent<AdvancedPlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.EnableClimbing(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Disable climbing for the player
            var playerMovement = other.GetComponent<AdvancedPlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.EnableClimbing(false);
            }
        }
    }
}
