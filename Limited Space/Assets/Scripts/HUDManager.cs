using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public GameObject weaponModeUI;

    // UI references for mode indicators
    public TextMeshProUGUI moveObjectsModeIndicator;
    public TextMeshProUGUI modifySurfacesModeIndicator;
    public TextMeshProUGUI blasterModeIndicator;

    // Define colors
    public Color defaultColor = Color.white;
    public Color activeColor = Color.yellow;

    private void Start()
    {
        // Initially disable the Weapon Mode UI
        SetWeaponModeUIActive(false);
    }

    public void UpdateWeaponModeIndicator(AdvancedArmCannon.WeaponMode currentMode, bool moveObjectsUnlocked, bool modifySurfacesUnlocked, bool blasterUnlocked)
    {
        // Update colors and activation based on current mode and unlock status
        if (moveObjectsModeIndicator != null)
        {
            moveObjectsModeIndicator.color = (currentMode == AdvancedArmCannon.WeaponMode.MoveObjects) ? activeColor : defaultColor;
            moveObjectsModeIndicator.gameObject.SetActive(moveObjectsUnlocked);
        }

        if (modifySurfacesModeIndicator != null)
        {
            modifySurfacesModeIndicator.color = (currentMode == AdvancedArmCannon.WeaponMode.ModifySurfaces) ? activeColor : defaultColor;
            modifySurfacesModeIndicator.gameObject.SetActive(modifySurfacesUnlocked);
        }

        if (blasterModeIndicator != null)
        {
            blasterModeIndicator.color = (currentMode == AdvancedArmCannon.WeaponMode.Blaster) ? activeColor : defaultColor;
            blasterModeIndicator.gameObject.SetActive(blasterUnlocked);
        }
    }

    // Method to enable the weapon mode indicator in the HUD
    public void EnableWeaponModeIndicator(AdvancedArmCannon.WeaponMode mode)
    {
        // Only enable the UI for the unlocked mode
        switch (mode)
        {
            case AdvancedArmCannon.WeaponMode.MoveObjects:
                if (moveObjectsModeIndicator != null)
                    moveObjectsModeIndicator.gameObject.SetActive(true);
                break;
            case AdvancedArmCannon.WeaponMode.ModifySurfaces:
                if (modifySurfacesModeIndicator != null)
                    modifySurfacesModeIndicator.gameObject.SetActive(true);
                break;
            case AdvancedArmCannon.WeaponMode.Blaster:
                if (blasterModeIndicator != null)
                    blasterModeIndicator.gameObject.SetActive(true);
                break;
        }
    }

    public void SetWeaponModeUIActive(bool isActive)
    {
        weaponModeUI.SetActive(isActive);
    }
}
