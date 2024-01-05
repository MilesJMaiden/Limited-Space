using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Assuming you have a method in your player script to enable the weapon
            other.GetComponent<AdvancedPlayerMovement>().EnableWeapon();

            // Enable the Weapon Mode UI
            FindObjectOfType<HUDManager>().SetWeaponModeUIActive(true);

            // Optionally, destroy or deactivate the pickup object
            Destroy(gameObject);
        }
    }
}
