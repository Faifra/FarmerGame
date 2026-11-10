
using UnityEngine;

public class SniperUpgradeScript : MonoBehaviour
{
    public static SniperUpgradeScript Instance { get; private set; }

    [Header("Upgrade Activation")]
    [SerializeField] private bool fasterReload;
    [SerializeField] private bool exposedWeakPoints;
    [SerializeField] private bool ricochetShot;

    [Header("Faster Reload")]
    [SerializeField, Range(0.1f, 1f)]
    private float reloadDurationMultiplier = 0.75f;

    [Header("Exposed Weak Points")]
    [SerializeField, Min(1f)]
    private float weakPointDamageMultiplier = 2f;

    [Header("Ricochet Shot")]
    [SerializeField] private bool allowRicochet = true;

    public float ReloadDurationMultiplier => fasterReload ? reloadDurationMultiplier : 1f;

    public bool ExposedWeakPoints => exposedWeakPoints;

    public float WeakPointDamageMultiplier => weakPointDamageMultiplier;

    public bool RicochetShot => ricochetShot && allowRicochet;

    private void Awake()
    {
        Instance = this;
        WeakPointScript.RefreshAll();
    }

    private void OnEnable()
    {
        Instance = this;
        WeakPointScript.RefreshAll();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            WeakPointScript.RefreshAll();
        }
    }

    public void ActivateFasterReload()
    {
        fasterReload = true;
    }

    public void ActivateExposedWeakPoints()
    {
        exposedWeakPoints = true;
        WeakPointScript.RefreshAll();
    }

    public void ActivateRicochetShot()
    {
        ricochetShot = true;
    }

    public void ResetUpgrades()
    {
        fasterReload = false;
        exposedWeakPoints = false;
        ricochetShot = false;

        WeakPointScript.RefreshAll();
    }
}