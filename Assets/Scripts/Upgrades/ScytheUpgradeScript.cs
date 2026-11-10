
using UnityEngine;

/// <summary>
/// Stores and activates upgrades for the Scythe.
/// </summary>
public class ScytheUpgradeScript : MonoBehaviour
{
    [Header("Upgrade Activation")]
    [SerializeField] private bool increasedAttackRadius;
    [SerializeField] private bool increasingAttackSpeed;
    [SerializeField] private bool healthOnKill;

    [Header("Increased Attack Radius")]
    [SerializeField, Min(1f)]
    private float attackRadiusMultiplier = 1.25f;

    [Header("Increasing Attack Speed")]
    [SerializeField, Range(0f, 0.5f)]
    private float cooldownReductionPerHit = 0.05f;

    [SerializeField, Range(0f, 0.9f)]
    private float maximumCooldownReduction = 0.3f;

    [SerializeField, Min(0.1f)]
    private float hitStreakResetTime = 2f;

    [Header("Health on Kill")]
    [SerializeField, Min(1)]
    private int healthRestoredPerKill = 1;

    public bool IncreasedAttackRadius => increasedAttackRadius;
    public bool IncreasingAttackSpeed => increasingAttackSpeed;
    public bool HealthOnKill => healthOnKill;

    public float AttackRadiusMultiplier => increasedAttackRadius ? attackRadiusMultiplier : 1f;

    public float CooldownReductionPerHit => cooldownReductionPerHit;
    public float MaximumCooldownReduction => maximumCooldownReduction;
    public float HitStreakResetTime => hitStreakResetTime;

    public int HealthRestoredPerKill => healthRestoredPerKill;

    public void ActivateIncreasedAttackRadius()
    {
        increasedAttackRadius = true;
    }

    public void ActivateIncreasingAttackSpeed()
    {
        increasingAttackSpeed = true;
    }

    public void ActivateHealthOnKill()
    {
        healthOnKill = true;
    }

    public void ResetUpgrades()
    {
        increasedAttackRadius = false;
        increasingAttackSpeed = false;
        healthOnKill = false;
    }
}