using UnityEngine;

public class WaterBalloonUpgradeScript : MonoBehaviour
{
    [Header("Upgrade Activation")]
    [SerializeField] private bool expandedSplash;
    [SerializeField] private bool rubberizedBalloon;
    [SerializeField] private bool concussiveAftershock;

    [Header("Expanded Splash")]
    [SerializeField, Min(1f)] private float explosionRadiusMultiplier = 1.5f;

    [Header("Concussive Aftershock")]
    [SerializeField, Range(0.1f, 1f)] private float secondaryRadiusMultiplier = 0.5f;
    [SerializeField, Min(0f)] private float secondaryExplosionDelay = 0.25f;
    [SerializeField, Min(0f)] private float stunDuration = 1.5f;
    [SerializeField, Min(0f)] private float secondaryEffectScale = 0.6f;

    public float ExplosionRadiusMultiplier => expandedSplash ? explosionRadiusMultiplier : 1f;

    public bool RubberizedBalloon => rubberizedBalloon;
    public bool ConcussiveAftershock => concussiveAftershock;

    public float SecondaryRadiusMultiplier => secondaryRadiusMultiplier;
    public float SecondaryExplosionDelay => secondaryExplosionDelay;
    public float StunDuration => stunDuration;
    public float SecondaryEffectScale => secondaryEffectScale;

    public void ActivateExpandedSplash()
    {
        expandedSplash = true;
    }

    public void ActivateRubberizedBalloon()
    {
        rubberizedBalloon = true;
    }

    public void ActivateConcussiveAftershock()
    {
        concussiveAftershock = true;
    }

    public void ResetUpgrades()
    {
        expandedSplash = false;
        rubberizedBalloon = false;
        concussiveAftershock = false;
    }
}