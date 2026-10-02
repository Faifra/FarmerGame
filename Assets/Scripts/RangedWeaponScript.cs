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

        ProjectileScript projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

        Rigidbody projectileBody = projectile.GetComponent<Rigidbody>();

        projectileBody.linearVelocity = muzzle.forward * muzzleVelocity;
    }
}
