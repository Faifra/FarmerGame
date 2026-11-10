using UnityEngine;

public class LaunchTrapUpgradeScript : MonoBehaviour
{
    [Header("Upgrade Activation")]
    [SerializeField] private bool highForceLaunch;
    [SerializeField] private bool concussiveExhaust;
    [SerializeField] private bool doubleTrigger;

    [Header("High-Impact Launch")]
    [SerializeField, Min(1f)] private float launchHeightMultiplier = 1.15f;
    [SerializeField, Min(1f)] private float damageMultiplier = 1.15f;

    [Header("Concussive Exhaust")]
    [SerializeField] private float stunRadius = 3f;
    [SerializeField] private float stunDuration = 1.5f;

    public float LaunchHeightMultiplier => highForceLaunch ? launchHeightMultiplier : 1f;

    public float DamageMultiplier => highForceLaunch ? damageMultiplier : 1f;

    public bool ConcussiveBlast => concussiveExhaust;
    public bool DoubleTrigger => doubleTrigger;

    public float StunRadius => stunRadius;
    public float StunDuration => stunDuration;

    public void ActivateHighForceLaunch()
    {
        highForceLaunch = true;
    }

    public void ActivateConcussiveBlast()
    {
        concussiveExhaust = true;
    }

    public void ActivateDoubleTrigger()
    {
        doubleTrigger = true;
    }

    public void ResetUpgrades()
    {
        highForceLaunch = false;
        concussiveExhaust = false;
        doubleTrigger = false;
    }
}