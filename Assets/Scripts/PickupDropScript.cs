using UnityEngine;

public class PickupDropScript : MonoBehaviour
{
    [SerializeField] private string materialType;
    [SerializeField] private int amount = 1;

    [Header("Magnet")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private float pickupSpeed = 8f;

    [Header("Drop Movement")]
    [SerializeField] private float settleTime = 1.4f;

    private Transform player;
    private Rigidbody body;

    private bool isBeingPulled = false;
    private bool collected = false;

    private float settleTimer;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

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

    private void Collect()
    {
        if (collected)
            return;

        collected = true;

        DropsUIControllerScript ui = FindFirstObjectByType<DropsUIControllerScript>();

        if (ui != null)
        {
            ui.AddDrops(amount);
        }

        //PlayerResourceScript resources = player.GetComponent<PlayerResourceScript>();

        //if (resources != null)
        //{
        //    resources.AddDrops(amount);
        //}

        Debug.Log(
            $"Picked up {amount} x {materialType}"
        );

        Destroy(gameObject);
    }
}