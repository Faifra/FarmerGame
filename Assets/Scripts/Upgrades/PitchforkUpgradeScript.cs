using UnityEngine;

public class PitchforkUpgradeScript : MonoBehaviour
{
    [Header("Upgrade Activation")]
    [SerializeField] private bool bonusDamage = false;
    [SerializeField] private bool enemySlowdown = false;
    [SerializeField] private bool bonusResources = false;

    [Header("Bonus Damage")]
    [SerializeField, Range(0f, 1f)]
    private float bonusDamagePercentage = 0.15f;

    [Header("Enemy Slowdown")]
    [SerializeField, Range(0.1f, 1f)]
    private float slowMultiplier = 0.9f;

    [SerializeField, Min(0f)]
    private float slowDuration = 3f;

    [Header("Bonus Resources")]
    [SerializeField, Range(0f, 1f)]
    private float bonusDropPercentage = 0.05f;

    public bool BonusDamage => bonusDamage;
    public bool EnemySlowdown => enemySlowdown;
    public bool BonusResources => bonusResources;

    public float BonusDamagePercentage => bonusDamagePercentage;
    public float SlowMultiplier => slowMultiplier;
    public float SlowDuration => slowDuration;
    public float BonusDropPercentage => bonusDropPercentage;

    public void ActivateBonusDamage()
    {
        bonusDamage = true;
    }

    public void ActivateEnemySlowdown()
    {
        enemySlowdown = true;
    }

    public void ActivateBonusResources()
    {
        bonusResources = true;
    }

    public void ResetUpgrades()
    {
        bonusDamage = false;
        enemySlowdown = false;
        bonusResources = false;
    }
}