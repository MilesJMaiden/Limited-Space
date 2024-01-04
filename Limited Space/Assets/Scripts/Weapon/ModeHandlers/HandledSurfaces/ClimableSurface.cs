using UnityEngine;

public class ClimbableSurface : MonoBehaviour
{
    private BoxCollider triggerCollider;

    public void SetTriggerCollider(BoxCollider collider)
    {
        triggerCollider = collider;
    }

    public void RemoveTriggerCollider()
    {
        if (triggerCollider != null)
        {
            Destroy(triggerCollider);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerMovement = other.GetComponent<AdvancedPlayerMovement>();
            if (playerMovement != null)
            {
                Debug.Log("Climbing enabled");
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
                Debug.Log("Climbing disabled");
                playerMovement.EnableClimbing(false);
            }
        }
    }
}
