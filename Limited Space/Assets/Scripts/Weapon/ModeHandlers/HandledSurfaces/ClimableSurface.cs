using System.Collections;
using UnityEngine;

public class ClimbableSurface : MonoBehaviour
{
    private BoxCollider triggerCollider;
    private bool isPlayerTransitioning = false;

    public void SetTriggerCollider(BoxCollider collider)
    {
        triggerCollider = collider;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPlayerTransitioning)
        {
            Debug.Log("Climbing enabled");
            other.GetComponent<AdvancedPlayerMovement>().EnableClimbing(true);
            isPlayerTransitioning = true;
            StartCoroutine(ResetTransitionState());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Climbing disabled");
            other.GetComponent<AdvancedPlayerMovement>().EnableClimbing(false);
        }
    }

    private IEnumerator ResetTransitionState()
    {
        yield return new WaitForSeconds(1f); // Adjust time as needed
        isPlayerTransitioning = false;
    }

    public void RemoveTriggerCollider()
    {
        if (triggerCollider != null)
        {
            Destroy(triggerCollider);
        }
    }
}
