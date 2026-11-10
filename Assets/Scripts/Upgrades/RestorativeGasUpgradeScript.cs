using UnityEngine;

public class RestorativeGasUpgradeScript : MonoBehaviour
{
    [Header("Upgrade Activation")]
    [SerializeField] private bool extendedCanister;
    [SerializeField] private bool rapidRefill;
    [SerializeField] private bool lingeringTonic;

    [Header("Extended Canister")]
    [SerializeField, Min(1f)] private float magazineSizeMultiplier = 1.2f;

    [Header("Rapid Refill")]
    [SerializeField, Range(0.1f, 1f)]
    private float reloadDurationMultiplier = 0.75f;

    [Header("Lingering Tonic")]
    [SerializeField, Min(1f)] private float damageMultiplier = 2f;
    [SerializeField, Min(0f)] private float lingerDuration = 1.5f;

    public float MagazineSizeMultiplier => extendedCanister ? magazineSizeMultiplier : 1f;

    public float ReloadDurationMultiplier => rapidRefill ? reloadDurationMultiplier : 1f;

    public float DamageMultiplier => lingeringTonic ? damageMultiplier : 1f;

    public float LingerDuration => lingerDuration;

    public void ActivateExtendedCanister()
    {
        extendedCanister = true;
    }

    public void ActivateRapidRefill()
    {
        rapidRefill = true;
    }

    public void ActivateLingeringTonic()
    {
        lingeringTonic = true;
    }

    public void ResetUpgrades()
    {
        extendedCanister = false;
        rapidRefill = false;
        lingeringTonic = false;
    }
}