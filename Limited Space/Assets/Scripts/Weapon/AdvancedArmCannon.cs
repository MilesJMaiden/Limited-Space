using UnityEngine;
using UnityEngine.InputSystem;

public class AdvancedArmCannon : MonoBehaviour
{
    public CinemachineLook cinemachineLook;
    private AdvancedPlayerMovement playerMovement;
    private HUDManager hudManager;

    public enum WeaponMode { MoveObjects, ModifySurfaces, Blaster }
    private WeaponMode currentMode;

    private PlayerControls playerControls;

    // Handlers for each mode
    private MoveObjectsHandler moveObjectsHandler;
    private ModifySurfacesHandler modifySurfacesHandler;
    private BlasterHandler blasterHandler;

    // Unlock flags for each weapon mode
    public bool moveObjectsUnlocked = false;
    public bool modifySurfacesUnlocked = false;
    public bool blasterUnlocked = false;

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

        // Assign HUDManager reference
        hudManager = FindObjectOfType<HUDManager>();
        if (hudManager == null)
        {
            Debug.LogError("HUDManager not found in the scene.");
        }
    }

    void OnEnable()
    {
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    void SwitchMode()
    {
        if (!AnyModesUnlocked())
        {
            // If no modes are unlocked, don't switch and return
            return;
        }

        WeaponMode originalMode = currentMode;
        do
        {
            currentMode = (WeaponMode)(((int)currentMode + 1) % System.Enum.GetNames(typeof(WeaponMode)).Length);
        } while (!IsModeUnlocked(currentMode) && currentMode != originalMode);

        // Update the HUD
        if (hudManager != null)
            hudManager.UpdateWeaponModeIndicator(currentMode, moveObjectsUnlocked, modifySurfacesUnlocked, blasterUnlocked);
    }

    bool AnyModesUnlocked()
    {
        return moveObjectsUnlocked || modifySurfacesUnlocked || blasterUnlocked;
    }

    private bool IsModeUnlocked(WeaponMode mode)
    {
        switch (mode)
        {
            case WeaponMode.MoveObjects:
                return moveObjectsUnlocked;
            case WeaponMode.ModifySurfaces:
                return modifySurfacesUnlocked;
            case WeaponMode.Blaster:
                return blasterUnlocked;
            default:
                return false;
        }
    }

    public void UnlockWeaponMode(AdvancedArmCannon.WeaponMode mode)
    {
        bool wasAnyModeUnlocked = AnyModesUnlocked();

        switch (mode)
        {
            case AdvancedArmCannon.WeaponMode.MoveObjects:
                moveObjectsUnlocked = true;
                break;
            case AdvancedArmCannon.WeaponMode.ModifySurfaces:
                modifySurfacesUnlocked = true;
                break;
            case AdvancedArmCannon.WeaponMode.Blaster:
                blasterUnlocked = true;
                break;
        }

        // If no mode was unlocked before, set the newly unlocked mode as the current mode
        if (!wasAnyModeUnlocked)
        {
            currentMode = mode;
        }

        if (hudManager != null)
        {
            hudManager.EnableWeaponModeIndicator(mode);
            hudManager.UpdateWeaponModeIndicator(currentMode, moveObjectsUnlocked, modifySurfacesUnlocked, blasterUnlocked);
        }
    }

    private void Fire()
    {
        if (!IsModeUnlocked(currentMode))
            return;

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
}
