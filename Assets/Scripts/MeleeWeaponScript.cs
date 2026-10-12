using UnityEngine;

public class MeleeWeaponScript : WeaponScript
{
    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackRadius = 0.75f;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask damageLayers;

    [Header("Knockback")]
    [SerializeField] private float knockbackSpeed = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    public override void Attack()
    {
        if (!CanAttack())
            return;

        StartAttackCooldown();

        Vector3 attackPosition =
            transform.position + transform.forward * attackRange;

        Collider[] hits = Physics.OverlapSphere(
            attackPosition,
            attackRadius,
            damageLayers
        );

        foreach (Collider hit in hits)
        {
            EnemyHealthScript health =
                hit.GetComponentInParent<EnemyHealthScript>();

            if (health == null)
                continue;

            // Push the enemy away from the weapon.
            Vector3 knockbackDirection =
                health.transform.position - transform.position;

            // Keep knockback horizontal.
            knockbackDirection.y = 0f;

            if (knockbackDirection.sqrMagnitude > 0.001f)
            {
                knockbackDirection.Normalize();

                health.TakeDamage(
                    damage,
                    knockbackDirection,
                    knockbackSpeed,
                    knockbackDuration
                );
            }
            else
            {
                health.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 attackPosition =
            transform.position + transform.forward * attackRange;

        Gizmos.DrawWireSphere(attackPosition, attackRadius);
    }
}