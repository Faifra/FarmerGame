using UnityEngine;
using UnityEngine.AI;

public class CornAI : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject cornPrefab;
    [SerializeField] private GameObject kernelPrefab;
    [SerializeField] private Transform kernelSP; //kernel spawn point
    [SerializeField] private float reloadTime = 5f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float attackRange = 15f;

    [Header("Throwing Distance")]
    [SerializeField] private float throwingDistance = 10f;
    [SerializeField] private float distanceTolerance = 1.5f;

    [Header("Melee Behaviour")]
    [SerializeField] private float meleeEnterDistance = 2.5f;
    [SerializeField] private float meleeExitDistance = 4f;

    [Header("Movement")]
    [SerializeField] private float navMeshSearchRadius = 4f;

    private Transform player;
    private MoveTo moveTo;
    private NavMeshAgent agent;

    private float reloadTimer;
    private bool meleeMode;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;

        moveTo = GetComponent<MoveTo>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (player == null || moveTo == null || agent == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (!meleeMode && distance <= meleeEnterDistance)
        {
            meleeMode = true;
            reloadTimer = 0f;
        }

        if (meleeMode)
        {
            if (distance <= meleeExitDistance)
            {
                moveTo.SetMovementDestination(player.position);
                return;
            }

            meleeMode = false;
        }

        if (distance > throwingDistance + distanceTolerance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Vector3 destination = player.position - direction * throwingDistance;
            MoveToPosition(destination);
        }
        else if (distance < throwingDistance - distanceTolerance)
        {
            Vector3 direction = (transform.position - player.position).normalized;
            Vector3 destination = player.position + direction * throwingDistance;
            MoveToPosition(destination);
        }
        else
        {
            moveTo.StopMovementOverride();
        }

        if (distance <= attackRange)
        {
            reloadTimer += Time.deltaTime;

            if (reloadTimer >= reloadTime)
            {
                ThrowKernel();
                reloadTimer = 0f;
            }
        }
    }

    private void MoveToPosition(Vector3 destination)
    {
        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, navMeshSearchRadius, agent.areaMask))
        {
            moveTo.SetMovementDestination(hit.position);
        }
        else
        {
            moveTo.StopMovementOverride();
        }
    }

    private void ThrowKernel()
    {
        Debug.Log("Throwing kernel!");
        GameObject kernel = Instantiate(kernelPrefab, kernelSP.position, Quaternion.identity);

        Vector3 direction = (player.position - kernelSP.position).normalized;

        Rigidbody rb = kernel.GetComponent<Rigidbody>();
        rb.AddForce(direction * throwForce, ForceMode.Impulse); //applies force to kernels rigidbody
    }

    private void OnDisable()
    {
        if (moveTo != null)
            moveTo.ClearMovementOverride();
    }
}
