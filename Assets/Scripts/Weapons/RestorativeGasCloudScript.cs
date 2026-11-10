using System.Collections.Generic;
using UnityEngine;

public class RestorativeGasCloudScript : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float travelSpeed = 5f;
    [SerializeField] private float travelDuration = 0.5f;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 3f;

    [Header("Damage")]
    [SerializeField] private int damagePerTick = 1;
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private LayerMask enemyLayers;

    private Rigidbody body;
    private SphereCollider cloudCollider;
    private RestorativeGasUpgradeScript gasUpgrades;

    private Vector3 travelDirection;
    private float elapsedTime;
    private bool isTravelling = true;

    private readonly Dictionary<EnemyHealthScript, HashSet<Collider>> overlappingColliders = new Dictionary<EnemyHealthScript, HashSet<Collider>>();

    private class SharedDamageState
    {
        public HashSet<RestorativeGasCloudScript> clouds = new HashSet<RestorativeGasCloudScript>();

        public RestorativeGasCloudScript owner;
        public float enterTime;
        public float nextDamageTime;
        public bool lingering;
    }

    private static readonly Dictionary<EnemyHealthScript, SharedDamageState> sharedDamage = new Dictionary<EnemyHealthScript, SharedDamageState>();

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        cloudCollider = GetComponent<SphereCollider>();

        body.isKinematic = true;
        body.useGravity = false;

        cloudCollider.isTrigger = true;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector3 direction, RestorativeGasUpgradeScript upgrades = null)
    {
        gasUpgrades = upgrades;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        travelDirection = direction.normalized;
        transform.rotation = Quaternion.LookRotation(travelDirection);
    }

    private void FixedUpdate()
    {
        if (!isTravelling)
            return;

        elapsedTime += Time.fixedDeltaTime;

        if (elapsedTime >= travelDuration)
        {
            isTravelling = false;
            return;
        }

        Vector3 movement = travelDirection * travelSpeed * Time.fixedDeltaTime;

        body.MovePosition(body.position + movement);
    }

    private bool TryGetValidEnemy(Collider other, out EnemyHealthScript enemy)
    {
        enemy = null;

        if (other == null)
            return false;

        if ((enemyLayers.value & (1 << other.gameObject.layer)) == 0)
            return false;

        enemy = other.GetComponentInParent<EnemyHealthScript>();

        return enemy != null && !enemy.IsDead;
    }

    private void RegisterEnemyCollider(EnemyHealthScript enemy, Collider enemyCollider)
    {
        if (!overlappingColliders.TryGetValue(enemy, out HashSet<Collider> colliders))
        {
            colliders = new HashSet<Collider>();
            overlappingColliders.Add(enemy, colliders);

            RegisterCloud(enemy, this);
        }

        colliders.Add(enemyCollider);
    }

    private static void RegisterCloud(EnemyHealthScript enemy, RestorativeGasCloudScript cloud)
    {
        if (!sharedDamage.TryGetValue(enemy, out SharedDamageState state))
        {
            state = new SharedDamageState {owner = cloud, enterTime = Time.time};

            sharedDamage.Add(enemy, state);
        }

        state.clouds.Add(cloud);
    }

    private static void UnregisterCloud(EnemyHealthScript enemy, RestorativeGasCloudScript cloud)
    {
        if (!sharedDamage.TryGetValue(enemy, out SharedDamageState state))
            return;

        state.clouds.Remove(cloud);

        if (state.owner == cloud)
        {
            state.owner = null;

            foreach (RestorativeGasCloudScript remainingCloud in state.clouds)
            {
                if (remainingCloud != null)
                {
                    state.owner = remainingCloud;
                    break;
                }
            }
        }

        if (state.clouds.Count == 0) sharedDamage.Remove(enemy);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!TryGetValidEnemy(other, out EnemyHealthScript enemy))
            return;

        RegisterEnemyCollider(enemy, other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!TryGetValidEnemy(other, out EnemyHealthScript enemy))
            return;

        RegisterEnemyCollider(enemy, other);

        if (!sharedDamage.TryGetValue(enemy, out SharedDamageState state))
            return;

        if (state.owner != this)
            return;

        if (!state.lingering && gasUpgrades != null && gasUpgrades.DamageMultiplier > 1f && Time.time - state.enterTime >= gasUpgrades.LingerDuration)
        {
            state.lingering = true;
        }

        if (Time.time < state.nextDamageTime)
            return;

        float damageMultiplier = state.lingering && gasUpgrades != null ? gasUpgrades.DamageMultiplier : 1f;

        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(damagePerTick * damageMultiplier));

        enemy.TakeDamage(finalDamage);

        state.nextDamageTime = Time.time + Mathf.Max(0.05f, damageInterval);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null)
            return;

        EnemyHealthScript enemy = other.GetComponentInParent<EnemyHealthScript>();

        if (enemy == null)
            return;

        if (!overlappingColliders.TryGetValue(enemy, out HashSet<Collider> colliders))
            return;

        colliders.Remove(other);

        if (colliders.Count == 0)
        {
            overlappingColliders.Remove(enemy);
            UnregisterCloud(enemy, this);
        }
    }

    private void OnDestroy()
    {
        List<EnemyHealthScript> enemies = new List<EnemyHealthScript>(overlappingColliders.Keys);

        foreach (EnemyHealthScript enemy in enemies)
        {
            if (enemy != null)
                UnregisterCloud(enemy, this);
        }

        overlappingColliders.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        SphereCollider sphere = GetComponent<SphereCollider>();

        if (sphere == null)
            return;

        Gizmos.color = Color.green;

        Vector3 center = transform.TransformPoint(sphere.center);

        float radius = sphere.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

        Gizmos.DrawWireSphere(center, radius);
    }
}