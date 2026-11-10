using UnityEngine;

public class BeehiveTrapUpgradeScript : MonoBehaviour
{
    [Header("Upgrade Activation")]
    [SerializeField] private bool expandedColony;
    [SerializeField] private bool frenziedSwarm;
    [SerializeField] private bool swarmReplication;

    [Header("Expanded Colony")]
    [SerializeField, Min(1f)] private float damageCapMultiplier = 1.75f;

    [Header("Frenzied Swarm")]
    [SerializeField, Min(1f)] private float movementSpeedMultiplier = 2f;
    [SerializeField, Min(1f)] private float damageMultiplier = 1.2f;

    public float DamageCapMultiplier => expandedColony ? damageCapMultiplier : 1f;

    public float MovementSpeedMultiplier => frenziedSwarm ? movementSpeedMultiplier : 1f;

    public float DamageMultiplier => frenziedSwarm ? damageMultiplier : 1f;

    public bool SwarmReplication => swarmReplication;

    public void ActivateExpandedColony()
    {
        expandedColony = true;
    }

    public void ActivateFrenziedSwarm()
    {
        frenziedSwarm = true;
    }

    public void ActivateSwarmReplication()
    {
        swarmReplication = true;
    }

    public void ResetUpgrades()
    {
        expandedColony = false;
        frenziedSwarm = false;
        swarmReplication = false;
    }
}