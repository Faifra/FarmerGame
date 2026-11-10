using UnityEngine;

public class SprinklerTrapUpgradeScript : MonoBehaviour
{
    [Header("Upgrade Activation")]
    [SerializeField] private bool pressurizedSpray;
    [SerializeField] private bool rapidRotation;
    [SerializeField] private bool reinforcedBarriers;

    [Header("Pressurized Spray")]
    [SerializeField, Min(1f)] private float damageMultiplier = 1.05f;
    [SerializeField, Min(1f)] private float rangeMultiplier = 1.05f;

    [Header("Rapid Rotation")]
    [SerializeField, Min(1f)] private float rotationSpeedMultiplier = 1.25f;

    [Header("Reinforced Barriers")]
    [SerializeField, Min(1f)] private float wallHealthMultiplier = 1.5f;

    public float DamageMultiplier => pressurizedSpray ? damageMultiplier : 1f;
    public float RangeMultiplier => pressurizedSpray ? rangeMultiplier : 1f;
    public float RotationSpeedMultiplier => rapidRotation ? rotationSpeedMultiplier : 1f;
    public float WallHealthMultiplier => reinforcedBarriers ? wallHealthMultiplier : 1f;

    public void ActivatePressurizedSpray()
    {
        pressurizedSpray = true;
    }

    public void ActivateRapidRotation()
    {
        rapidRotation = true;
    }

    public void ActivateReinforcedBarriers()
    {
        reinforcedBarriers = true;
    }

    public void ResetUpgrades()
    {
        pressurizedSpray = false;
        rapidRotation = false;
        reinforcedBarriers = false;
    }
}