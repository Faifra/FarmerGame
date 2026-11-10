using UnityEngine;

public class MilkChurnUpgradeScript : MonoBehaviour
{
    [Header("Upgrade Activation")]
    [SerializeField] private bool rapidChurning;
    [SerializeField] private bool largerSplatter;
    [SerializeField] private bool unstableMixture;

    [Header("Rapid Churning")]
    [SerializeField, Range(0.1f, 1f)]
    private float reloadDurationMultiplier = 0.75f;

    [Header("Larger Splatter")]
    [SerializeField, Min(1f)]
    private float popRadiusMultiplier = 2f;

    [Header("Unstable Mixture")]
    [SerializeField, Range(0.1f, 1f)]
    private float overloadThresholdMultiplier = 0.7f;

    public float ReloadDurationMultiplier => rapidChurning ? reloadDurationMultiplier : 1f;

    public float PopRadiusMultiplier => largerSplatter ? popRadiusMultiplier : 1f;

    public float OverloadThresholdMultiplier => unstableMixture ? overloadThresholdMultiplier : 1f;

    public void ActivateRapidChurning()
    {
        rapidChurning = true;
    }

    public void ActivateLargerSplatter()
    {
        largerSplatter = true;
    }

    public void ActivateUnstableMixture()
    {
        unstableMixture = true;
    }

    public void ResetUpgrades()
    {
        rapidChurning = false;
        largerSplatter = false;
        unstableMixture = false;
    }
}