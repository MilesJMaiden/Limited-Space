using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    public Slider playerHealthSlider;

    public GameObject weaponModeUI;

    // UI references for mode indicators
    public TextMeshProUGUI moveObjectsModeIndicator;
    public TextMeshProUGUI modifySurfacesModeIndicator;
    public TextMeshProUGUI blasterModeIndicator;

    public CanvasGroup startPanelCanvasGroup;

    // Define colors
    public Color defaultColor = Color.white;
    public Color activeColor = Color.yellow;

    public float fadeDuration = 2f;

    public GameObject redKeycardUI;
    public GameObject greenKeycardUI;
    public GameObject blueKeycardUI;
    public GameObject goldKeycardUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        // Initially disable the Weapon Mode UI
        SetWeaponModeUIActive(false);
        EnableAndFadeOutPanel(startPanelCanvasGroup, fadeDuration);
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

    public void UpdateKeycardUI(Keycard.KeycardType keycardType, bool isActive)
    {
        switch (keycardType)
        {
            case Keycard.KeycardType.Red:
                redKeycardUI.SetActive(isActive);
                break;
            case Keycard.KeycardType.Green:
                greenKeycardUI.SetActive(isActive);
                break;
            case Keycard.KeycardType.Blue:
                blueKeycardUI.SetActive(isActive);
                break;
            case Keycard.KeycardType.Gold:
                goldKeycardUI.SetActive(isActive);
                break;
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

    private void EnableAndFadeOutPanel(CanvasGroup panel, float duration)
    {
        panel.gameObject.SetActive(true);
        panel.alpha = 1;
        StartCoroutine(FadeOutPanel(panel, duration));
    }
    private IEnumerator FadeOutPanel(CanvasGroup panel, float duration)
    {
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            panel.alpha = Mathf.Lerp(1, 0, currentTime / duration);
            yield return null;
        }

        panel.gameObject.SetActive(false);
    }

    // Method to update the player's health bar
    public void UpdatePlayerHealthDisplay(float currentHealth, float maxHealth)
    {
        if (playerHealthSlider != null)
        {
            playerHealthSlider.maxValue = maxHealth;
            playerHealthSlider.value = currentHealth;
        }
    }
}