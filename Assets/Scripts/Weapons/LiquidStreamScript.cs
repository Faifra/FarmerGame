using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles continuous liquid streams and periodic damage to enemies inside the stream.
/// </summary>
public class LiquidStreamScript : MonoBehaviour
{
    [Header("Stream")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private float range = 10f;
    [SerializeField] private float spreadAngle = 5f;
    [SerializeField] private float closeRangeRadius = 1f;

    [Header("Burst")]
    [SerializeField] private float burstDuration = 0.2f;

    [Header("Damage")]
    [SerializeField] private int damagePerTick = 2;
    [SerializeField] private float tickInterval = 0.2f;

    [Header("Upgrades")]
    [SerializeField] private BasicSprayUpgradeScript sprayUpgrades;
    [SerializeField] private ShotgunSprayUpgradeScript shotgunUpgrades;

    [Header("Visual Effect")]
    [SerializeField] private ParticleSystem streamEffect;

    private bool isActive;
    private float tickTimer;

    private float burstTimer;
    private bool burstActive;

    // Tracks enemies currently being sprayed and how long they have been sprayed.
    private readonly Dictionary<EnemyHealthScript, float> enemyExposureTimes = new Dictionary<EnemyHealthScript, float>();

    private readonly HashSet<EnemyWallScript> hitWalls = new();

    // Tracks which enemies have already received knockback during this exposure.
    private readonly HashSet<EnemyHealthScript> knockedBackEnemies = new HashSet<EnemyHealthScript>();

    // Tracks enemies currently inside the stream during a damage tick.
    private readonly HashSet<EnemyHealthScript> hitEnemies = new HashSet<EnemyHealthScript>();

    private readonly List<EnemyHealthScript> enemiesToRemove = new List<EnemyHealthScript>();

    public void SetStreamActive(bool active)
    {
        if (!active)
        {
            FinishAllExposures();
        }

        if (isActive == active && !burstActive)
            return;

        burstActive = false;
        isActive = active;
        tickTimer = 0f;

        if (streamEffect != null)
        {
            if (isActive)
                streamEffect.Play();
            else
                streamEffect.Stop();
        }
    }

    private void Update()
    {
        if (!isActive || muzzle == null)
            return;

        if (burstActive)
        {
            burstTimer += Time.deltaTime;

            if (burstTimer >= burstDuration)
            {
                burstActive = false;
                isActive = false;

                if (streamEffect != null)
                    streamEffect.Stop();

                return;
            }
        }

        tickTimer += Time.deltaTime;

        if (tickTimer >= tickInterval)
        {
            tickTimer %= Mathf.Max(tickInterval, 0.001f);
            ApplyDamageTick();
        }
    }

    // Damages enemies inside the stream and tracks their exposure.
    private void ApplyDamageTick()
    {
        float currentRange = range;

        if (sprayUpgrades != null)
            currentRange += sprayUpgrades.AdditionalRange;

        Collider[] hits = Physics.OverlapSphere(muzzle.position, currentRange, Physics.AllLayers, QueryTriggerInteraction.Collide);

        hitEnemies.Clear();
        hitWalls.Clear();

        foreach (Collider hit in hits)
        {
            EnemyHealthScript health = hit.GetComponentInParent<EnemyHealthScript>();
            EnemyWallScript wall = hit.GetComponentInParent<EnemyWallScript>();

            if (health == null && wall == null)
                continue;

            if (health != null && (health.IsDead || hitEnemies.Contains(health)))
                continue;

            if (wall != null && health == null && hitWalls.Contains(wall))
                continue;

            Vector3 targetPoint = hit.ClosestPoint(muzzle.position);
            Vector3 direction = targetPoint - muzzle.position;
            float distance = direction.magnitude;

            if (distance > currentRange)
                continue;

            if (distance > closeRangeRadius)
            {
                float angle = Vector3.Angle(muzzle.forward, direction.normalized);

                if (angle > spreadAngle)
                    continue;
            }

            float damageMultiplier = sprayUpgrades != null ? sprayUpgrades.DamageMultiplier : 1f;

            if (shotgunUpgrades != null)
            {
                damageMultiplier *= shotgunUpgrades.GetDamageMultiplier(distance, currentRange);
            }

            int finalDamage = Mathf.Max(1, Mathf.RoundToInt(damagePerTick * damageMultiplier));

            if (health != null)
            {
                hitEnemies.Add(health);

                if (!enemyExposureTimes.ContainsKey(health))
                    enemyExposureTimes.Add(health, 0f);

                health.TakeDamage(finalDamage);

                enemyExposureTimes[health] += tickInterval;

                if (sprayUpgrades != null && sprayUpgrades.Knockback && !knockedBackEnemies.Contains(health) && enemyExposureTimes[health] >= sprayUpgrades.KnockbackThreshold)
                {
                    Vector3 knockbackDirection = health.transform.position - muzzle.position;
                    knockbackDirection.y = 0f;

                    if (knockbackDirection.sqrMagnitude > 0.001f)
                    {
                        health.ApplyKnockback(knockbackDirection, sprayUpgrades.KnockbackSpeed, sprayUpgrades.KnockbackDuration);

                        knockedBackEnemies.Add(health);
                    }
                }
            }
            else if (wall != null)
            {
                hitWalls.Add(wall);
                wall.TakeDamage(finalDamage);
            }
        }

        enemiesToRemove.Clear();

        foreach (KeyValuePair<EnemyHealthScript, float> exposure in enemyExposureTimes)
        {
            if (exposure.Key == null || !hitEnemies.Contains(exposure.Key))
            {
                enemiesToRemove.Add(exposure.Key);
            }
        }

        foreach (EnemyHealthScript enemy in enemiesToRemove)
        {
            FinishExposure(enemy);
        }
    }

    // Applies bonus damage and removes an enemy's exposure tracking.
    private void FinishExposure(EnemyHealthScript enemy)
    {
        if (ReferenceEquals(enemy, null))
            return;

        if (enemyExposureTimes.ContainsKey(enemy))
        {
            if (enemy != null && sprayUpgrades != null && !enemy.IsDead)
            {
                enemy.TakePercentageDamage(sprayUpgrades.BonusDamagePercentage);
            }

            enemyExposureTimes.Remove(enemy);
        }

        knockedBackEnemies.Remove(enemy);
    }

    // Finishes exposure for every enemy when spraying stops.
    private void FinishAllExposures()
    {
        enemiesToRemove.Clear();

        foreach (EnemyHealthScript enemy in enemyExposureTimes.Keys)
        {
            enemiesToRemove.Add(enemy);
        }

        foreach (EnemyHealthScript enemy in enemiesToRemove)
        {
            FinishExposure(enemy);
        }
    }

    // Fires one short burst.
    public void FireBurst()
    {
        if (muzzle == null)
            return;

        FinishAllExposures();

        burstActive = true;
        burstTimer = 0f;
        tickTimer = 0f;
        isActive = true;

        if (streamEffect != null)
        {
            streamEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            streamEffect.Play();
        }
    }

    private void OnDisable()
    {
        FinishAllExposures();

        isActive = false;
        burstActive = false;

        if (streamEffect != null)
            streamEffect.Stop();
    }
}