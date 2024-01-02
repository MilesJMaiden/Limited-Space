using UnityEngine;

public class BlasterHandler
{
    private AdvancedArmCannon armCannon;
    private bool isCharging;
    private float chargeTime;
    private float maxChargeTime;

    public GameObject projectilePrefab; // Assign in the Inspector
    public Transform firePoint; // Assign the FirePoint Transform in the Inspector

    // Variables for projectile behavior
    public float minProjectileSize = 0.5f;
    public float maxProjectileSize = 2.0f;
    public float projectileSpeed = 10.0f;

    public BlasterHandler(AdvancedArmCannon armCannon)
    {
        this.armCannon = armCannon;
        this.maxChargeTime = 2.0f; // Example value
        // Initialize firePoint, if not set in the Inspector
        firePoint = firePoint ?? armCannon.transform.Find("FirePoint");
    }

    public void StartCharging()
    {
        isCharging = true;
        chargeTime = 0;
    }

    public void FireChargedShot()
    {
        float chargePercentage = Mathf.Clamp01(chargeTime / maxChargeTime);

        // Get a projectile from the object pool or instantiate a new one
        GameObject projectile = ObjectPool.Instance.GetPooledObject(firePoint.position, firePoint.rotation);

        // Adjust size and speed based on charge percentage
        float size = Mathf.Lerp(minProjectileSize, maxProjectileSize, chargePercentage);
        projectile.transform.localScale = Vector3.one * size;
        projectile.GetComponent<Rigidbody>().velocity = firePoint.forward * projectileSpeed;

        // Additional setup like enabling the projectile, setting its damage, etc.

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