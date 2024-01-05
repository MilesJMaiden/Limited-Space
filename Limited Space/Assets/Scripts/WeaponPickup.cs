using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponPickup : MonoBehaviour
{
    public TextMeshProUGUI promptText; // World space UI text component
    public GameObject vfxPrefab; // Prefab for VFX
    public AudioClip pickupSound; // Audio clip for pickup sound

    private PlayerControls playerControls;
    private bool isPlayerNear = false;

    private void Awake()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (promptText != null)
                promptText.gameObject.SetActive(true);

            // Get the PlayerControls component from the player
            var playerMovement = other.GetComponent<AdvancedPlayerMovement>();
            if (playerMovement != null)
            {
                playerControls = playerMovement.PlayerControls;
                playerControls.FPSPlayerActions.Interact.performed += OnInteractPerformed;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (promptText != null)
                promptText.gameObject.SetActive(false);

            if (playerControls != null)
            {
                playerControls.FPSPlayerActions.Interact.performed -= OnInteractPerformed;
                playerControls = null;
            }
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (isPlayerNear)
        {
            PickupWeapon();

            // Unregister the callback
            if (playerControls != null)
            {
                playerControls.FPSPlayerActions.Interact.performed -= OnInteractPerformed;
            }
        }
    }

    private void PickupWeapon()
    {
        // Instantiate VFX and play audio
        if (vfxPrefab != null)
            Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        // Enable the weapon for the player
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var playerMovement = player.GetComponent<AdvancedPlayerMovement>();
            if (playerMovement != null)
                playerMovement.EnableWeapon();
        }

        // Enable the Weapon Mode UI
        var hudManager = FindObjectOfType<HUDManager>();
        if (hudManager != null)
            hudManager.SetWeaponModeUIActive(true);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Unregister the callback when the object is being destroyed
        if (playerControls != null)
        {
            playerControls.FPSPlayerActions.Interact.performed -= OnInteractPerformed;
        }
    }
}
