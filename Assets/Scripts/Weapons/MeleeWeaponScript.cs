
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles animated melee attacks, weapon hitboxes, damage, and knockback.
/// </summary>
public class MeleeWeaponScript : WeaponScript
{
    public enum AttackStyle
    {
        Thrust, Swing
    }

    [Header("Attack Style")]
    [SerializeField] private AttackStyle attackStyle = AttackStyle.Thrust;

    [Header("Hitbox")]
    [SerializeField] private Collider weaponHitbox;
    [SerializeField] private LayerMask damageLayers;

    [Header("Damage")]
    [SerializeField] private int damage = 1;

    [Header("Animation")]
    [SerializeField] private float attackDuration = 0.3f;
    [SerializeField] private float returnDuration = 0.15f;

    [Header("Thrust")]
    [SerializeField] private float thrustDistance = 1.2f;
    [SerializeField] private Vector3 thrustDirection = Vector3.forward;

    [Header("Swing")]
    [SerializeField] private float swingAngle = 100f;
    [SerializeField] private float swingDistance = 0.3f;
    [SerializeField] private bool reverseSwingDirection = false;
    [SerializeField] private Transform swingReference;

    [Header("Knockback")]
    [SerializeField] private float knockbackSpeed = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    [Header("Pitchfork Charge")]
    [SerializeField] private bool enableChargeAttack = false;
    [SerializeField, Min(0.1f)] private float chargeDuration = 1.5f;
    [SerializeField, Min(0f)] private float chargePullbackDistance = 0.4f;
    [SerializeField, Min(0f)] private float wobbleAngle = 3f;
    [SerializeField, Min(0f)] private float wobbleFrequency = 12f;

    [Header("Fully Charged Attack")]
    [SerializeField, Min(1f)] private float chargedDamageMultiplier = 2f;
    [SerializeField, Min(1f)] private float chargedKnockbackSpeedMultiplier = 2f;
    [SerializeField, Min(1f)] private float chargedKnockbackDurationMultiplier = 1.5f;

    [Header("Upgrades")]
    [SerializeField] private PitchforkUpgradeScript pitchforkUpgrades;
    [SerializeField] private ScytheUpgradeScript scytheUpgrades;

    private Vector3 restingLocalPosition;
    private Quaternion restingLocalRotation;

    private Vector3 originalHitboxScale;
    private float originalAttackCooldown;

    private Coroutine attackCoroutine;
    private bool attackInProgress;

    private bool isCharging;
    private float chargeStartTime;
    private bool currentAttackFullyCharged;
    private bool chargedHitRegistered;

    private int consecutiveHits;
    private float lastHitTime;

    private readonly HashSet<EnemyHealthScript> hitEnemies = new();
    private readonly HashSet<EnemyWallScript> hitWalls = new();

    private void Awake()
    {
        restingLocalPosition = transform.localPosition;
        restingLocalRotation = transform.localRotation;

        originalAttackCooldown = attackCooldown;

        if (swingReference == null)
            swingReference = transform.parent;

        if (weaponHitbox != null)
        {
            weaponHitbox.enabled = false;
            originalHitboxScale = weaponHitbox.transform.localScale;
        }
    }

    private void Update()
    {
        if (scytheUpgrades != null && scytheUpgrades.IncreasingAttackSpeed)
        {
            if (consecutiveHits > 0 && Time.time - lastHitTime > scytheUpgrades.HitStreakResetTime)
            {
                consecutiveHits = 0;
                attackCooldown = originalAttackCooldown;
            }
        }

        if (isCharging)
        {
            UpdateChargeAnimation();
        }
    }

    public override void Attack()
    {
        if (enableChargeAttack)
        {
            BeginChargeAttack();
            return;
        }

        PerformQuickAttack();
    }

    public void BeginChargeAttack()
    {
        if (!enableChargeAttack || isCharging || attackInProgress || !CanAttack())
            return;

        isCharging = true;
        chargeStartTime = Time.time;
    }

    public void ReleaseChargeAttack()
    {
        if (!enableChargeAttack || !isCharging)
            return;

        float chargeTime = Time.time - chargeStartTime;

        currentAttackFullyCharged = chargeTime >= chargeDuration;

        isCharging = false;

        PerformAttack();
    }

