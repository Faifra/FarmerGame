using UnityEngine;

/// <summary>
/// Handles ranged weapon attacks, including automatic fire and shotgun spread.
/// </summary>
public class RangedWeaponScript : WeaponScript
{
    // Projectile configuration and firing origin.
    [Header("Projectile")]
    [SerializeField] private ProjectileScript projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float muzzleVelocity;

    // Controls automatic firing and the delay between shots.
    [Header("Fire")]
    [SerializeField] private bool automatic = false;
    [SerializeField] private float fireRate = 0.1f;

    // Configures shotgun firing and projectile spread.
    [Header("Shotgun")]
    [SerializeField] private bool shotgun = false;
    [SerializeField] private int projectileCount = 6;
    [SerializeField] private float spreadAngle = 15f;

    // Time when the weapon can next fire automatically.
    private float nextFireTime = 0f;

    // Indicates whether the weapon supports continuous firing.
    public bool IsAutomatic => automatic;

    // Fires repeatedly while the attack input is held.
    public void HoldAttack()
    {
        if (!automatic)
            return;

        if (Time.time < nextFireTime)
            return;

        Fire();

        nextFireTime = Time.time + fireRate;
    }

    // Handles a single attack from a non-automatic weapon.
    public override void Attack()
    {
        if (automatic)
            return;

        if (!CanAttack())
            return;

        if (projectilePrefab == null || muzzle == null)
            return;

        StartAttackCooldown();
        Fire();
    }

    // Chooses between a single projectile and a shotgun spread.
    private void Fire()
    {
        if (projectilePrefab == null || muzzle == null)
            return;

        if (shotgun)
        {
            FireShotgun();
        }
        else
        {
            FireSingle();
        }
    }

    // Spawns and launches one projectile in the muzzle's forward direction.
    private void FireSingle()
    {
        ProjectileScript projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

        Rigidbody projectileBody = projectile.GetComponent<Rigidbody>();

        projectileBody.linearVelocity = muzzle.forward * muzzleVelocity;
    }

    // Spawns multiple projectiles with randomized firing directions.
    private void FireShotgun()
    {
        for (int i = 0; i < projectileCount; i++)
        {
            Vector3 direction = GetSpreadDirection();

            ProjectileScript projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(direction));

            Rigidbody projectileBody = projectile.GetComponent<Rigidbody>();

            projectileBody.linearVelocity = direction * muzzleVelocity;
        }
    }

    // Calculates a random projectile direction within the configured spread angles.
    private Vector3 GetSpreadDirection()
    {
        float horizontalAngle = Random.Range(-spreadAngle, spreadAngle);

        float verticalAngle = Random.Range(-spreadAngle, spreadAngle);

        Quaternion spreadRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);

        return spreadRotation * muzzle.forward;
    }
}