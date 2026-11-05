
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
    [SerializeField] private int maximumTotalDamage = 150;

    private BeehiveTrapScript hive;
    private EnemyHealthScript currentTarget;

    private Vector3 hivePosition;
    private float damageTimer;
    private int totalDamageDealt;

    private bool returning;
    private bool damageLimitReached;

    public void SetHive(BeehiveTrapScript beehive)
    {
        hive = beehive;
        hivePosition = beehive.transform.position;
    }

    public void SetTarget(EnemyHealthScript target)
    {
        if (damageLimitReached)
            return;

        if (currentTarget != null)
            return;

        if (target != null)
        {
            currentTarget = target;
            returning = false;
            damageTimer = 0f;
        }
        else
        {
            returning = true;
        }
    }

    public bool IsTargetInRange(float range)
    {
        if (currentTarget == null || hive == null)
            return false;

        float distance = Vector3.Distance(hive.transform.position, currentTarget.transform.position);

        return distance <= range;
    }

    public bool HasTarget()
    {
        return currentTarget != null;
    }

    private void Update()
    {
        if (hive == null)
        {
            Destroy(gameObject);
            return;
        }

        if (damageLimitReached)
        {
            currentTarget = null;
            returning = true;
        }

        if (currentTarget != null)
        {
            FollowTarget();
        }
        else
        {
            ReturnToHive();
        }
    }

    private void FollowTarget()
    {
        if (currentTarget == null)
        {
            returning = true;
            return;
        }

        Vector3 targetPosition = currentTarget.transform.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        Vector3 direction = targetPosition - transform.position;

        if (direction.sqrMagnitude > 0.001f)
            transform.forward = direction.normalized;

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance <= attackDistance)
        {
            DealDamage();
        }
    }

    private void DealDamage()
    {
        damageTimer += Time.deltaTime;

        if (damageTimer < damageInterval)
            return;

        damageTimer = 0f;

        int remainingDamage = maximumTotalDamage - totalDamageDealt;

        if (remainingDamage <= 0)
        {
            damageLimitReached = true;
            currentTarget = null;
            returning = true;
            return;
        }

        int damage = Mathf.Min(damagePerTick, remainingDamage);

        if (currentTarget != null)
        {
            currentTarget.TakeDamage(damage);
            totalDamageDealt += damage;
        }

        if (totalDamageDealt >= maximumTotalDamage)
        {
            damageLimitReached = true;
            currentTarget = null;
            returning = true;
        }
    }

    private void ReturnToHive()
    {
        returning = true;

        transform.position = Vector3.MoveTowards(transform.position, hivePosition, moveSpeed * Time.deltaTime);

        Vector3 direction = hivePosition - transform.position;

        if (direction.sqrMagnitude > 0.001f)
            transform.forward = direction.normalized;

        if (Vector3.Distance(transform.position, hivePosition) <= returnDistance)
        {
            transform.position = hivePosition;

            if (damageLimitReached)
            {
                hive.SwarmReturned(true);
                Destroy(gameObject);
            }
        }
    }
}