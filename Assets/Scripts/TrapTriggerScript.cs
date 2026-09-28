using UnityEngine;

public class TrapTriggerScript : MonoBehaviour
{
    [SerializeField] private LayerMask activationLayers;

    private TrapBaseScript trap;

    private void Awake()
    {
        trap = GetComponent<TrapBaseScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((activationLayers.value & (1 << other.gameObject.layer)) == 0) return;

        trap.Activate(other.gameObject);
    }
}
