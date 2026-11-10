using UnityEngine;

public class GarlicAI : MonoBehaviour
{
    [Header("General")] //references.
    [SerializeField] private GameObject trailPrefab;
    [SerializeField] private GameObject gasCloud;
    [SerializeField] private Transform player;

    [Header("Trail Settings")] //how often a trail should spawn. 
    [SerializeField] private float trailInterval = 1f; 
    [SerializeField] private float trailTimer = 0f;

    [Header("Explosion Settings")] //explosion stuff, radius, damage. lingeringcloud is a cloud that spawns after explosion that does DoT.
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionDamage = 25f;
    [SerializeField] private float lingeringCloudDuration = 14f;
    [SerializeField] private float lingeringCloudTimer = 0f;
    [SerializeField] private float rangeToExplode = 3f; //distance from player to explode garlic.

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    // Update is called once per frame
    void Update()
    {
        trailTimer += Time.deltaTime; //timer to track trail spawning

        if (trailTimer >= trailInterval) //spawn prefab trail every N seconds.
        {
            GameObject trail = Instantiate(trailPrefab, transform.position, Quaternion.identity);
            trail.SetActive(true); //activate prefab.
            trailTimer = 0f; //resettimer
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= rangeToExplode)
        {
            Explode();
            GameObject gas = Instantiate(gasCloud, transform.position, Quaternion.identity);
            gas.SetActive(true);
            Destroy(gameObject); //destroy garlic after explosion.
            //apply effect here perhaps
        }
    }

    private void Explode() //collision stuff for garlic explosion. 
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius); //create sphere around garlic 

        foreach (Collider hit in hitColliders) //iterate colliders.
        {
            if (hit.CompareTag("Player")) //check for player collision
            {
                GameManagerScript.SetHP(GameManagerScript.GetHP() - (int)explosionDamage); //damage
                Debug.Log($"Garlic exploded, player HP: {GameManagerScript.GetHP()}"); //debug log
                break; //BREAK
            }
        }
    }
}
