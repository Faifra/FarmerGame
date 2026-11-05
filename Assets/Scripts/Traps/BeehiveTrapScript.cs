
using UnityEngine;

/// <summary>
/// Spawns and manages a bee swarm that attacks enemies near the placed hive.
/// </summary>
public class BeehiveTrapScript : TrapBaseScript
{
    [Header("Swarm")]
    [SerializeField] private GameObject beeSwarmPrefab;
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private float targetCheckInterval = 0.2f;

    private BeeSwarmScript beeSwarm;
    private float targetCheckTimer;
    private bool hivePlaced;

    private EnemyDropsScript drops;

    public void InitializePlacedHive()
    {
        if (hivePlaced)
            return;

        hivePlaced = true;

        if (beeSwarmPrefab == null)
            return;
        

        GameObject swarmObject = Instantiate(beeSwarmPrefab, transform.position, Quaternion.identity);

        beeSwarm = swarmObject.GetComponent<BeeSwarmScript>();

        if (beeSwarm == null)
        {
            Destroy(swarmObject);
            return;
        }

        beeSwarm.SetHive(this);
    }

    private void Update()
    {
        if (!hivePlaced || beeSwarm == null)
            return;

        targetCheckTimer += Time.deltaTime;

        if (targetCheckTimer < targetCheckInterval)
            return;

        targetCheckTimer = 0f;

        if (beeSwarm.HasTarget() && beeSwarm.IsTargetInRange(detectionRange))
            return;

        EnemyHealthScript closestEnemy = FindClosestEnemy();

        beeSwarm.SetTarget(closestEnemy);
    }

    private EnemyHealthScript FindClosestEnemy()
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
        }

        return closestEnemy;
    }

    public override void Activate(GameObject target) { }

    public void SwarmReturned(bool damageLimitReached)
    {
        if (damageLimitReached)
        {
            if (drops == null)
                drops = GetComponent<EnemyDropsScript>();

            if (drops != null)
            {
                drops.Drop();
            }

            Destroy(gameObject);
        }
    }
}