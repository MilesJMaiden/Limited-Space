using UnityEngine;

public class Door : MonoBehaviour
{
    public Keycard.KeycardType requiredKeycard;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && InventorySystem.Instance.HasKeycard(requiredKeycard))
        {
            // Logic to open the door
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        // Door opening animation or logic
    }
}
