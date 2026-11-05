
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
        Thrust,
        Swing
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

    private Vector3 restingLocalPosition;
    private Quaternion restingLocalRotation;
    private Coroutine attackCoroutine;
    private bool attackInProgress;

    private readonly HashSet<EnemyHealthScript> hitEnemies = new();

    private void Awake()
    {
        restingLocalPosition = transform.localPosition;
        restingLocalRotation = transform.localRotation;

        if (swingReference == null)
            swingReference = transform.parent;

        if (weaponHitbox != null)
            weaponHitbox.enabled = false;
    }

    public override void Attack()
    {
        if (attackInProgress || !CanAttack())
            return;

        StartAttackCooldown();
        attackCoroutine = StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        attackInProgress = true;
        hitEnemies.Clear();

        Vector3 startPosition = restingLocalPosition;
        Quaternion startRotation = restingLocalRotation;

        Vector3 attackPosition = startPosition;

        if (attackStyle == AttackStyle.Thrust)
        {
            Vector3 direction = thrustDirection.sqrMagnitude > 0.001f ? thrustDirection.normalized : Vector3.forward;

            attackPosition += direction * thrustDistance;
        }

        if (weaponHitbox != null)
            weaponHitbox.enabled = true;

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
            weaponHitbox.enabled = false;

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

        transform.localPosition = startPosition;
        transform.localRotation = startRotation;

        attackInProgress = false;
        attackCoroutine = null;
    }

    public void TryHit(Collider other)
    {
        if (!attackInProgress || weaponHitbox == null ||
            !weaponHitbox.enabled)
            return;

        if ((damageLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        EnemyHealthScript health = other.GetComponentInParent<EnemyHealthScript>();

        if (health == null || !hitEnemies.Add(health))
            return;

        Vector3 knockbackDirection = health.transform.position - transform.position;

        knockbackDirection.y = 0f;

        if (knockbackDirection.sqrMagnitude > 0.001f)
        {
            knockbackDirection.Normalize();

            health.TakeDamage(damage, knockbackDirection, knockbackSpeed, knockbackDuration);
        }
        else
        {
            health.TakeDamage(damage);
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
            weaponHitbox.enabled = false;

        attackInProgress = false;
        transform.localPosition = restingLocalPosition;
        transform.localRotation = restingLocalRotation;
    }
}
