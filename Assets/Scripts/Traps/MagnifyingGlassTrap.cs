
using UnityEngine;

/// <summary>
/// Tracks enemies and focuses sunlight on them, dealing damage and slowing them.
/// </summary>
public class MagnifyingGlassTrapScript : TrapBaseScript
{
    [Header("Targeting")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private Transform lensPivot;
    [SerializeField] private Transform beamStart;
    [SerializeField] private LayerMask obstacleLayers;

    [Header("Sunlight Beam")]
    [SerializeField] private LineRenderer sunlightBeam;
    [SerializeField] private float beamWidth = 0.12f;
    [SerializeField] private float targetHeight = 0.8f;

    [Header("Damage")]
    [SerializeField] private int damagePerTick = 1;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private int maximumTotalDamage = 250;

    [Header("Slow")]
    [SerializeField, Range(0.1f, 1f)] private float slowMultiplier = 0.5f;
    [SerializeField] private float slowDuration = 1.2f;

    private EnemyHealthScript currentTarget;
    private float damageTimer;
    private bool trapPlaced;

    private int totalDamageDealt;

    public void InitializePlacedTrap()
    {
        trapPlaced = true;

        if (sunlightBeam != null)
        {
            sunlightBeam.positionCount = 2;
            sunlightBeam.useWorldSpace = true;
            sunlightBeam.startWidth = beamWidth;
            sunlightBeam.endWidth = beamWidth * 0.25f;
            sunlightBeam.enabled = false;
        }

        FindNewTarget();
    }

    private void Update()
    {
        if (!trapPlaced)
            return;

        if (currentTarget == null || !IsTargetInRange() || !IsTargetVisible(currentTarget))
        {
            currentTarget = null;
            FindNewTarget();
        }

        if (currentTarget == null)
        {
            if (sunlightBeam != null)
                sunlightBeam.enabled = false;

            damageTimer = 0f;
            return;
        }

        TrackTarget();
        UpdateSunlightBeam();

        damageTimer += Time.deltaTime;

        if (damageTimer >= damageInterval)
        {
            damageTimer = 0f;
            DamageTarget();
        }
    }

    private void FindNewTarget()
    {
        EnemyHealthScript[] enemies = FindObjectsByType<EnemyHealthScript>(FindObjectsSortMode.None);

        EnemyHealthScript closestEnemy = null;
        float closestDistance = detectionRange;

        foreach (EnemyHealthScript enemy in enemies)
        {
            if (enemy == null)
                continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }

            if (!IsTargetVisible(enemy))
                continue;
        }

        currentTarget = closestEnemy;
        damageTimer = 0f;
    }

    private bool IsTargetInRange()
    {
        if (currentTarget == null)
            return false;

        return Vector3.Distance(transform.position, currentTarget.transform.position) <= detectionRange;
    }

    private bool IsTargetVisible(EnemyHealthScript enemy)
    {
        if (enemy == null)
            return false;

        Vector3 origin = beamStart != null ? beamStart.position : transform.position;

        Vector3 targetPosition = enemy.transform.position + Vector3.up * targetHeight;

        Vector3 direction = targetPosition - origin;
        float distance = direction.magnitude;

        if (distance <= 0.001f)
            return true;

        return !Physics.Raycast(origin, direction.normalized, distance, obstacleLayers, QueryTriggerInteraction.Ignore);
    }

    private void TrackTarget()
    {
        if (lensPivot == null || currentTarget == null)
            return;

        Vector3 targetPosition = currentTarget.transform.position + Vector3.up * targetHeight;

        Vector3 direction = targetPosition - lensPivot.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            lensPivot.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void UpdateSunlightBeam()
    {
        if (sunlightBeam == null || beamStart == null || currentTarget == null)
            return;

        Vector3 targetPosition = currentTarget.transform.position + Vector3.up * targetHeight;

        sunlightBeam.SetPosition(0, beamStart.position);
        sunlightBeam.SetPosition(1, targetPosition);
        sunlightBeam.enabled = true;
    }

    private void DamageTarget()
    {
        if (currentTarget == null)
            return;

        int remainingDamage = maximumTotalDamage - totalDamageDealt;

        if (remainingDamage <= 0)
        {
            DestroyTrap();
            return;
        }

        int damage = Mathf.Min(damagePerTick, remainingDamage);

        currentTarget.TakeDamage(damage);

        totalDamageDealt += damage;

        MoveTo enemyMovement = currentTarget.GetComponent<MoveTo>();

        if (enemyMovement != null)
        {
            enemyMovement.ApplySlow(slowMultiplier, slowDuration);
        }

        if (totalDamageDealt >= maximumTotalDamage)
        {
            DestroyTrap();
        }
    }

    public override void Activate(GameObject target) { }

    private void DestroyTrap()
    {
        if (sunlightBeam != null)
            sunlightBeam.enabled = false;

        Destroy(gameObject);
    }
}