    private void PerformQuickAttack()
    {
        if (attackInProgress || isCharging || !CanAttack())
            return;

        currentAttackFullyCharged = false;
        StartAttackCooldown();

        attackCoroutine = StartCoroutine(PerformAttackAnimation());
    }

    private void PerformAttack()
    {
        if (attackInProgress || !CanAttack())
            return;

        StartAttackCooldown();

        attackCoroutine = StartCoroutine(PerformAttackAnimation());
    }

    private void UpdateChargeAnimation()
    {
        float chargeProgress = Mathf.Clamp01((Time.time - chargeStartTime) / chargeDuration);

        Vector3 direction = thrustDirection.sqrMagnitude > 0.001f ? thrustDirection.normalized : Vector3.forward;

        Vector3 pullbackPosition = restingLocalPosition - direction * (chargePullbackDistance * chargeProgress);

        transform.localPosition = pullbackPosition;

        float wobble = Mathf.Sin(Time.time * wobbleFrequency) * wobbleAngle * chargeProgress;

        transform.localRotation = restingLocalRotation * Quaternion.AngleAxis(wobble, Vector3.forward);
    }

    private IEnumerator PerformAttackAnimation()
    {
        attackInProgress = true;
        hitEnemies.Clear();
        hitWalls.Clear();
        chargedHitRegistered = false;

        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;

        Vector3 attackPosition = startPosition;

        if (attackStyle == AttackStyle.Thrust)
        {
            Vector3 direction = thrustDirection.sqrMagnitude > 0.001f ? thrustDirection.normalized : Vector3.forward;

            attackPosition = restingLocalPosition + direction * thrustDistance;
        }

        if (weaponHitbox != null)
        {
            if (scytheUpgrades != null)
            {
                weaponHitbox.transform.localScale = originalHitboxScale * scytheUpgrades.AttackRadiusMultiplier;
            }

            weaponHitbox.enabled = true;
        }

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, attackDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = EaseInOut(t);

            if (attackStyle == AttackStyle.Thrust)
            {
                transform.localPosition = Vector3.Lerp(startPosition, attackPosition, EaseOut(t));

                transform.localRotation = startRotation;
            }
            else
            {
                float direction = reverseSwingDirection ? 1f : -1f;

                float angle = Mathf.Lerp(swingAngle * 0.5f, -swingAngle * 0.5f, easedT) * direction;

                Vector3 worldUp = swingReference != null ? swingReference.up : Vector3.up;

                Quaternion worldSwing = Quaternion.AngleAxis(angle, worldUp);

                Quaternion restingWorldRotation = swingReference != null ? swingReference.rotation * startRotation : startRotation;

                Quaternion targetWorldRotation = worldSwing * restingWorldRotation;

                if (transform.parent != null)
                {
                    transform.localRotation = Quaternion.Inverse(transform.parent.rotation) * targetWorldRotation;
                }
                else
                {
                    transform.rotation = targetWorldRotation;
                }

                transform.localPosition = startPosition + Vector3.forward * (Mathf.Sin(t * Mathf.PI) * swingDistance);
            }

            yield return null;
        }

        if (weaponHitbox != null)
        {
            weaponHitbox.enabled = false;
            weaponHitbox.transform.localScale = originalHitboxScale;
        }

        Vector3 currentPosition = transform.localPosition;
        Quaternion currentRotation = transform.localRotation;

        elapsed = 0f;
        duration = Mathf.Max(0.01f, returnDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = EaseInOut(Mathf.Clamp01(elapsed / duration));

            transform.localPosition = Vector3.Lerp(currentPosition, startPosition, t);

            transform.localRotation = Quaternion.Slerp(currentRotation, startRotation, t);

            yield return null;
        }

        transform.localPosition = restingLocalPosition;
        transform.localRotation = restingLocalRotation;

