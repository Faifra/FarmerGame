using UnityEngine;

/// <summary>
/// Handles melee attacks, damage detection, and knockback.
/// </summary>
public class MeleeWeaponScript : WeaponScript
{
    // Controls the attack's reach, hit radius, damage, and valid target layers.
    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackRadius = 0.75f;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask damageLayers;

    // Controls the strength and duration of knockback applied to enemies.
    [Header("Knockback")]
    [SerializeField] private float knockbackSpeed = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    // Performs a melee attack and damages all valid enemies within range.
    public override void Attack()
    {
        if (!CanAttack())
            return;

        StartAttackCooldown();

        Vector3 attackPosition = transform.position + transform.forward * attackRange;

        Collider[] hits = Physics.OverlapSphere(attackPosition, attackRadius, damageLayers);

        foreach (Collider hit in hits)
        {
            EnemyHealthScript health = hit.GetComponentInParent<EnemyHealthScript>();

            if (health == null)
                continue;

            Vector3 knockbackDirection = health.transform.position - transform.position;

            knockbackDirection.y = 0f;

            if (knockbackDirection.sqrMagnitude > 0.001f)
            {
                knockbackDirection.Normalize();

                health.TakeDamage(damage, knockbackDirection, knockbackSpeed, knockbackDuration);
            }
            else
            {
                health.TakeDamage(damage);
            }
        }
    }

    // Displays the attack's range and radius in the Unity Scene view.
    private void OnDrawGizmosSelected()
    {
        Vector3 attackPosition = transform.position + transform.forward * attackRange;

        Gizmos.DrawWireSphere(attackPosition, attackRadius);
    }
}