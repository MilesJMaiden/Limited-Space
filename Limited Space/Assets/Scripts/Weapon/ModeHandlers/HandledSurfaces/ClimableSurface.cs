using UnityEngine;

public class ClimbableSurface : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
            var playerMovement = other.GetComponent<AdvancedPlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.EnableClimbing(false);
            }
        }
    }
}
