
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
    [SerializeField, Range(0.1f, 1f)]
    private float slowMultiplier = 0.5f;
    [SerializeField] private float slowDuration = 1.2f;

    [Header("Upgrades")]
    [SerializeField] private MagnifyingGlassTrapUpgradeScript trapUpgrades;

    private EnemyHealthScript currentTarget;
    private EnemyHealthScript secondTarget;

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

        if (currentTarget == null || currentTarget.IsDead || !IsTargetInRange(currentTarget) || !IsTargetVisible(currentTarget))
        {
            currentTarget = null;
            secondTarget = null;
            FindNewTarget();
        }

        if (currentTarget == null)
        {
            if (sunlightBeam != null)
                sunlightBeam.enabled = false;

            damageTimer = 0f;
            return;
        }

        FindSecondTarget();

        TrackTarget();
        UpdateSunlightBeam();

        damageTimer += Time.deltaTime;

        if (damageTimer >= damageInterval)
        {
            damageTimer = 0f;
            DamageTargets();
        }
    }

    private void FindNewTarget()
    {
        EnemyHealthScript[] enemies = FindObjectsByType<EnemyHealthScript>(FindObjectsSortMode.None);

        EnemyHealthScript closestEnemy = null;
        float closestDistance = detectionRange;

        foreach (EnemyHealthScript enemy in enemies)
        {
            if (enemy == null || enemy.IsDead || !IsTargetVisible(enemy))
                continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        currentTarget = closestEnemy;
        secondTarget = null;
        damageTimer = 0f;
    }

    private void FindSecondTarget()
    {
        secondTarget = null;

        if (trapUpgrades == null || !trapUpgrades.RefractedBeam)
            return;

        if (currentTarget == null)
            return;

        EnemyHealthScript[] enemies = FindObjectsByType<EnemyHealthScript>(FindObjectsSortMode.None);

        float closestDistance = float.MaxValue;

        Vector3 firstTargetPosition = currentTarget.transform.position + Vector3.up * targetHeight;

        foreach (EnemyHealthScript enemy in enemies)
        {
            if (enemy == null || enemy.IsDead || enemy == currentTarget || !IsTargetInRange(enemy))
                continue;

            Vector3 secondTargetPosition = enemy.transform.position + Vector3.up * targetHeight;

            float distance = Vector3.Distance(currentTarget.transform.position, enemy.transform.position);

            if (distance >= closestDistance)
                continue;

            if (!IsBeamSegmentVisible(firstTargetPosition, secondTargetPosition))
                continue;

            closestDistance = distance;
            secondTarget = enemy;
        }
    }

    private bool IsTargetInRange(EnemyHealthScript enemy)
    {
        if (enemy == null)
            return false;

        return Vector3.Distance(transform.position, enemy.transform.position) <= detectionRange;
    }

    private bool IsTargetVisible(EnemyHealthScript enemy)
    {
        if (enemy == null)
            return false;

        Vector3 origin = beamStart != null ? beamStart.position : transform.position;

        Vector3 targetPosition = enemy.transform.position + Vector3.up * targetHeight;

        return IsBeamSegmentVisible(origin, targetPosition);
    }

    private bool IsBeamSegmentVisible(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        if (distance <= 0.001f)
            return true;

        return !Physics.Raycast(start, direction.normalized, distance, obstacleLayers, QueryTriggerInteraction.Ignore);
    }

    private void TrackTarget()
    {
        if (lensPivot == null || currentTarget == null)
            return;

        Vector3 targetPosition = currentTarget.transform.position + Vector3.up * targetHeight;

        Vector3 direction = targetPosition - lensPivot.position;

        if (direction.sqrMagnitude > 0.001f)
            lensPivot.rotation = Quaternion.LookRotation(direction);
    }

    private void UpdateSunlightBeam()
    {
        if (sunlightBeam == null || beamStart == null || currentTarget == null)
            return;

        Vector3 firstPosition = currentTarget.transform.position + Vector3.up * targetHeight;

        sunlightBeam.SetPosition(0, beamStart.position);
        sunlightBeam.SetPosition(1, firstPosition);

        if (secondTarget != null)
        {
            Vector3 secondPosition = secondTarget.transform.position + Vector3.up * targetHeight;

            sunlightBeam.positionCount = 3;
            sunlightBeam.SetPosition(2, secondPosition);
        }
        else
        {
            sunlightBeam.positionCount = 2;
        }

        sunlightBeam.enabled = true;
    }

    private void DamageTargets()
    {
        if (currentTarget == null)
            return;

        int damage = Mathf.Max(1, Mathf.RoundToInt(damagePerTick * (trapUpgrades != null ? trapUpgrades.DamageMultiplier : 1f)));

        DamageEnemy(currentTarget, damage);

        if (totalDamageDealt >= maximumTotalDamage)
        {
            DestroyTrap();
            return;
        }

        if (secondTarget != null && !secondTarget.IsDead)
        {
            DamageEnemy(secondTarget, damage);
        }

        if (totalDamageDealt >= maximumTotalDamage)
            DestroyTrap();
    }

    private void DamageEnemy(EnemyHealthScript enemy, int damage)
    {
        if (enemy == null || enemy.IsDead)
            return;

        int remainingDamage = maximumTotalDamage - totalDamageDealt;

        if (remainingDamage <= 0)
            return;

        int actualDamage = Mathf.Min(damage, remainingDamage);

        enemy.TakeDamage(actualDamage);
        totalDamageDealt += actualDamage;

        MoveTo enemyMovement = enemy.GetComponent<MoveTo>();

        if (enemyMovement != null)
        {
            float finalSlowMultiplier = slowMultiplier * (trapUpgrades != null ? trapUpgrades.SlowMultiplier : 1f);

            enemyMovement.ApplySlow(finalSlowMultiplier, slowDuration);
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