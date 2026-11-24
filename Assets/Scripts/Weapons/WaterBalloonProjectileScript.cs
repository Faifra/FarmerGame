using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class WaterBalloonProjectileScript : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] private int explosionDamage = 25;
    [SerializeField] private float explosionRadius = 4f;
    [SerializeField] private GameObject explosionEffectPrefab;

    [Header("Projectile Physics")]
    [SerializeField] private float gravityMultiplier = 2.5f;
    [SerializeField] private float lifetime = 8f;

    [Header("Bounce Physics")]
    [SerializeField, Min(1f)] private float bounceSpeedMultiplier = 1.5f;
    [SerializeField, Min(0f)] private float bounceUpwardBoost = 5f;

    [Header("Damage Detection")]
    [SerializeField] private LayerMask enemyLayers = ~0;
    [SerializeField] private LayerMask wallLayers = ~0;

    private Rigidbody projectileBody;
    private Collider projectileCollider;

    private WaterBalloonUpgradeScript balloonUpgrades;

    private bool hasExploded;
    private bool hasBounced;

    private Renderer[] projectileRenderers;

    private readonly HashSet<Collider> ignoredEnemyColliders = new HashSet<Collider>();

    private void Awake()
    {
        projectileBody = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();
        projectileRenderers = GetComponentsInChildren<Renderer>();

        projectileBody.isKinematic = false;
        projectileBody.useGravity = false;
        projectileBody.detectCollisions = true;
        projectileBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        projectileCollider.isTrigger = false;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(
        Vector3 initialVelocity,
        Transform firingOwner,
        WaterBalloonUpgradeScript upgrades = null)
    {
        balloonUpgrades = upgrades;
        projectileBody.linearVelocity = initialVelocity;

        if (firingOwner == null)
            return;

        Collider[] ownerColliders =
            firingOwner.GetComponentsInChildren<Collider>();

        foreach (Collider ownerCollider in ownerColliders)
        {
            if (ownerCollider != null)
            {
                Physics.IgnoreCollision(
                    projectileCollider,
                    ownerCollider,
                    true
                );
            }
        }
    }

    private void FixedUpdate()
    {
        if (hasExploded)
            return;

        projectileBody.AddForce(
            Physics.gravity * gravityMultiplier,
            ForceMode.Acceleration
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded || collision == null)
            return;

        Collider hitCollider = collision.collider;

        if (hitCollider == null || collision.contactCount == 0)
            return;

        // First collision: bounce once.
        if (balloonUpgrades != null &&
            balloonUpgrades.RubberizedBalloon &&
            !hasBounced)
        {
            Bounce(collision);
            return;
        }

        // Any collision after the first bounce causes an explosion.
        EnemyHealthScript enemy =
            hitCollider.GetComponentInParent<EnemyHealthScript>();

        if (enemy != null)
            IgnoreEnemyCollisions(enemy);

        Explode(collision.GetContact(0).point);
    }

    private void Bounce(Collision collision)
    {
        hasBounced = true;

        ContactPoint contact = collision.GetContact(0);
        Vector3 normal = contact.normal;

        Vector3 incomingVelocity = projectileBody.linearVelocity;

        // Reflect the incoming velocity away from the collision surface.
        Vector3 reflectedVelocity = Vector3.Reflect(incomingVelocity, normal);

        // Increase the reflected speed to give the balloon more distance.
        reflectedVelocity *= bounceSpeedMultiplier;

        // Add an upward boost so the balloon doesn't immediately fall back down.
        reflectedVelocity += Vector3.up * bounceUpwardBoost;

        // Move the balloon slightly away from the collision surface.
        projectileBody.position += normal * 0.05f;

        projectileBody.linearVelocity = reflectedVelocity;

        StartCoroutine(TemporarilyIgnoreCollision(collision.collider));
    }

    private IEnumerator TemporarilyIgnoreCollision(Collider other)
    {
        if (other == null)
            yield break;

        Physics.IgnoreCollision(projectileCollider, other, true);

        yield return new WaitForSeconds(0.15f);

        if (projectileCollider != null && other != null)
        {
            Physics.IgnoreCollision(projectileCollider, other, false);
        }
    }

    private void IgnoreEnemyCollisions(EnemyHealthScript enemy)
    {
        Collider[] enemyColliders =
            enemy.GetComponentsInChildren<Collider>();

        foreach (Collider enemyCollider in enemyColliders)
        {
            if (enemyCollider == null ||
                ignoredEnemyColliders.Contains(enemyCollider))
                continue;

            Physics.IgnoreCollision(
                projectileCollider,
                enemyCollider,
                true
            );

            ignoredEnemyColliders.Add(enemyCollider);
        }
    }

    private void Explode(Vector3 position)
    {
        if (hasExploded)
            return;

        hasExploded = true;

        // Stop physics and hide the projectile immediately.
        projectileBody.linearVelocity = Vector3.zero;
        projectileBody.angularVelocity = Vector3.zero;
        projectileBody.isKinematic = true;
        projectileBody.detectCollisions = false;

        projectileCollider.enabled = false;

        foreach (Renderer projectileRenderer in projectileRenderers)
        {
            if (projectileRenderer != null)
                projectileRenderer.enabled = false;
        }

        SpawnExplosionEffect(position, 1f);
        DamageNearbyTargets(position);

        if (balloonUpgrades != null &&
            balloonUpgrades.ConcussiveAftershock)
        {
            StartCoroutine(SecondaryStunExplosion(position));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator SecondaryStunExplosion(Vector3 position)
    {
        yield return new WaitForSeconds(
            balloonUpgrades.SecondaryExplosionDelay
        );

        if (this == null)
            yield break;

        float secondaryRadius =
            explosionRadius *
            (balloonUpgrades != null
                ? balloonUpgrades.ExplosionRadiusMultiplier
                : 1f) *
            balloonUpgrades.SecondaryRadiusMultiplier;

        SpawnExplosionEffect(
            position,
            balloonUpgrades.SecondaryEffectScale
        );

        StunNearbyEnemies(position, secondaryRadius);

        Destroy(gameObject);
    }

    private void SpawnExplosionEffect(Vector3 position, float scale)
    {
        if (explosionEffectPrefab == null)
            return;

        GameObject effect = Instantiate(
            explosionEffectPrefab,
            position,
            Quaternion.identity
        );

        effect.transform.localScale *= scale;

        ParticleSystem[] particleSystems =
            effect.GetComponentsInChildren<ParticleSystem>(true);

        float effectLifetime = 0f;

        foreach (ParticleSystem particles in particleSystems)
        {
            particles.gameObject.SetActive(true);
            particles.Play(true);

            ParticleSystem.MainModule main = particles.main;

            float duration =
                main.duration +
                main.startDelay.constantMax +
                main.startLifetime.constantMax;

            effectLifetime = Mathf.Max(effectLifetime, duration);
        }

        if (effectLifetime <= 0f)
            effectLifetime = 3f;

        Destroy(effect, effectLifetime + 0.5f);
    }

    private float GetExplosionRadius()
    {
        float multiplier = balloonUpgrades != null
            ? balloonUpgrades.ExplosionRadiusMultiplier
            : 1f;

        return explosionRadius * multiplier;
    }

    private void DamageNearbyTargets(Vector3 position)
    {
        int detectionLayers = enemyLayers.value | wallLayers.value;

        Collider[] hits = Physics.OverlapSphere(
            position,
            GetExplosionRadius(),
            detectionLayers,
            QueryTriggerInteraction.Collide
        );

        HashSet<EnemyHealthScript> damagedEnemies =
            new HashSet<EnemyHealthScript>();

        HashSet<EnemyWallScript> damagedWalls =
            new HashSet<EnemyWallScript>();

        foreach (Collider hit in hits)
        {
            if (hit == null)
                continue;

            if ((enemyLayers.value & (1 << hit.gameObject.layer)) != 0)
            {
                EnemyHealthScript enemy =
                    hit.GetComponentInParent<EnemyHealthScript>();

                if (enemy != null &&
                    !enemy.IsDead &&
                    damagedEnemies.Add(enemy))
                {
                    enemy.TakeDamage(explosionDamage);
                }
            }

            if ((wallLayers.value & (1 << hit.gameObject.layer)) != 0)
            {
                EnemyWallScript wall =
                    hit.GetComponentInParent<EnemyWallScript>();

                if (wall != null && damagedWalls.Add(wall))
                    wall.TakeDamage(explosionDamage);
            }
        }
    }

    private void StunNearbyEnemies(Vector3 position, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(
            position,
            radius,
            enemyLayers,
            QueryTriggerInteraction.Collide
        );

        HashSet<EnemyHealthScript> stunnedEnemies =
            new HashSet<EnemyHealthScript>();

        foreach (Collider hit in hits)
        {
            if (hit == null)
                continue;

            EnemyHealthScript enemy =
                hit.GetComponentInParent<EnemyHealthScript>();

            if (enemy == null ||
                enemy.IsDead ||
                !stunnedEnemies.Add(enemy))
                continue;

            MoveTo enemyMovement = enemy.GetComponent<MoveTo>();

            if (enemyMovement != null)
                enemyMovement.Stun(balloonUpgrades.StunDuration);
        }
    }

    private void OnDrawGizmosSelected()
    {
        float radius = explosionRadius;

        if (balloonUpgrades != null)
            radius *= balloonUpgrades.ExplosionRadiusMultiplier;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);

        if (balloonUpgrades != null &&
            balloonUpgrades.ConcussiveAftershock)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(
                transform.position,
                radius * balloonUpgrades.SecondaryRadiusMultiplier
            );
        }
    }
}