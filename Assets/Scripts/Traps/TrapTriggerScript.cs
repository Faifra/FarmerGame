using UnityEngine;

public class TrapTriggerScript : MonoBehaviour
{
    [SerializeField] private LayerMask activationLayers;

    private TrapBaseScript trap;

    private void Awake()
    {
        trap = GetComponentInParent<TrapBaseScript>();

        if (trap == null)
            Debug.LogError("TrapTriggerScript: No TrapBaseScript found on this object or its parents.", this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (trap == null)
            return;

        if ((activationLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        Debug.Log($"Trap triggered by: {other.name}", other);

        trap.Activate(other.gameObject);
    }
}