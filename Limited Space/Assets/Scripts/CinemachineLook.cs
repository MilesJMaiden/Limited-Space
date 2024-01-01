using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CinemachineLook : MonoBehaviour
{
    public float mouseSensitivity = 100.0f;
    public float verticalRotationLimit = 80f; // Limit for vertical rotation

    private PlayerControls playerControls;
    private CinemachineVirtualCamera cinemachineCamera;
    private CinemachinePOV povComponent;

    private float mouseX, mouseY;
    private float verticalRotation = 0f; // Keep track of the vertical rotation separately

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
        mouseX = playerControls.FPSPlayerActions.Look.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        mouseY = playerControls.FPSPlayerActions.Look.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        // Invert the vertical axis
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);

        // Apply the rotations
        povComponent.m_VerticalAxis.Value = verticalRotation;
        povComponent.m_HorizontalAxis.Value += mouseX;
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

    public void SetCameraMovementEnabled(bool isEnabled)
    {
        if (povComponent != null)
        {
            povComponent.enabled = isEnabled;
        }
    }
}