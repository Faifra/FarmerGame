using System.Collections;
using UnityEngine;

/// <summary>
/// Handles projectile weapons, liquid streams, ammunition, and reloading.
/// </summary>
public class RangedWeaponScript : WeaponScript
{
    public enum FireMode
    {
        Projectile,
        LiquidStream,
        RestorativeGas
    }

    [Header("Firing Mode")]
    [SerializeField] private FireMode fireMode = FireMode.Projectile;

    [Header("Restorative Gas")]
    [SerializeField] private RestorativeGasCloudScript gasCloudPrefab;
    [SerializeField] private float gasFireInterval = 0.1f;
    [SerializeField] private RestorativeGasUpgradeScript restorativeGasUpgrades;

    [Header("Crosshair Aiming")]
    [SerializeField] private Camera aimCamera;
    [SerializeField] private float aimDistance = 1000f;
    [SerializeField] private LayerMask aimLayers = ~0;

    [Header("Projectile")]
    [SerializeField] private ProjectileScript projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float muzzleVelocity;

    [Header("Water Balloon")]
    [SerializeField] private bool waterBalloonLauncher;
    [SerializeField] private WaterBalloonProjectileScript waterBalloonPrefab;
    [SerializeField] private WaterBalloonUpgradeScript waterBalloonUpgrades;

    [Header("Milk Churn")]
    [SerializeField] private bool milkChurn;
    [SerializeField] private BlobProjectileScript blobPrefab;
    [SerializeField] private MilkChurnUpgradeScript milkChurnUpgrades;

    [Header("Fire")]
    [SerializeField] private bool automatic = false;
    [SerializeField] private float fireRate = 0.1f;

    [Header("Shotgun")]
    [SerializeField] private bool shotgun = false;
    [SerializeField] private int projectileCount = 6;
    [SerializeField] private float spreadAngle = 15f;

    [Header("Shotgun Spray Upgrades")]
    [SerializeField] private ShotgunSprayUpgradeScript shotgunUpgrades;
    [SerializeField] private float shotgunBurstInterval = 0.25f;

    [Header("Sniper Upgrades")]
    [SerializeField] private bool sniperWeapon;
    [SerializeField] private SniperUpgradeScript sniperUpgrades;

    [Header("Liquid Stream")]
    [SerializeField] private LiquidStreamScript liquidStream;
    [SerializeField] private float streamAmmoInterval = 0.1f;

    [Header("Ammunition")]
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private float reloadDuration = 1.5f;

    private int currentAmmo;
    private int reserveAmmo;
    private float nextFireTime;
    private float streamAmmoTimer;
    private bool streamFiring;
    private bool isReloading;
    private bool shotgunBurstInProgress;

    private Coroutine reloadCoroutine;
    private Coroutine shotgunBurstCoroutine;

    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;

    public bool IsAutomatic => automatic || (fireMode == FireMode.LiquidStream && !shotgun || fireMode == FireMode.RestorativeGas);

    private void Awake()
    {
        if (aimCamera == null)
            aimCamera = Camera.main;

        magazineSize = Mathf.Max(1, magazineSize);

        if (fireMode == FireMode.RestorativeGas &&
            restorativeGasUpgrades != null)
        {
            magazineSize = Mathf.Max(1, Mathf.RoundToInt(magazineSize * restorativeGasUpgrades.MagazineSizeMultiplier));
        }

        currentAmmo = magazineSize;
        reserveAmmo = int.MaxValue;
    }

    private void Update()
    {
        if (isReloading || !streamFiring || fireMode != FireMode.LiquidStream || shotgun)
            return;

        streamAmmoTimer += Time.deltaTime;

        if (streamAmmoTimer >= Mathf.Max(0.01f, streamAmmoInterval))
        {
            streamAmmoTimer = 0f;
            currentAmmo--;

            if (currentAmmo <= 0)
            {
                currentAmmo = 0;
                StopAttack();
                TryAutoReload();
            }
        }
    }

    private bool HasValidProjectilePrefab()
    {
        if (waterBalloonLauncher)
            return waterBalloonPrefab != null;

        if (milkChurn)
            return blobPrefab != null;

        return projectilePrefab != null;
    }

    private Vector3 GetAimDirection()
    {
        if (aimCamera == null || muzzle == null)
            return muzzle != null ? muzzle.forward : transform.forward;

        Ray aimRay = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 aimPoint;

        if (Physics.Raycast(aimRay, out RaycastHit hit, aimDistance, aimLayers, QueryTriggerInteraction.Collide))
        {
            aimPoint = hit.point;
        }
        else
        {
            aimPoint = aimRay.GetPoint(aimDistance);
        }

        return (aimPoint - muzzle.position).normalized;
    }

