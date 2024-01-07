using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Keycard : MonoBehaviour
{
    public enum KeycardType { Red, Green, Blue, Gold }
    public KeycardType keycardType;
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
            CollectKeycard();

            // Unregister the callback
            if (playerControls != null)
            {
                playerControls.FPSPlayerActions.Interact.performed -= OnInteractPerformed;
            }
        }
    }

    private void CollectKeycard()
    {
        // Instantiate VFX and play audio
        if (vfxPrefab != null)
            Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        // Add the keycard to the inventory
        InventorySystem.Instance.AddKeycard(keycardType);
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
