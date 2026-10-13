using UnityEngine;

/// <summary>
/// Handles drop settling, automatic player attraction, and resource collection.
/// </summary>
public class PickupDropScript : MonoBehaviour
{
    // Identifies the resource type and quantity granted on collection.
    [SerializeField] private string materialType;
    [SerializeField] private int amount = 1;

    // Controls the distance and speed of the pickup magnet.
    [Header("Magnet")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private float pickupSpeed = 8f;

    // Determines how long the drop moves before settling.
    [Header("Drop Movement")]
    [SerializeField] private float settleTime = 1.4f;

    // References to the player and the drop's physics body.
    private Transform player;
    private Rigidbody body;

    // Tracks whether the drop is being pulled or has been collected.
    private bool isBeingPulled = false;
    private bool collected = false;

    // Tracks the elapsed time before the drop settles.
    private float settleTimer;

    // Caches the Rigidbody component.
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Updates the drop's movement and checks for nearby players.
    private void Update()
    {
        if (collected)
            return;

        if (isBeingPulled)
        {
            PullToPlayer();
            return;
        }

        SettleDrop();
        FindPlayer();
    }

    // Stops the drop's physics movement after the settle time.
    private void SettleDrop()
    {
        if (body == null)
            return;

        settleTimer += Time.deltaTime;

        if (settleTimer >= settleTime)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;

            body.isKinematic = true;
        }
    }

    // Finds the player and starts pulling the drop when in range.
    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
            return;

        float distance =
            Vector3.Distance(transform.position, playerObject.transform.position);

        if (distance <= pickupRange)
        {
            player = playerObject.transform;
            isBeingPulled = true;
        }
    }

    // Moves the drop toward the player and collects it when close enough.
    private void PullToPlayer()
    {
        if (player == null)
        {
            isBeingPulled = false;
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, player.position, pickupSpeed * Time.deltaTime);

        float distance =
            Vector3.Distance(transform.position, player.position);

        if (distance <= 1f)
        {
            Collect();
        }
    }

    // Grants the resource to the player and removes the drop.
    private void Collect()
    {
        if (collected)
            return;

        collected = true;

        if (player != null)
        {
            PlayerResourceScript resources = player.GetComponent<PlayerResourceScript>();

            if (resources != null)
            {
                resources.AddDrops(amount);
            }
        }

        Destroy(gameObject);
    }
}