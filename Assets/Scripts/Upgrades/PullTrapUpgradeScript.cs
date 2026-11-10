using UnityEngine;

public class PullTrapUpgradeScript : MonoBehaviour
{
    [Header("Upgrade Activation")]
    [SerializeField] private bool expandedVacuum;
    [SerializeField] private bool extendedReach;
    [SerializeField] private bool forcefulLaunch;

    [Header("Overwhelming Force")]
    [SerializeField, Min(3)] private int maximumEnemies = 5;

    [Header("Extended Reach")]
    [SerializeField, Min(1f)] private float pullRangeMultiplier = 1.2f;
    [SerializeField, Range(0.1f, 1f)] private float pullDurationMultiplier = 0.75f;

    [Header("Forceful Launch")]
    [SerializeField, Min(1f)] private float launchDistanceMultiplier = 1.5f;
    [SerializeField, Min(1f)] private float damageMultiplier = 1.5f;

    public int MaximumEnemies => expandedVacuum ? maximumEnemies : 3;
    public float PullRangeMultiplier => extendedReach ? pullRangeMultiplier : 1f;
    public float PullDurationMultiplier => extendedReach ? pullDurationMultiplier : 1f;
    public float LaunchDistanceMultiplier => forcefulLaunch ? launchDistanceMultiplier : 1f;
    public float DamageMultiplier => forcefulLaunch ? damageMultiplier : 1f;

    public void ActivateExpandedVacuum()
    {
        expandedVacuum = true;
    }

    public void ActivateExtendedReach()
    {
        extendedReach = true;
    }

    public void ActivateForcefulLaunch()
    {
        forcefulLaunch = true;
    }

    public void ResetUpgrades()
    {
        expandedVacuum = false;
        extendedReach = false;
        forcefulLaunch = false;
    }
}