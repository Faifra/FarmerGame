using UnityEngine;

public class RangedWeaponScript : WeaponScript
{
    [SerializeField] private ProjectileScript projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float muzzleVelocity;

    public override void Attack()
    {
        ProjectileScript projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

        Rigidbody projectileBody = projectile.GetComponent<Rigidbody>();

        projectileBody.linearVelocity = muzzle.forward * muzzleVelocity;
    }
}
