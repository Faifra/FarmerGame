using UnityEngine;

public class RangedWeaponScript : WeaponScript
{
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

    private float nextFireTime = 0f;

    public bool IsAutomatic => automatic;

    public void HoldAttack()
    {
        if (!automatic)
            return;

        if (Time.time < nextFireTime)
            return;

        Fire();

        nextFireTime = Time.time + fireRate;
    }

    public override void Attack()
    {
        if (automatic)
            return;

        Fire();
    }

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

    private void FireSingle()
    {
        ProjectileScript projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

        Rigidbody projectileBody = projectile.GetComponent<Rigidbody>();

        projectileBody.linearVelocity = muzzle.forward * muzzleVelocity;
    }

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

    private Vector3 GetSpreadDirection()
    {
        float horizontalAngle = Random.Range(-spreadAngle, spreadAngle);

        float verticalAngle = Random.Range(-spreadAngle, spreadAngle);

        Quaternion spreadRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);

        return spreadRotation * muzzle.forward;
    }
}