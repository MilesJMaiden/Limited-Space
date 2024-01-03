using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    // UI references for mode indicators
    public TextMeshProUGUI moveObjectsModeIndicator;
    public TextMeshProUGUI modifySurfacesModeIndicator;
    public TextMeshProUGUI blasterModeIndicator;

    // Method to update the active weapon mode indicator
    public void UpdateWeaponModeIndicator(AdvancedArmCannon.WeaponMode currentMode)
    {
        if (moveObjectsModeIndicator != null)
            moveObjectsModeIndicator.gameObject.SetActive(currentMode == AdvancedArmCannon.WeaponMode.MoveObjects);

        if (modifySurfacesModeIndicator != null)
            modifySurfacesModeIndicator.gameObject.SetActive(currentMode == AdvancedArmCannon.WeaponMode.ModifySurfaces);

        if (blasterModeIndicator != null)
            blasterModeIndicator.gameObject.SetActive(currentMode == AdvancedArmCannon.WeaponMode.Blaster);
    }
}
