using UnityEngine;

/// <summary>
/// Controls the upgrades for the basic liquid spray weapon.
/// </summary>
public class BasicSprayUpgradeScript : MonoBehaviour
{
    [Header("Upgrade Activation")]
    [SerializeField] private bool improvedSpray = false;
    [SerializeField] private bool maximumHealthDamage = false;
    [SerializeField] private bool knockback = false;

    [Header("Improved Spray")]
    [SerializeField] private float damageMultiplier = 1.5f;
    [SerializeField] private float additionalRange = 2f;

    [Header("Maximum Health Damage")]
    [SerializeField, Range(0f, 1f)] private float bonusDamagePercentage = 0.1f;

    [Header("Knockback")]
    [SerializeField] private float knockbackThreshold = 0.6f;
    [SerializeField] private float knockbackSpeed = 4f;
    [SerializeField] private float knockbackDuration = 0.25f;

    public bool ImprovedSpray => improvedSpray;
    public bool MaximumHealthDamage => maximumHealthDamage;
    public bool Knockback => knockback;

    public float DamageMultiplier => improvedSpray ? damageMultiplier : 1f;
    public float AdditionalRange => improvedSpray ? additionalRange : 0f;

    public float BonusDamagePercentage => maximumHealthDamage ? bonusDamagePercentage : 0f;

    public float KnockbackThreshold => knockbackThreshold;
    public float KnockbackSpeed => knockbackSpeed;
    public float KnockbackDuration => knockbackDuration;

    public void ActivateImprovedSpray()
    {
        improvedSpray = true;
    }

    public void ActivateMaximumHealthDamage()
    {
        maximumHealthDamage = true;
    }

    public void ActivateKnockback()
    {
        knockback = true;
    }

    public void ResetUpgrades()
    {
        improvedSpray = false;
        maximumHealthDamage = false;
        knockback = false;
    }
}