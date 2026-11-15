using UnityEngine;
using UnityEngine.Events;

public class PowerUpScript : MonoBehaviour
{
    [SerializeField]
    private UnityEvent powerup_pickup = new UnityEvent();

    [SerializeField]
    private PowerUpSO powerUp;

    [SerializeField]
    private PlayerMovementScript movementScript;

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
            movementScript.ApplyPowerUp(powerUp);
            Destroy(gameObject);
        }
    }
}
