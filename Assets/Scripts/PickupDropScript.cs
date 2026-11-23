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

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayers;

    [Header("Pile Visual")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float visualGrowthMultiplier = 1f;

    [Header("Distance Rendering")]
    [SerializeField] private float renderDistance = 35f;
    [SerializeField] private float renderCheckInterval = 0.5f;

    private Renderer[] visualRenderers;
    private float renderCheckTimer;
    private bool isVisible = true;

    // References to the player and the drop's physics body.
    private Transform player;
    private Rigidbody body;

    // Tracks whether the drop is being pulled or has been collected.
    private bool isBeingPulled = false;
    private bool collected = false;

    private bool isGrounded;
    private bool hasSettled;

    // Tracks the elapsed time before the drop settles.
    private float settleTimer;
    public bool HasSettled => hasSettled;

    private Vector3 originalVisualScale;

    // Caches the Rigidbody component.
    private void Awake()
    {
        body = GetComponent<Rigidbody>();

        if (visualRoot != null)
        {
            originalVisualScale = visualRoot.localScale;
        }

        if (visualRoot != null)
            visualRenderers = visualRoot.GetComponentsInChildren<Renderer>(true);
    }

    private void UpdateVisibility()
    {
        if (visualRenderers == null || visualRenderers.Length == 0)
            return;

        renderCheckTimer -= Time.deltaTime;

        if (renderCheckTimer > 0f)
            return;

        renderCheckTimer = renderCheckInterval;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject == null)
                return;

            player = playerObject.transform;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        bool shouldBeVisible = distance <= renderDistance;

        if (shouldBeVisible == isVisible)
            return;

        isVisible = shouldBeVisible;

        foreach (Renderer visualRenderer in visualRenderers)
        {
            if (visualRenderer != null)
                visualRenderer.enabled = isVisible;
        }
    }

    // Updates the drop's movement and checks for nearby players.
    private void Update()
    {
        if (collected)
            return;

        if (hasSettled && !isBeingPulled)
            UpdateVisibility();

        if (isBeingPulled)
        {
            PullToPlayer();
            return;
        }

        if (!hasSettled)
        {
            if (isGrounded)
            {
                settleTimer += Time.deltaTime;

                if (settleTimer >= settleTime)
                    SettleDrop();
            }

            return;
        }

        FindPlayer();
    }

    // Stops the drop's physics movement after the settle time.
    private void SettleDrop()
    {
        if (body == null || hasSettled)
            return;

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.isKinematic = true;

        hasSettled = true;

        if (DropManagerScript.Instance != null)
        {
            DropManagerScript.Instance.RegisterSettledDrop(this);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (hasSettled || body == null)
            return;

        int layerMask = 1 << collision.gameObject.layer;

        if ((groundLayers.value & layerMask) == 0)
            return;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (hasSettled)
            return;

        int layerMask = 1 << collision.gameObject.layer;

        if ((groundLayers.value & layerMask) != 0)
        {
            isGrounded = false;
            settleTimer = 0f;
        }
    }

    public bool CanMergeWith(PickupDropScript other, float distance)
    {
        if (other == null || !hasSettled || !other.hasSettled)
            return false;

        if (materialType != other.materialType)
            return false;

        return Vector3.Distance(transform.position, other.transform.position) <= distance;
    }

    public void AbsorbDrop(PickupDropScript other)
    {
        if (other == null || other == this)
            return;

        amount += other.amount;

        UpdatePileVisual();

        other.collected = true;
        Destroy(other.gameObject);
    }

    private void UpdatePileVisual()
    {
        if (visualRoot == null)
            return;

        float growth = Mathf.Pow(Mathf.Max(1, amount), 1f / 3f);

        visualRoot.localScale = originalVisualScale * growth * visualGrowthMultiplier;
    }

    // Finds the player and starts pulling the drop when in range.
    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
            return;

        float distance = Vector3.Distance(transform.position, playerObject.transform.position);

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

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= 1f)
            Collect();    
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
                resources.AddDrops(amount);
        }

        Destroy(gameObject);
    }
}