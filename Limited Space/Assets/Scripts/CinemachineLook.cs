using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CinemachineLook : MonoBehaviour
{
    public float lookSpeed = 1.0f;
    private PlayerControls playerControls;
    private CinemachineVirtualCamera cinemachineCamera;
    private CinemachinePOV povComponent;

    private void Awake()
    {
        playerControls = new PlayerControls();
        cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
        povComponent = cinemachineCamera.GetCinemachineComponent<CinemachinePOV>();
    }

    private void OnEnable()
    {
        playerControls.FPSPlayerActions.Enable();
        LockCursor();
    }

    private void OnDisable()
    {
        playerControls.FPSPlayerActions.Disable();
        UnlockCursor();
    }

    private void Update()
    {
        Vector2 lookInput = playerControls.FPSPlayerActions.Look.ReadValue<Vector2>() * lookSpeed;
        povComponent.m_HorizontalAxis.Value += lookInput.x * Time.deltaTime;
        povComponent.m_VerticalAxis.Value += lookInput.y * Time.deltaTime;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Optional: Call this method to toggle cursor lock state
    public void ToggleCursorLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            UnlockCursor();
        }
        else
        {
            LockCursor();
        }
    }

    // Method to adjust camera offset based on player size
    public void AdjustCameraOffset(bool isSmall, float sizeReductionFactor)
    {
        if (cinemachineCamera != null)
        {
            var transposer = cinemachineCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                // Adjust the 'Follow Offset' based on whether the player is small or not
                if (isSmall)
                {
                    // Calculate new offset for when the player is small
                    transposer.m_FollowOffset /= sizeReductionFactor;
                }
                else
                {
                    // Reset to original offset when the player returns to normal size
                    transposer.m_FollowOffset *= sizeReductionFactor;
                }
            }
        }
    }
}
