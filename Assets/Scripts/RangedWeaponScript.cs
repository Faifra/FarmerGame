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
        LiquidStream
    }

    [Header("Firing Mode")]
    [SerializeField] private FireMode fireMode = FireMode.Projectile;

    [Header("Projectile")]
    [SerializeField] private ProjectileScript projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float muzzleVelocity;

    [Header("Fire")]
    [SerializeField] private bool automatic = false;
    [SerializeField] private float fireRate = 0.1f;

    [Header("Shotgun")]
    [SerializeField] private bool shotgun = false;
    [SerializeField] private int projectileCount = 6;
    [SerializeField] private float spreadAngle = 15f;

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
    private Coroutine reloadCoroutine;

    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;

    public bool IsAutomatic => automatic || (fireMode == FireMode.LiquidStream && !shotgun);

    private void Awake()
    {
        magazineSize = Mathf.Max(1, magazineSize);
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

    // Fires repeatedly while automatic fire input is held.
    public void HoldAttack()
    {
        if (isReloading)
            return;

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

        if (projectilePrefab == null || muzzle == null)
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
        if (isReloading)
            return;

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

                currentAmmo--;
                StartAttackCooldown();
                FireLiquidBurst();

                if (currentAmmo <= 0)
                {
                    currentAmmo = 0;
                    TryAutoReload();
                }
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

        if (projectilePrefab == null || muzzle == null)
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

    // Reloads the weapon using available reserve ammunition.
    public void Reload()
    {
        if (isReloading || currentAmmo >= magazineSize || reserveAmmo <= 0)
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

        yield return new WaitForSeconds(reloadDuration);

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
        ProjectileScript projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

        Rigidbody projectileBody = projectile.GetComponent<Rigidbody>();

        if (projectileBody != null)
            projectileBody.linearVelocity = muzzle.forward * muzzleVelocity;
    }

    // Spawns multiple projectiles with randomized directions.
    private void FireShotgun()
    {
        for (int i = 0; i < projectileCount; i++)
        {
            Vector3 direction = GetSpreadDirection();

            ProjectileScript projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(direction));

            Rigidbody projectileBody = projectile.GetComponent<Rigidbody>();

            if (projectileBody != null)
                projectileBody.linearVelocity = direction * muzzleVelocity;
        }
    }

    private Vector3 GetSpreadDirection()
    {
        float horizontalAngle = Random.Range(-spreadAngle, spreadAngle);
        float verticalAngle = Random.Range(-spreadAngle, spreadAngle);

        Quaternion spreadRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);

        return spreadRotation * muzzle.forward;
    }

    private void OnDisable()
    {
        StopAttack();

        if (reloadCoroutine != null)
            StopCoroutine(reloadCoroutine);

        reloadCoroutine = null;
        isReloading = false;
    }
}