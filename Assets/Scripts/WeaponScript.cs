using UnityEngine;

public abstract class WeaponScript : MonoBehaviour
{
    [Header("Attack Cooldown")]
    [SerializeField] protected float attackCooldown = 0.5f;

    private float nextAttackTime = 0f;

    protected bool CanAttack()
    {
        return Time.time >= nextAttackTime;
    }

    protected void StartAttackCooldown()
    {
        nextAttackTime = Time.time + attackCooldown;
    }

    public abstract void Attack();
}