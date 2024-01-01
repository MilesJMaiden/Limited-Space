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

    // Add these public properties
    public float moveObjectSpeed = 5f;
    public float rotateSpeed = 90f; // Degrees per second

    void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.ArmCannon.SwitchMode.performed += _ => SwitchMode();
        playerControls.ArmCannon.Fire.performed += _ => Fire();
        playerControls.ArmCannon.Fire.canceled += _ => ReleaseFire();
        playerControls.ArmCannon.MoveObject.performed += ctx => moveObjectsHandler.MoveObject(ctx.ReadValue<Vector2>());
        playerControls.ArmCannon.RotateObject.performed += ctx => moveObjectsHandler.RotateObject(ctx.ReadValue<float>());

        // Initialize mode handlers
        moveObjectsHandler = new MoveObjectsHandler(this);
        modifySurfacesHandler = new ModifySurfacesHandler(this);
        blasterHandler = new BlasterHandler(this);
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
        // Additional logic for mode switching, like updating UI
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
