
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlantVineSpawnerScript : MonoBehaviour
{
    [Header("Vine")]
    [SerializeField] private GameObject vinePrefab;
    [SerializeField] private float detectionRange = 20f;

    [Header("Movement")]
    [SerializeField] private float arrivalDistance = 1f;
    [SerializeField] private float plantingDuration = 2f;

    [Header("Growth")]
    [SerializeField] private float checkInterval = 1.5f;
    [SerializeField, Min(1)] private int maximumVinesToGrow = 1;

    private NavMeshAgent agent;
    private MoveTo moveTo;

    private float checkTimer;
    private int vinesGrown;
    private bool isGrowingVine;

    private Transform originalGoal;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        moveTo = GetComponent<MoveTo>();
    }

    private void Start()
    {
        if (moveTo != null)
            originalGoal = moveTo.goal;
    }

    private void Update()
    {
        if (vinePrefab == null || agent == null || moveTo == null || isGrowingVine || vinesGrown >= maximumVinesToGrow)
        {
            return;
        }

        checkTimer += Time.deltaTime;

        if (checkTimer < checkInterval)
            return;

        checkTimer = 0f;

        TryGrowNearbyVine();
    }

    private void TryGrowNearbyVine()
    {
        VineClimbAnchorScript[] anchors = FindObjectsByType<VineClimbAnchorScript>(FindObjectsSortMode.None);

        VineClimbAnchorScript closestAnchor = null;
        float closestDistance = detectionRange;

        foreach (VineClimbAnchorScript anchor in anchors)
        {
            if (anchor == null || !anchor.CanGrowVine)
                continue;

            float distance = Vector3.Distance(transform.position, anchor.BottomPoint.position);

            if (distance >= closestDistance)
                continue;

            closestDistance = distance;
            closestAnchor = anchor;
        }

        if (closestAnchor == null)
            return;

        StartCoroutine(GoToBuildingAndGrowVine(closestAnchor));
    }

    private IEnumerator GoToBuildingAndGrowVine(VineClimbAnchorScript anchor)
    {
        isGrowingVine = true;

        if (anchor == null)
        {
            isGrowingVine = false;
            yield break;
        }

        Vector3 bottomPosition = anchor.BottomPoint.position;

        moveTo.goal = anchor.BottomPoint;

        while (anchor != null && agent != null && agent.enabled && agent.isOnNavMesh)
        {
            if (!agent.pathPending && agent.remainingDistance <= arrivalDistance)
            {
                break;
            }

            yield return null;
        }

        if (anchor == null || agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            RestoreOriginalGoal();
            isGrowingVine = false;
            yield break;
        }

        agent.isStopped = true;
        agent.ResetPath();

        float elapsed = 0f;

        while (elapsed < plantingDuration)
        {
            if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            {
                RestoreOriginalGoal();
                isGrowingVine = false;
                yield break;
            }

            agent.isStopped = true;
            elapsed += Time.deltaTime;

            yield return null;
        }

        if (anchor != null && anchor.TryGrowVine(vinePrefab))
            vinesGrown++;

        RestoreOriginalGoal();

        if (agent != null && agent.enabled)
            agent.isStopped = false;

        isGrowingVine = false;
    }

    private void RestoreOriginalGoal()
    {
        if (moveTo != null)
            moveTo.goal = originalGoal;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isGrowingVine = false;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.isStopped = false;

        RestoreOriginalGoal();
    }
}