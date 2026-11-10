using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns and manages bee swarms that attack enemies near the placed hive.
/// All swarms share one total damage limit.
/// </summary>
public class BeehiveTrapScript : TrapBaseScript
{
    [Header("Swarm")]
    [SerializeField] private GameObject beeSwarmPrefab;
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private float targetCheckInterval = 0.2f;

    [Header("Damage Limit")]
    [SerializeField] private int maximumTotalDamage = 150;

    [Header("Upgrades")]
    [SerializeField] private BeehiveTrapUpgradeScript trapUpgrades;

    private readonly List<BeeSwarmScript> swarms = new List<BeeSwarmScript>();

    private float targetCheckTimer;
    private bool hivePlaced;
    private bool hiveDestroyed;

    private int totalDamageDealt;

    private EnemyDropsScript drops;

    public float SwarmMovementMultiplier => trapUpgrades != null ? trapUpgrades.MovementSpeedMultiplier : 1f;

    public float SwarmDamageMultiplier => trapUpgrades != null ? trapUpgrades.DamageMultiplier : 1f;

    public int RemainingDamage =>
        Mathf.Max(0, GetMaximumDamage() - totalDamageDealt);

    private int GetMaximumDamage()
    {
        float multiplier = trapUpgrades != null ? trapUpgrades.DamageCapMultiplier : 1f;

        return Mathf.Max(1, Mathf.RoundToInt(maximumTotalDamage * multiplier));
    }

    public void InitializePlacedHive()
    {
        if (hivePlaced)
            return;

        hivePlaced = true;
        SpawnSwarm(transform.position);
    }

    private void Update()
    {
        if (!hivePlaced || hiveDestroyed)
            return;

        if (RemainingDamage <= 0)
        {
            DestroyHive();
            return;
        }

        targetCheckTimer += Time.deltaTime;

        if (targetCheckTimer < targetCheckInterval)
            return;

        targetCheckTimer = 0f;

        RemoveMissingSwarms();

        foreach (BeeSwarmScript swarm in swarms)
        {
            if (swarm == null || swarm.HasTarget())
                continue;

            EnemyHealthScript target = FindClosestEnemy(swarm);

            if (target != null)
                swarm.SetTarget(target);
        }
    }

    private void SpawnSwarm(Vector3 spawnPosition)
    {
        if (beeSwarmPrefab == null || hiveDestroyed || RemainingDamage <= 0)
            return;

        GameObject swarmObject = Instantiate(beeSwarmPrefab, spawnPosition, Quaternion.identity);

        BeeSwarmScript swarm = swarmObject.GetComponent<BeeSwarmScript>();

        if (swarm == null)
        {
            Destroy(swarmObject);
            return;
        }

        swarms.Add(swarm);
        swarm.SetHive(this);
    }

    private EnemyHealthScript FindClosestEnemy(BeeSwarmScript requestingSwarm)
    {
        EnemyHealthScript[] enemies = FindObjectsByType<EnemyHealthScript>(FindObjectsSortMode.None);

        EnemyHealthScript closestEnemy = null;
        float closestDistance = detectionRange;

        foreach (EnemyHealthScript enemy in enemies)
        {
            if (enemy == null || enemy.IsDead)
                continue;

            bool alreadyTargeted = false;

            foreach (BeeSwarmScript swarm in swarms)
            {
                if (swarm == null || swarm == requestingSwarm)
                    continue;

                if (swarm.CurrentTarget == enemy)
                {
                    alreadyTargeted = true;
                    break;
                }
            }

            if (alreadyTargeted)
                continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    private void RemoveMissingSwarms()
    {
        swarms.RemoveAll(swarm => swarm == null);
    }

    public bool RegisterDamage(int damage)
    {
        if (hiveDestroyed || damage <= 0)
            return false;

        int actualDamage = Mathf.Min(damage, RemainingDamage);

        totalDamageDealt += actualDamage;

        if (RemainingDamage <= 0)
        {
            foreach (BeeSwarmScript swarm in swarms)
            {
                if (swarm != null)
                    swarm.StopAttacking();
            }
        }

        return RemainingDamage <= 0;
    }

    public void SwarmKilledEnemy(Vector3 enemyPosition)
    {
        if (trapUpgrades == null || !trapUpgrades.SwarmReplication || RemainingDamage <= 0)
            return;

        SpawnSwarm(enemyPosition);
    }

    public void SwarmReturned(BeeSwarmScript swarm)
    {
        swarms.Remove(swarm);

        if (RemainingDamage <= 0)
        {
            DestroyHive();
            return;
        }
    }

    private void DestroyHive()
    {
        if (hiveDestroyed)
            return;

        hiveDestroyed = true;

        foreach (BeeSwarmScript swarm in swarms)
        {
            if (swarm != null)
                Destroy(swarm.gameObject);
        }

        swarms.Clear();

        if (drops == null)
            drops = GetComponent<EnemyDropsScript>();

        if (drops != null)
            drops.Drop();

        Destroy(gameObject);
    }

    public override void Activate(GameObject target) { }
}