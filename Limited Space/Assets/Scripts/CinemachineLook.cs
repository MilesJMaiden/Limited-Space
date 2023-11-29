using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CinemachineLook : MonoBehaviour
{
    public float lookSpeed = 1.0f;
    private PlayerControls playerControls;
    private CinemachineVirtualCamera cinemachineCamera;
    private CinemachinePOV povComponent;

    private Vector3 originalFollowOffset;

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

 
}
