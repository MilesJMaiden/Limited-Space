using UnityEngine;

public class BlasterHandler
{
    private AdvancedArmCannon armCannon;
    private bool isCharging;
    private float chargeTime;
    private float maxChargeTime; // Set this to the maximum charge time

    public BlasterHandler(AdvancedArmCannon armCannon, float maxChargeTime)
    {
        this.armCannon = armCannon;
        this.maxChargeTime = maxChargeTime;
    }

    public void StartCharging()
    {
        isCharging = true;
        chargeTime = 0;
    }

    public void FireChargedShot()
    {
        float chargePercentage = Mathf.Clamp01(chargeTime / maxChargeTime);
        // Create and fire projectile with size and power based on chargePercentage
        isCharging = false;
    }

    public void Update()
    {
        if (isCharging)
        {
            chargeTime += Time.deltaTime;
            // Handle charging visual/audio effects
        }
    }
}
