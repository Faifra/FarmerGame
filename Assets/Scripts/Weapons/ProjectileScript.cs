
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField, Min(0.001f)] private float hitDetectionRadius = 0.05f;

    [Header("Detection")]
    [SerializeField] private LayerMask enemyLayers = ~0;
    [SerializeField] private LayerMask weakPointLayers = ~0;
    [SerializeField] private LayerMask wallLayers = ~0;

    private bool isSniperProjectile;
    private bool canRicochet;
    private bool hasRicocheted;
    private bool hasHit;

    private Rigidbody projectileBody;
    private Collider[] projectileColliders;

    private EnemyHealthScript lastHitEnemy;
    private Vector3 previousPosition;
    private bool hasInitializedPosition;

    private void Awake()
    {
        projectileBody = GetComponent<Rigidbody>();
        projectileColliders = GetComponentsInChildren<Collider>();
    }

    public void Initialize(bool sniperProjectile, bool ricochetEnabled)
    {
        isSniperProjectile = sniperProjectile;
        canRicochet = ricochetEnabled;
    }

    private void Start()
    {
        previousPosition = projectileBody != null ? projectileBody.position : transform.position;

        hasInitializedPosition = true;

        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (hasHit)
            return;

        Vector3 currentPosition = projectileBody != null ? projectileBody.position : transform.position;

        Vector3 movement = currentPosition - previousPosition;
        float distance = movement.magnitude;

        if (distance > 0.0001f)
        {
            Vector3 direction = movement / distance;

            RaycastHit[] hits = Physics.SphereCastAll(previousPosition, hitDetectionRadius, direction, distance, enemyLayers | weakPointLayers | wallLayers, QueryTriggerInteraction.Collide);

            ProcessHits(hits);
        }

        previousPosition = currentPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit)
            return;

        ProcessSingleCollider(other);
    }

    private void ProcessHits(RaycastHit[] hits)
    {
        if (hits.Length == 0)
            return;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        if (isSniperProjectile)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == null)
                    continue;

                if ((weakPointLayers.value & (1 << hit.collider.gameObject.layer)) == 0)
                    continue;

                WeakPointScript weakPoint = hit.collider.GetComponentInParent<WeakPointScript>();

                if (weakPoint == null || !weakPoint.IsUnlocked)
                    continue;

                EnemyHealthScript health = hit.collider.GetComponentInParent<EnemyHealthScript>();

                if (health == null || health.IsDead || health == lastHitEnemy)
                    continue;

                HitEnemy(health, weakPoint.DamageMultiplier);
                return;
            }
        }

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null)
                continue;

            if ((wallLayers.value & (1 << hit.collider.gameObject.layer)) != 0)
            {
                EnemyWallScript wall = hit.collider.GetComponentInParent<EnemyWallScript>();

                if (wall != null)
                {
                    HitWall(wall);
                    return;
                }
            }

            if ((enemyLayers.value & (1 << hit.collider.gameObject.layer)) == 0)
                continue;

            EnemyHealthScript health = hit.collider.GetComponentInParent<EnemyHealthScript>();

            if (health == null || health.IsDead || health == lastHitEnemy)
                continue;

            HitEnemy(health, 1f);
            return;
        }
    }

    private void ProcessSingleCollider(Collider other)
    {
        if ((wallLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            EnemyWallScript wall = other.GetComponentInParent<EnemyWallScript>();

            if (wall != null)
            {
                HitWall(wall);
                return;
            }
        }

        EnemyHealthScript health = other.GetComponentInParent<EnemyHealthScript>();

        if (health == null || health.IsDead || health == lastHitEnemy)
            return;

        float damageMultiplier = 1f;

        if (isSniperProjectile && (weakPointLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            WeakPointScript weakPoint = other.GetComponentInParent<WeakPointScript>();

            if (weakPoint != null && weakPoint.IsUnlocked)
                damageMultiplier = weakPoint.DamageMultiplier;
        }

        HitEnemy(health, damageMultiplier);
    }

    private void HitWall(EnemyWallScript wall)
    {
        if (hasHit || wall == null)
            return;

        hasHit = true;

        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(damage));

        wall.TakeDamage(finalDamage);

        Destroy(gameObject);
    }

    private void HitEnemy(EnemyHealthScript health, float damageMultiplier)
    {
        if (hasHit || health == null || health.IsDead)
            return;

        hasHit = true;
        lastHitEnemy = health;

        bool shouldRicochet = canRicochet && !hasRicocheted;

        if (shouldRicochet)
        {
            hasRicocheted = true;
            IgnoreEnemyCollisions(health);
        }

        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(damage * damageMultiplier));

        health.TakeDamage(finalDamage);

        if (shouldRicochet)
        {
            hasHit = false;
            Ricochet(health);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void IgnoreEnemyCollisions(EnemyHealthScript enemy)
    {
        if (enemy == null)
            return;

        Collider[] enemyColliders = enemy.GetComponentsInChildren<Collider>();

        foreach (Collider projectileCollider in projectileColliders)
        {
            if (projectileCollider == null)
                continue;

            foreach (Collider enemyCollider in enemyColliders)
            {
                if (enemyCollider != null)
                {
                    Physics.IgnoreCollision(projectileCollider, enemyCollider, true);
                }
            }
        }
    }

    private void Ricochet(EnemyHealthScript previouslyHitEnemy)
    {
        if (projectileBody == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 incomingDirection = projectileBody.linearVelocity.normalized;

        if (incomingDirection.sqrMagnitude < 0.001f)
            incomingDirection = transform.forward;

        EnemyHealthScript[] enemies = FindObjectsByType<EnemyHealthScript>(FindObjectsSortMode.None);

        EnemyHealthScript closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (EnemyHealthScript enemy in enemies)
        {
            if (enemy == null || enemy.IsDead || enemy == previouslyHitEnemy)
                continue;

            float distance = (enemy.transform.position - transform.position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        Vector3 ricochetDirection = closestEnemy != null ? (closestEnemy.transform.position - transform.position).normalized : -incomingDirection;

        float speed = projectileBody.linearVelocity.magnitude;

        if (speed < 0.001f)
            speed = 1f;

        projectileBody.linearVelocity = ricochetDirection * speed;
        transform.rotation = Quaternion.LookRotation(ricochetDirection);

        previousPosition = projectileBody.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitDetectionRadius);
    }
}