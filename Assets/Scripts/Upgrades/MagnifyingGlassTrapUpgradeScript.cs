
using UnityEngine;

public class MagnifyingGlassTrapUpgradeScript : MonoBehaviour
{
    [Header("Upgrade Activation")]
    [SerializeField] private bool focusedSunlight;
    [SerializeField] private bool intensifiedHeat;
    [SerializeField] private bool refractedBeam;

    [Header("Focused Sunlight")]
    [SerializeField, Min(1f)] private float damageMultiplier = 2f;

    [Header("Intensified Heat")]
    [SerializeField, Range(0.1f, 1f)]
    private float slowMultiplier = 0.8f;

    public float DamageMultiplier => focusedSunlight ? damageMultiplier : 1f;

    public float SlowMultiplier => intensifiedHeat ? slowMultiplier : 1f;

    public bool RefractedBeam => refractedBeam;

    public void ActivateFocusedSunlight()
    {
        focusedSunlight = true;
    }

    public void ActivateIntensifiedHeat()
    {
        intensifiedHeat = true;
    }

    public void ActivateRefractedBeam()
    {
        refractedBeam = true;
    }

    public void ResetUpgrades()
    {
        focusedSunlight = false;
        intensifiedHeat = false;
        refractedBeam = false;
    }
}