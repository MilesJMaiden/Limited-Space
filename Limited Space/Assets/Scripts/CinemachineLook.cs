using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CinemachineLook : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineCamera;
    private CinemachinePOV povComponent;

    private PlayerControls playerControls;
    private bool isCameraMovementEnabled = true;

    public float mouseSensitivity = 100.0f;
    public float verticalRotationLimit = 80f;

    private float mouseX, mouseY;
    private float verticalRotation = 0f; 

    private float savedVerticalRotation = 0f;
    private float savedHorizontalRotation = 0f;

    private Quaternion savedRotation;

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
        if (isCameraMovementEnabled)
        {
            UpdateCameraRotation();
        }
    }

    private void UpdateCameraRotation()
    {
        mouseX = playerControls.FPSPlayerActions.Look.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        mouseY = playerControls.FPSPlayerActions.Look.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);

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
        isCameraMovementEnabled = isEnabled;

        if (!isEnabled)
        {
            // Save current rotation
            SaveCurrentRotation();
            // Disable the input axis to stop updating the camera rotation
            povComponent.m_HorizontalAxis.m_MaxSpeed = 0;
            povComponent.m_VerticalAxis.m_MaxSpeed = 0;
        }
        else
        {
            // Re-enable the input axis
            povComponent.m_HorizontalAxis.m_MaxSpeed = mouseSensitivity;
            povComponent.m_VerticalAxis.m_MaxSpeed = mouseSensitivity;
            // Restore the saved rotation
            RestoreSavedRotation();
        }
    }

    public void SetCameraUpdateEnabled(bool isEnabled)
    {
        if (cinemachineCamera != null)
        {
            if (isEnabled)
            {
                cinemachineCamera.enabled = true;
            }
            else
            {
                cinemachineCamera.enabled = false;
            }
        }
    }

    public void SaveCurrentRotation()
    {
        savedVerticalRotation = povComponent.m_VerticalAxis.Value;
        savedHorizontalRotation = povComponent.m_HorizontalAxis.Value;
    }

    public void RestoreSavedRotation()
    {
        povComponent.m_VerticalAxis.Value = savedVerticalRotation;
        povComponent.m_HorizontalAxis.Value = savedHorizontalRotation;
    }
}