    // Fires repeatedly while automatic fire input is held.
    public void HoldAttack()
    {
        if (isReloading)
            return;

        if (fireMode == FireMode.RestorativeGas)
        {
            if (currentAmmo <= 0)
            {
                TryAutoReload();
                return;
            }

            if (Time.time < nextFireTime)
                return;

            if (gasCloudPrefab == null || muzzle == null)
                return;

            currentAmmo--;

            FireRestorativeGas();

            nextFireTime = Time.time + gasFireInterval;
            return;
        }

        if (fireMode == FireMode.LiquidStream)
        {
            if (shotgun)
                return;

            if (currentAmmo <= 0)
            {
                StopAttack();
                TryAutoReload();
                return;
            }

            if (liquidStream != null && !streamFiring)
            {
                streamFiring = true;
                streamAmmoTimer = 0f;
                liquidStream.SetStreamActive(true);
            }

            return;
        }

        if (!automatic || Time.time < nextFireTime)
            return;

        if (currentAmmo <= 0)
        {
            TryAutoReload();
            return;
        }

        if (!HasValidProjectilePrefab() || muzzle == null)
            return;

        currentAmmo--;
        Fire();
        nextFireTime = Time.time + fireRate;

        if (currentAmmo <= 0)
        {
            currentAmmo = 0;
            TryAutoReload();
        }
    }

    // Handles a single attack.
    public override void Attack()
    {
        if (isReloading || shotgunBurstInProgress)
            return;

        if (fireMode == FireMode.RestorativeGas)
        {
            if (currentAmmo <= 0)
            {
                TryAutoReload();
                return;
            }

            if (Time.time < nextFireTime || !CanAttack())
                return;

            if (gasCloudPrefab == null || muzzle == null)
                return;

            currentAmmo--;

            StartAttackCooldown();

            FireRestorativeGas();

            nextFireTime = Time.time + gasFireInterval;
            return;
        }

        if (fireMode == FireMode.LiquidStream)
        {
            if (liquidStream == null)
                return;

            if (shotgun)
            {
                if (currentAmmo <= 0)
                {
                    TryAutoReload();
                    return;
                }

                if (!CanAttack())
                    return;

                int burstsToFire = shotgunUpgrades != null ? shotgunUpgrades.BurstsPerPress : 1;

                burstsToFire = Mathf.Min(burstsToFire, currentAmmo);

                currentAmmo -= burstsToFire;

                StartAttackCooldown();

                shotgunBurstInProgress = true;
                shotgunBurstCoroutine = StartCoroutine(FireShotgunBursts(burstsToFire));
            }
            else
            {
                if (currentAmmo <= 0)
                {
                    TryAutoReload();
                    return;
                }

                if (!streamFiring)
                {
                    streamFiring = true;
                    streamAmmoTimer = 0f;
                    liquidStream.SetStreamActive(true);
                }
            }

            return;
        }

        if (automatic)
            return;

        if (currentAmmo <= 0)
        {
            TryAutoReload();
            return;
        }

        if (!CanAttack())
            return;

        if (!HasValidProjectilePrefab() || muzzle == null)
            return;

        currentAmmo--;
        StartAttackCooldown();
        Fire();

        if (currentAmmo <= 0)
        {
            currentAmmo = 0;
            TryAutoReload();
        }
    }

    // Fires the shotgun bursts consecutively.
    private IEnumerator FireShotgunBursts(int burstCount)
    {
        for (int i = 0; i < burstCount; i++)
        {
            if (liquidStream == null)
                break;

            liquidStream.FireBurst();

            if (i < burstCount - 1)
            {
                yield return new WaitForSeconds(Mathf.Max(0.01f, shotgunBurstInterval));
            }
        }

        shotgunBurstInProgress = false;
        shotgunBurstCoroutine = null;

        if (currentAmmo <= 0)
        {
            currentAmmo = 0;
            TryAutoReload();
        }
    }

    // Reloads the weapon using available reserve ammunition.
    public void Reload()
    {
        if (isReloading || shotgunBurstInProgress || currentAmmo >= magazineSize || reserveAmmo <= 0)
            return;

        StopAttack();
        reloadCoroutine = StartCoroutine(ReloadRoutine());
    }

