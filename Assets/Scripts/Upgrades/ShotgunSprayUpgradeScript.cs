using UnityEngine;

/// <summary>
/// Controls the upgrades for the shotgun spray weapon.
/// </summary>
public class ShotgunSprayUpgradeScript : MonoBehaviour
{
    [Header("Upgrade Activation")]
    [SerializeField] private bool doubleBurst = false;
    [SerializeField] private bool fasterReload = false;
    [SerializeField] private bool closeRangeDamage = false;

    [Header("Double Burst")]
    [SerializeField, Min(1)] private int burstsPerPress = 2;

    [Header("Faster Reload")]
    [SerializeField, Range(0.1f, 1f)]
    private float reloadDurationMultiplier = 0.75f;

    [Header("Close-Range Damage")]
    [SerializeField] private float damageAtTwoThirdsRange = 1.1f;
    [SerializeField] private float damageAtOneThirdRange = 1.2f;

    public bool DoubleBurst => doubleBurst;
    public bool FasterReload => fasterReload;
    public bool CloseRangeDamage => closeRangeDamage;

    public int BurstsPerPress => doubleBurst ? burstsPerPress : 1;

    public float ReloadDurationMultiplier => fasterReload ? reloadDurationMultiplier : 1f;

    public float GetDamageMultiplier(float distance, float weaponRange)
    {
        if (!closeRangeDamage || weaponRange <= 0f)
            return 1f;

        if (distance <= weaponRange / 3f)
            return damageAtOneThirdRange;

        if (distance <= weaponRange * 2f / 3f)
            return damageAtTwoThirdsRange;

        return 1f;
    }

    public void ActivateDoubleBurst()
    {
        doubleBurst = true;
    }

    public void ActivateFasterReload()
    {
        fasterReload = true;
    }

    public void ActivateCloseRangeDamage()
    {
        closeRangeDamage = true;
    }

    public void ResetUpgrades()
    {
        doubleBurst = false;
        fasterReload = false;
        closeRangeDamage = false;
    }
}