using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponModeUnlockable : MonoBehaviour
{
    public AdvancedArmCannon.WeaponMode modeToUnlock;
    public TextMeshProUGUI promptText;
    public GameObject vfxPrefab; // For visual effect
    public AudioClip pickupSound; // Audio clip to be played

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

            // Get the AdvancedPlayerMovement component from the player
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
            // Instantiate VFX and play audio
            if (vfxPrefab != null)
                Instantiate(vfxPrefab, transform.position, Quaternion.identity);
            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            // Unlock the weapon mode
            var armCannon = FindObjectOfType<AdvancedArmCannon>();
            if (armCannon != null)
                armCannon.UnlockWeaponMode(modeToUnlock);

            // Unregister the callback immediately after interaction
            if (playerControls != null)
            {
                playerControls.FPSPlayerActions.Interact.performed -= OnInteractPerformed;
            }

            Destroy(gameObject);
        }
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