        attackInProgress = false;
        attackCoroutine = null;
        currentAttackFullyCharged = false;
    }

    public void TryHit(Collider other)
    {
        if (!attackInProgress || weaponHitbox == null || !weaponHitbox.enabled)
            return;

        if (currentAttackFullyCharged && chargedHitRegistered)
            return;

        if ((damageLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        EnemyWallScript wall = other.GetComponentInParent<EnemyWallScript>();

        if (wall != null)
        {

            if (hitWalls.Add(wall))
                wall.TakeDamage(damage);

            return;
        }

        EnemyHealthScript health = other.GetComponentInParent<EnemyHealthScript>();

        if (health == null || !hitEnemies.Add(health))
            return;

        Vector3 knockbackDirection = health.transform.position - transform.position;

        knockbackDirection.y = 0f;

        bool wasAlive = !health.IsDead;

        int finalDamage = damage;
        float finalKnockbackSpeed = knockbackSpeed;
        float finalKnockbackDuration = knockbackDuration;

        int bonusDrops = 0;

        if (currentAttackFullyCharged)
        {
            finalDamage = Mathf.RoundToInt(damage * chargedDamageMultiplier);

            finalKnockbackSpeed *= chargedKnockbackSpeedMultiplier;
            finalKnockbackDuration *= chargedKnockbackDurationMultiplier;

            if (pitchforkUpgrades != null)
            {
                if (pitchforkUpgrades.BonusDamage)
                {
                    int bonusDamage = Mathf.RoundToInt(health.MaxHealth * pitchforkUpgrades.BonusDamagePercentage);

                    finalDamage += bonusDamage;
                }

                if (pitchforkUpgrades.BonusResources)
                {
                    bonusDrops = Mathf.RoundToInt(health.MaxHealth * pitchforkUpgrades.BonusDropPercentage);
                }
            }
        }

        if (knockbackDirection.sqrMagnitude > 0.001f)
        {
            knockbackDirection.Normalize();
        }

        if (currentAttackFullyCharged && pitchforkUpgrades != null && pitchforkUpgrades.BonusResources)
        {
            health.TakeDamageFromChargedThrust(finalDamage, knockbackDirection, finalKnockbackSpeed, finalKnockbackDuration, bonusDrops);
        }
        else if (knockbackDirection.sqrMagnitude > 0.001f)
        {
            health.TakeDamage(finalDamage, knockbackDirection, finalKnockbackSpeed, finalKnockbackDuration);
        }
        else
        {
            health.TakeDamage(finalDamage);
        }

        if (currentAttackFullyCharged && pitchforkUpgrades != null && pitchforkUpgrades.EnemySlowdown && !health.IsDead)
        {
            MoveTo enemyMovement = health.GetComponent<MoveTo>();

            if (enemyMovement != null)
            {
                enemyMovement.ApplySlow(pitchforkUpgrades.SlowMultiplier, pitchforkUpgrades.SlowDuration);
            }
        }

        if (currentAttackFullyCharged && wasAlive)
        {
            chargedHitRegistered = true;
        }

        if (scytheUpgrades == null)
            return;

        if (scytheUpgrades.IncreasingAttackSpeed && wasAlive)
        {
            if (Time.time - lastHitTime > scytheUpgrades.HitStreakResetTime)
            {
                consecutiveHits = 0;
            }

            consecutiveHits++;
            lastHitTime = Time.time;

            float cooldownReduction = Mathf.Min(consecutiveHits * scytheUpgrades.CooldownReductionPerHit, scytheUpgrades.MaximumCooldownReduction);

            attackCooldown = originalAttackCooldown * (1f - cooldownReduction);
        }

        if (scytheUpgrades.HealthOnKill && wasAlive && health.IsDead)
        {
            GameManagerScript.RestoreHP(scytheUpgrades.HealthRestoredPerKill);
        }
    }

    private float EaseOut(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    private float EaseInOut(float t)
    {
        return t * t * (3f - 2f * t);
    }

    private void OnDisable()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        if (weaponHitbox != null)
        {
            weaponHitbox.enabled = false;
            weaponHitbox.transform.localScale = originalHitboxScale;
        }

        isCharging = false;
        attackInProgress = false;
        currentAttackFullyCharged = false;
        chargedHitRegistered = false;

        transform.localPosition = restingLocalPosition;
        transform.localRotation = restingLocalRotation;
    }
}