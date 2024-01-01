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

    void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.FPSPlayerActions.SwitchMode.performed += _ => SwitchMode();
        playerControls.FPSPlayerActions.Fire.performed += _ => Fire();
        playerControls.FPSPlayerActions.Fire.canceled += _ => ReleaseFire();

        // Get the CinemachineLook component
        CinemachineLook cinemachineLook = GetComponent<CinemachineLook>();

        // Initialize mode handlers
        moveObjectsHandler = new MoveObjectsHandler(this, moveObjectSpeed, rotateSpeed, cinemachineLook);
        playerControls.FPSPlayerActions.RotateObjectModeToggle.performed += _ => moveObjectsHandler.ToggleObjectRotationMode();
        modifySurfacesHandler = new ModifySurfacesHandler(this, maxModifiedSurfaces);
        blasterHandler = new BlasterHandler(this);

        // Input bindings for MoveObject and RotateObject
        playerControls.FPSPlayerActions.AdjustObjectDistance.performed += ctx => moveObjectsHandler.MoveObject(ctx.ReadValue<Vector2>());

        // Handling rotation - checking if right mouse button is held and then using mouse delta for rotation
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
        switch (currentMode)
        {
            case WeaponMode.MoveObjects:
                moveObjectsHandler.Update();
                break;
            case WeaponMode.ModifySurfaces:
                modifySurfacesHandler.Update();
                break;
            case WeaponMode.Blaster:
                blasterHandler.Update();
                break;
        }
    }

    // Pass on necessary references or data to handlers as needed
}
