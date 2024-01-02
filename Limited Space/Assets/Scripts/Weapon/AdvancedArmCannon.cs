using UnityEngine;
using UnityEngine.InputSystem;

public class AdvancedArmCannon : MonoBehaviour
{
    private enum WeaponMode { MoveObjects, ModifySurfaces, Blaster }
    private WeaponMode currentMode;

    private PlayerControls playerControls;

    // Handlers for each mode
    private MoveObjectsHandler moveObjectsHandler;
    private ModifySurfacesHandler modifySurfacesHandler;
    private BlasterHandler blasterHandler;

    // Configurable parameters
    public float moveObjectSpeed = 5f; // Speed at which objects are moved
    public float rotateSpeed = 90f; // Degrees per second for rotation
    public int maxModifiedSurfaces = 3; // Max number of surfaces that can be modified at once

    public CinemachineLook cinemachineLook; // Add this line

    private AdvancedPlayerMovement playerMovement;

    void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.FPSPlayerActions.SwitchMode.performed += _ => SwitchMode();
        playerControls.FPSPlayerActions.Fire.performed += _ => Fire();
        playerControls.FPSPlayerActions.Fire.canceled += _ => ReleaseFire();

        GameObject fpsVirtualCamera = GameObject.Find("FPSVirtualCamera");
        if (fpsVirtualCamera == null)
        {
            Debug.LogError("FPS Virtual Camera not found in scene.");
            return;
        }

        CinemachineLook cinemachineLook = fpsVirtualCamera.GetComponent<CinemachineLook>();
        if (cinemachineLook == null)
        {
            Debug.LogError("CinemachineLook component not found on FPS Virtual Camera.");
            return;
        }

        // Find the AdvancedPlayerMovement component in the parent GameObject
        playerMovement = GetComponentInParent<AdvancedPlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("AdvancedPlayerMovement component not found in parent GameObject.");
            return;
        }

        // Initialize mode handlers with the playerMovement reference
        moveObjectsHandler = new MoveObjectsHandler(this, moveObjectSpeed, rotateSpeed, cinemachineLook, playerMovement);
        modifySurfacesHandler = new ModifySurfacesHandler(this, maxModifiedSurfaces);
        blasterHandler = new BlasterHandler(this);

        // Input bindings for MoveObject and RotateObject
        playerControls.FPSPlayerActions.AdjustObjectDistance.performed += ctx => moveObjectsHandler.MoveObject(ctx.ReadValue<Vector2>());
        playerControls.FPSPlayerActions.RotateObject.performed += ctx =>
        {
            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 rotationDelta = ctx.ReadValue<Vector2>();
                moveObjectsHandler.RotateObject(rotationDelta);
            }
        };
    }

    void OnEnable()
    {
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    private void SwitchMode()
    {
        currentMode = (WeaponMode)(((int)currentMode + 1) % System.Enum.GetNames(typeof(WeaponMode)).Length);
        // Add logic to update UI or visual indicators for the current mode
    }

    private void Fire()
    {
        switch (currentMode)
        {
            case WeaponMode.MoveObjects:
                moveObjectsHandler.TryGrabObject();
                break;
            case WeaponMode.ModifySurfaces:
                modifySurfacesHandler.ModifySurface();
                break;
            case WeaponMode.Blaster:
                blasterHandler.StartCharging();
                break;
        }
    }

    private void ReleaseFire()
    {
        if (currentMode == WeaponMode.Blaster)
        {
            blasterHandler.FireChargedShot();
        }
    }

    void Update()
    {
        // Update logic specific to each mode
        switch (currentMode)
        {
            case WeaponMode.MoveObjects:
                if (moveObjectsHandler != null)
                {
                    moveObjectsHandler.Update();
                }
                break;
            case WeaponMode.ModifySurfaces:
                if (modifySurfacesHandler != null)
                {
                    modifySurfacesHandler.Update();
                }
                break;
            case WeaponMode.Blaster:
                if (blasterHandler != null)
                {
                    blasterHandler.Update();
                }
                break;
        }
    }

    // Pass on necessary references or data to handlers as needed
}