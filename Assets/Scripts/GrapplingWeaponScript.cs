
using UnityEngine;

/// <summary>
/// Fires a grappling projectile that can attach to enemies.
/// </summary>
public class GrapplingWeaponScript : WeaponScript
{
    [Header("Projectile")]
    [SerializeField] private GrappleProjectileScript projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float projectileSpeed = 35f;

    [Header("References")]
    [SerializeField] private GrappleControllerScript grappleController;

    public override void Attack()
    {
        if (!CanAttack())
            return;

        if (projectilePrefab == null || muzzle == null || grappleController == null)
            return;

        if (grappleController.IsTethered)
            return;

        StartAttackCooldown();

        GrappleProjectileScript projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

        projectile.Initialize(grappleController);

        Rigidbody body = projectile.GetComponent<Rigidbody>();

        if (body != null)
            body.linearVelocity = muzzle.forward * projectileSpeed;
    }
}