
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

    [Header("Crosshair Aiming")]
    [SerializeField] private Camera aimCamera;
    [SerializeField] private float aimDistance = 1000f;
    [SerializeField] private LayerMask aimLayers = ~0;

    [Header("References")]
    [SerializeField] private GrappleControllerScript grappleController;
    [SerializeField] private GrappleUpgradeScript grappleUpgrades;

    private float baseAttackCooldown;
    private bool cooldownInitialized;

    private void Awake()
    {
        if (aimCamera == null)
            aimCamera = Camera.main;
    }

    public override void Attack()
    {
        if (!cooldownInitialized)
        {
            baseAttackCooldown = attackCooldown;
            cooldownInitialized = true;
        }

        attackCooldown = baseAttackCooldown * (grappleUpgrades != null ? grappleUpgrades.CooldownMultiplier : 1f);

        if (!CanAttack())
            return;

        if (projectilePrefab == null || muzzle == null || grappleController == null)
            return;

        if (grappleController.IsTethered)
            return;

        Vector3 direction = GetAimDirection();

        StartAttackCooldown();

        GrappleProjectileScript projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(direction));

        projectile.Initialize(grappleController);

        Rigidbody body = projectile.GetComponent<Rigidbody>();

        if (body != null)
            body.linearVelocity = direction * projectileSpeed;
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
}