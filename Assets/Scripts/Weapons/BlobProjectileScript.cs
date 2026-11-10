
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class BlobProjectileScript : MonoBehaviour
{
    [Header("Initial Impact")]
    [SerializeField] private int impactDamage = 1;

    [Header("Pop Damage")]
    [SerializeField] private int popDamage = 8;
    [SerializeField] private int overloadDamage = 25;
    [SerializeField] private float popRadius = 2f;
    [SerializeField] private float fuseDuration = 3f;

    [Header("Projectile")]
    [SerializeField] private float lifetime = 10f;

    [Header("Detection")]
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private LayerMask environmentLayers;

    [Header("Effects")]
    [SerializeField] private GameObject popEffectPrefab;

    private Rigidbody projectileBody;
    private SphereCollider projectileCollider;

    private EnemyHealthScript attachedEnemy;
    private BlobStackScript blobStack;

    private MilkChurnUpgradeScript milkChurnUpgrades;

    private readonly HashSet<Collider> ownerColliders = new HashSet<Collider>();

    private Vector3 previousPosition;

    private bool hasStuck;
    private bool hasPopped;

    private void Awake()
    {
        projectileBody = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<SphereCollider>();

        projectileBody.isKinematic = false;
        projectileBody.useGravity = true;
        projectileBody.constraints = RigidbodyConstraints.None;
        projectileBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        projectileCollider.isTrigger = true;
    }

    private void Start()
    {
        previousPosition = projectileBody.position;
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector3 initialVelocity, Transform firingOwner, MilkChurnUpgradeScript upgrades = null)
    {
        milkChurnUpgrades = upgrades;

        projectileBody.isKinematic = false;
        projectileBody.useGravity = true;
        projectileBody.linearVelocity = initialVelocity;
        projectileBody.WakeUp();

        previousPosition = projectileBody.position;

        if (firingOwner == null)
            return;

        Collider[] colliders = firingOwner.GetComponentsInChildren<Collider>();

        foreach (Collider ownerCollider in colliders)
        {
            if (ownerCollider == null)
                continue;

            ownerColliders.Add(ownerCollider);

            Physics.IgnoreCollision(projectileCollider, ownerCollider, true);
        }
    }

    private void FixedUpdate()
    {
        if (hasStuck || hasPopped)
            return;

        Vector3 currentPosition = projectileBody.position;
        Vector3 movement = currentPosition - previousPosition;
        float distance = movement.magnitude;

        if (distance > 0.0001f)
        {
            Vector3 direction = movement / distance;

            float castRadius = projectileCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

            RaycastHit[] hits = Physics.SphereCastAll(previousPosition, castRadius, direction, distance, enemyLayers | environmentLayers, QueryTriggerInteraction.Collide);

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == null)
                    continue;

                if (ownerColliders.Contains(hit.collider))
                    continue;

                if (hit.collider == projectileCollider)
                    continue;

                Vector3 impactCenter = previousPosition + direction * hit.distance;

                if (TryStickToTarget(hit.collider, impactCenter))
                    return;
            }
        }

        previousPosition = currentPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasStuck || hasPopped || other == null)
            return;

        if (ownerColliders.Contains(other))
            return;

        TryStickToTarget(other, transform.position);
    }

    private bool TryStickToTarget(Collider hitCollider, Vector3 impactPosition)
    {
        if (hitCollider == null || hasStuck || hasPopped)
            return false;

        if (ownerColliders.Contains(hitCollider))
            return false;

        if (IsLayerIncluded(hitCollider.gameObject.layer, enemyLayers))
        {
            EnemyHealthScript enemy = hitCollider.GetComponentInParent<EnemyHealthScript>();

            if (enemy != null)
            {
                if (hitCollider == enemy.BodyCollider && !enemy.IsDead)
                {
                    StickToEnemy(enemy, impactPosition);
                    return true;
                }

                return false;
            }
        }

        if (IsLayerIncluded(hitCollider.gameObject.layer, environmentLayers))
        {
            StickToEnvironment(hitCollider, impactPosition);
            return true;
        }

        return false;
    }

    private bool IsLayerIncluded(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    private void StickToEnemy(EnemyHealthScript enemy, Vector3 impactPosition)
    {
        if (hasStuck || enemy == null || enemy.IsDead)
            return;

        hasStuck = true;

        transform.position = impactPosition;
        StopProjectileMovement();

        attachedEnemy = enemy;
        transform.SetParent(enemy.transform, true);

        enemy.TakeDamage(impactDamage);

        blobStack = enemy.GetComponent<BlobStackScript>();

        if (blobStack == null)
            blobStack = enemy.gameObject.AddComponent<BlobStackScript>();

        blobStack.RegisterBlob(this, GetOverloadThresholdMultiplier());

        StartCoroutine(PopAfterDelay());
    }

    private void StickToEnvironment(Collider surface, Vector3 impactPosition)
    {
        if (hasStuck || surface == null)
            return;

        hasStuck = true;

        transform.position = impactPosition;
        StopProjectileMovement();

        transform.SetParent(surface.transform, true);

        StartCoroutine(PopAfterDelay());
    }

    private void StopProjectileMovement()
    {
        projectileBody.linearVelocity = Vector3.zero;
        projectileBody.angularVelocity = Vector3.zero;
        projectileBody.isKinematic = true;

        projectileCollider.enabled = false;
    }

    private IEnumerator PopAfterDelay()
    {
        yield return new WaitForSeconds(fuseDuration);

        if (!hasPopped)
            Pop(false);
    }

    public void Pop(bool overloaded)
    {
        if (hasPopped)
            return;

        hasPopped = true;

        if (blobStack != null)
            blobStack.UnregisterBlob(this);

        int damage = overloaded ? overloadDamage : popDamage;

        SpawnPopEffect();
        DamageNearbyEnemies(damage);

        Destroy(gameObject);
    }

    private void DamageNearbyEnemies(int damage)
    {
        HashSet<EnemyHealthScript> damagedEnemies = new HashSet<EnemyHealthScript>();

        if (attachedEnemy != null && !attachedEnemy.IsDead)
        {
            attachedEnemy.TakeDamage(damage);
            damagedEnemies.Add(attachedEnemy);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, GetPopRadius(), enemyLayers, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            if (hit == null)
                continue;

            EnemyHealthScript enemy = hit.GetComponentInParent<EnemyHealthScript>();

            if (enemy == null || enemy.IsDead)
                continue;

            if (damagedEnemies.Add(enemy))
                enemy.TakeDamage(damage);
        }
    }

    private void SpawnPopEffect()
    {
        if (popEffectPrefab == null)
            return;

        GameObject effect = Instantiate(popEffectPrefab, transform.position, Quaternion.identity);

        ParticleSystem[] particleSystems = effect.GetComponentsInChildren<ParticleSystem>(true);

        float effectLifetime = 3f;

        foreach (ParticleSystem particles in particleSystems)
        {
            particles.gameObject.SetActive(true);
            particles.Play(true);

            ParticleSystem.MainModule main = particles.main;

            float duration = main.duration + main.startDelay.constantMax + main.startLifetime.constantMax;

            effectLifetime = Mathf.Max(effectLifetime, duration);
        }

        Destroy(effect, effectLifetime + 0.5f);
    }

    private float GetPopRadius()
    {
        float multiplier = milkChurnUpgrades != null ? milkChurnUpgrades.PopRadiusMultiplier : 1f;

        return popRadius * multiplier;
    }

    private float GetOverloadThresholdMultiplier()
    {
        return milkChurnUpgrades != null ? milkChurnUpgrades.OverloadThresholdMultiplier : 1f;
    }

    private void OnDestroy()
    {
        if (blobStack != null)
            blobStack.UnregisterBlob(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, GetPopRadius());
    }
}