    private void TryAutoReload()
    {
        if (currentAmmo <= 0 && reserveAmmo > 0)
            Reload();
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        StopAttack();

        float finalReloadDuration = reloadDuration;

        if (shotgunUpgrades != null)
        {
            finalReloadDuration *= shotgunUpgrades.ReloadDurationMultiplier;
        }

        if (sniperWeapon && sniperUpgrades != null)
        {
            finalReloadDuration *= sniperUpgrades.ReloadDurationMultiplier;
        }

        if (milkChurn && milkChurnUpgrades != null)
        {
            finalReloadDuration *= milkChurnUpgrades.ReloadDurationMultiplier;
        }

        if (fireMode == FireMode.RestorativeGas && restorativeGasUpgrades != null)
        {
            finalReloadDuration *= restorativeGasUpgrades.ReloadDurationMultiplier;
        }

        yield return new WaitForSeconds(finalReloadDuration);

        int ammoNeeded = magazineSize - currentAmmo;
        currentAmmo += ammoNeeded;

        isReloading = false;
        reloadCoroutine = null;
    }

    // Stops sustained firing when the attack button is released.
    public void StopAttack()
    {
        streamFiring = false;
        streamAmmoTimer = 0f;

        if (liquidStream != null && !shotgun)
            liquidStream.SetStreamActive(false);
    }

    // Chooses between a single projectile and a shotgun spread.
    private void Fire()
    {
        if (waterBalloonLauncher)
        {
            FireWaterBalloon();
            return;
        }

        if (milkChurn)
        {
            FireMilkChurn();
            return;
        }

        if (projectilePrefab == null || muzzle == null)
            return;

        if (shotgun)
            FireShotgun();
        else
            FireSingle();
    }

    private void FireLiquidBurst()
    {
        if (liquidStream != null)
            liquidStream.FireBurst();
    }

    // Spawns and launches one projectile.
    private void FireSingle()
    {
        Vector3 direction = GetAimDirection();

        ProjectileScript projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(direction));

        projectile.Initialize(sniperWeapon, sniperWeapon && sniperUpgrades != null && sniperUpgrades.RicochetShot);

        Rigidbody projectileBody = projectile.GetComponent<Rigidbody>();

        if (projectileBody != null)
            projectileBody.linearVelocity = direction * muzzleVelocity;
    }

    // Spawns multiple projectiles with randomized directions.
    private void FireShotgun()
    {
        Vector3 aimDirection = GetAimDirection();

        for (int i = 0; i < projectileCount; i++)
        {
            Vector3 direction = GetSpreadDirection(aimDirection);

            ProjectileScript projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(direction));

            projectile.Initialize(sniperWeapon, sniperWeapon && sniperUpgrades != null && sniperUpgrades.RicochetShot);

            Rigidbody projectileBody = projectile.GetComponent<Rigidbody>();
            
            if (projectileBody != null)
                projectileBody.linearVelocity = direction * muzzleVelocity;
        }
    }

    private void FireMilkChurn()
    {
        if (blobPrefab == null || muzzle == null)
            return;

        Vector3 aimDirection = GetAimDirection();

        for (int i = 0; i < projectileCount; i++)
        {
            Vector3 direction = GetSpreadDirection(aimDirection);

            BlobProjectileScript blob = Instantiate(blobPrefab, muzzle.position, Quaternion.LookRotation(direction));

            blob.Initialize(direction * muzzleVelocity, transform.root, milkChurnUpgrades);
        }
    }

    private Vector3 GetSpreadDirection(Vector3 aimDirection)
    {
        float horizontalAngle = Random.Range(-spreadAngle, spreadAngle);
        float verticalAngle = Random.Range(-spreadAngle, spreadAngle);

        Quaternion aimRotation = Quaternion.LookRotation(aimDirection);

        Quaternion spreadRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);

        return aimRotation * spreadRotation * Vector3.forward;
    }

    private void FireWaterBalloon()
    {
        if (waterBalloonPrefab == null || muzzle == null)
            return;

        Vector3 direction = GetAimDirection();

        WaterBalloonProjectileScript projectile = Instantiate(waterBalloonPrefab, muzzle.position, Quaternion.LookRotation(direction));

        projectile.Initialize(direction * muzzleVelocity, transform.root, waterBalloonUpgrades);
    }

    private void FireRestorativeGas()
    {
        if (gasCloudPrefab == null || muzzle == null)
            return;

        Vector3 direction = GetAimDirection();

        RestorativeGasCloudScript cloud = Instantiate(gasCloudPrefab, muzzle.position, Quaternion.LookRotation(direction));

        cloud.Initialize(direction, restorativeGasUpgrades);
    }

    private void OnDisable()
    {
        StopAttack();

        if (reloadCoroutine != null)
            StopCoroutine(reloadCoroutine);

        if (shotgunBurstCoroutine != null)
            StopCoroutine (shotgunBurstCoroutine);

        reloadCoroutine = null;
        shotgunBurstCoroutine = null;

        isReloading = false;
        shotgunBurstInProgress = false;
    }
}