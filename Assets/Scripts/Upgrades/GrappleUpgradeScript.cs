
using UnityEngine;

public class GrappleUpgradeScript : MonoBehaviour
{
    [Header("Upgrade Activation")]
    [SerializeField] private bool reinforcedGraft;
    [SerializeField] private bool corrosiveGraft;
    [SerializeField] private bool grapplerMomentum;

    [Header("Reinforced Graft")]
    [SerializeField, Min(1f)] private float pullSpeedMultiplier = 1.25f;
    [SerializeField, Range(0.1f, 1f)] private float cooldownMultiplier = 0.75f;

    [Header("Corrosive Graft")]
    [SerializeField, Min(1f)] private float vulnerabilityDurationMultiplier = 2f;

    [Header("Grappler Momentum")]
    [SerializeField, Range(0f, 1f)] private float healthRestoredPercentage = 0.03f;
    [SerializeField, Min(1f)] private float movementSpeedMultiplier = 1.1f;
    [SerializeField, Min(0f)] private float movementSpeedDuration = 5f;

    public float PullSpeedMultiplier => reinforcedGraft ? pullSpeedMultiplier : 1f;

    public float CooldownMultiplier => reinforcedGraft ? cooldownMultiplier : 1f;

    public float VulnerabilityDurationMultiplier => corrosiveGraft ? vulnerabilityDurationMultiplier : 1f;

    public bool GrapplerMomentum => grapplerMomentum;
    public float HealthRestoredPercentage => healthRestoredPercentage;
    public float MovementSpeedMultiplier => movementSpeedMultiplier;
    public float MovementSpeedDuration => movementSpeedDuration;

    public void ActivateReinforcedGraft()
    {
        reinforcedGraft = true;
    }

    public void ActivateCorrosiveGraft()
    {
        corrosiveGraft = true;
    }

    public void ActivateGrapplerMomentum()
    {
        grapplerMomentum = true;
    }

    public void ResetUpgrades()
    {
        reinforcedGraft = false;
        corrosiveGraft = false;
        grapplerMomentum = false;
    }
}