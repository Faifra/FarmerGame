using UnityEngine;

public class GarlicTrailScript : MonoBehaviour
{
    [Header("Trail Settings")] //damage and tick interval for trail. testing / balancing
    [SerializeField] private float trailDamage = 5f;
    [SerializeField] private float tickTimer = 0f;
    [SerializeField] private float tickInterval = 0.5f;

    [Header("Trail Lifetime Settings")] //track lifetime for each trail prefab.
    [SerializeField] private float trailLifetime = 7f;
    [SerializeField] private float lifeTimeTimer = 0f;
   

    private void Update()
    {
        lifeTimeTimer += Time.deltaTime; //update lifetime and track

        if (lifeTimeTimer >= trailLifetime)
        {
            Destroy(gameObject); //destroy trail after lifetime.  **needs balacing / testing**
        }
    }

    private void OnTriggerStay(Collider other)
    {
        tickTimer += Time.deltaTime; //track time the player is inside the collider.

        if (tickTimer >= tickInterval) //compare ticktimer with tickinterval. should take damage ever N seconds. **needs testing / balancing**
        {
            if (other.CompareTag("Player")) //check for player
            {
                GameManagerScript.SetHP(GameManagerScript.GetHP() - (int)trailDamage); //take tick damage.
                tickTimer = 0f; //reset to continue damage ticks on timer.
                Debug.Log("player taking damage");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) //check for player
        {
            tickTimer = 0f; //reset timer when player exits collider
        }
    }
}
