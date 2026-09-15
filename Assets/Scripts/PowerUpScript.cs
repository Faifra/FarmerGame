using UnityEngine;
using UnityEngine.Events;

public class PowerUpScript : MonoBehaviour
{
    [SerializeField]
    private UnityEvent powerup_pickup = new UnityEvent();

    void Start()
    {
        // Fail safe
        if(powerup_pickup == null)
            powerup_pickup = new UnityEvent();

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            powerup_pickup.Invoke();
            Destroy(gameObject);
        }
    }
}
