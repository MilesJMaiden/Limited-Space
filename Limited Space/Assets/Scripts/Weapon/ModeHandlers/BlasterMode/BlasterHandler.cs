using UnityEngine;

public class BlasterHandler
{
    private AdvancedArmCannon armCannon;
    private bool isCharging;
    private float chargeTime;
    private float maxChargeTime;

    public GameObject projectilePrefab;
    public Transform firePoint; 

    // Variables for projectile behavior
    public float minProjectileSize = 1.5f;
    public float maxProjectileSize = 4.5f;
    public float minProjectileSpeed = 40.0f;
    public float maxProjectileSpeed = 60.0f;

    private GameObject currentProjectile;

    public BlasterHandler(AdvancedArmCannon armCannon)
    {
        this.armCannon = armCannon;
        maxChargeTime = 2.0f;
        firePoint = firePoint ?? armCannon.transform.Find("FirePoint");
    }

    public void StartCharging()
    {
        isCharging = true;
        chargeTime = 0;

        // Instantiate or get a projectile from the object pool
        currentProjectile = ObjectPool.Instance.GetPooledObject(firePoint.position, firePoint.rotation);
        currentProjectile.transform.localScale = Vector3.one * minProjectileSize;

        // Make projectile visible but not functional
        currentProjectile.SetActive(true);
        currentProjectile.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void FireChargedShot()
    {
        if (currentProjectile != null)
        {
            float chargePercentage = Mathf.Clamp01(chargeTime / maxChargeTime);

            // Finalize size and speed based on charge percentage
            float size = Mathf.Lerp(minProjectileSize, maxProjectileSize, chargePercentage);
            float speed = Mathf.Lerp(minProjectileSpeed, maxProjectileSpeed, chargePercentage);
            currentProjectile.transform.localScale = Vector3.one * size;
            Rigidbody rb = currentProjectile.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.velocity = firePoint.forward * speed;

            // TODO setting its damage using the lerped size.

            currentProjectile = null; // Reset
        }

        isCharging = false;
    }

    public void Update()
    {
        if (isCharging && currentProjectile != null)
        {
            chargeTime += Time.deltaTime;

            // Update projectile size as it charges
            float chargePercentage = Mathf.Clamp01(chargeTime / maxChargeTime);
            float size = Mathf.Lerp(minProjectileSize, maxProjectileSize, chargePercentage);
            currentProjectile.transform.localScale = Vector3.one * size;

            // Handle charging visual/audio effects
        }
    }
}
