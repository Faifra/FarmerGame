using UnityEngine;

public class SaladLeafScript : MonoBehaviour
{
    [Header("References")]
    private PlayerMovementScript playerMovementScript;

    [Header("Settings")]
    [SerializeField] private float leafDamage = 10f; //damage that the leaf deals
    [SerializeField] private float lifeTime = 30f; //lifetime of the leaf.
    [SerializeField] private float stunDuration = 2f;
    [SerializeField] private float stunCooldown = 3f;
    private float lifeTimeTimer;

    private void OnTriggerEnter(Collider other) //collider logic
    {
        if (other.CompareTag("Player")) //if collider tag is player
        {
            playerMovementScript = other.GetComponent<PlayerMovementScript>();

            GameManagerScript.SetHP(GameManagerScript.GetHP() - (int)leafDamage); //player takes damage
            Debug.Log($"player health: {GameManagerScript.GetHP()}");

            GameManagerScript.instance.ApplyStun(stunDuration, stunCooldown);
        }
        else if (other.CompareTag("LeafBreaker")) //if the tag is "leafbreaker" we can break the leaf with a specific weapon. this is the scythe for now.
        {
            Destroy(gameObject); //destroy the leaf
        }
    }

    private void Update()
    {
        lifeTimeTimer += Time.deltaTime;

        if (lifeTimeTimer >= lifeTime)
        {
            Destroy(gameObject);
        }

    }
}
