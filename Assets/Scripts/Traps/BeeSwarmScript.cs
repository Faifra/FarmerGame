using UnityEngine;

/// <summary>
/// Moves between the beehive and enemies, dealing damage over time.
/// </summary>
public class BeeSwarmScript : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float returnDistance = 0.2f;

    [Header("Damage")]
    [SerializeField] private int damagePerTick = 1;
    [SerializeField] private float damageInterval = 0.5f;

    private BeehiveTrapScript hive;
    private EnemyHealthScript currentTarget;

    private Vector3 hivePosition;
    private float damageTimer;
    private float fractionalDamage;

    private bool returning;
    private bool stopped;
    private bool returnedToHive;

    public EnemyHealthScript CurrentTarget => currentTarget;

    public void SetHive(BeehiveTrapScript beehive)
    {
        hive = beehive;
        hivePosition = beehive.transform.position;
    }

    public void SetTarget(EnemyHealthScript target)
    {
        if (stopped || target == null || currentTarget != null)
            return;

        currentTarget = target;
        returning = false;
        damageTimer = 0f;
    }

    public bool IsTargetInRange(float range)
    {
        if (currentTarget == null || hive == null)
            return false;

        return Vector3.Distance(hive.transform.position, currentTarget.transform.position) <= range;
    }

    public bool HasTarget()
    {
        return currentTarget != null && !currentTarget.IsDead;
    }

    public void StopAttacking()
    {
        stopped = true;
        currentTarget = null;
        returning = true;
    }

    private void Update()
    {
        if (hive == null)
        {
            Destroy(gameObject);
            return;
        }

        if (hive.RemainingDamage <= 0)
            StopAttacking();

        if (stopped || currentTarget == null || currentTarget.IsDead)
        {
            if (currentTarget != null && currentTarget.IsDead)
                currentTarget = null;

            ReturnToHive();
            return;
        }

        FollowTarget();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = currentTarget.transform.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * hive.SwarmMovementMultiplier * Time.deltaTime);

        Vector3 direction = targetPosition - transform.position;

        if (direction.sqrMagnitude > 0.001f)
            transform.forward = direction.normalized;

        if (Vector3.Distance(transform.position, targetPosition) <= attackDistance)
            DealDamage();
    }

    private void DealDamage()
    {
        if (hive.RemainingDamage <= 0)
        {
            StopAttacking();
            return;
        }

        damageTimer += Time.deltaTime;

        if (damageTimer < damageInterval)
            return;

        damageTimer = 0f;

        float damageToApply = damagePerTick * hive.SwarmDamageMultiplier + fractionalDamage;

        int damage = Mathf.FloorToInt(damageToApply);

        fractionalDamage = damageToApply - damage;

        if (damage <= 0)
            return;

        damage = Mathf.Min(damage, hive.RemainingDamage);

        Vector3 enemyPosition = currentTarget.transform.position;

        currentTarget.TakeDamage(damage);

        bool enemyKilled = currentTarget.IsDead;

        bool limitReached = hive.RegisterDamage(damage);

        if (enemyKilled)
        {
            currentTarget = null;
            hive.SwarmKilledEnemy(enemyPosition);
        }

        if (limitReached)
            StopAttacking();
    }

    private void ReturnToHive()
    {
        returning = true;

        transform.position = Vector3.MoveTowards(transform.position, hivePosition, moveSpeed * hive.SwarmMovementMultiplier * Time.deltaTime);

        Vector3 direction = hivePosition - transform.position;

        if (direction.sqrMagnitude > 0.001f)
            transform.forward = direction.normalized;

        if (Vector3.Distance(transform.position, hivePosition) <= returnDistance)
        {
            transform.position = hivePosition;

            if (stopped && !returnedToHive)
            {
                returnedToHive = true;
                hive.SwarmReturned(this);
                Destroy(gameObject);
            }
        }
    }
}