using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class VineClimbAgentScript : MonoBehaviour
{
    [Header("Climbing")]
    [SerializeField, Min(0.1f)] private float climbDuration = 2f;

    private NavMeshAgent agent;
    private bool isClimbing;
    private bool originalUpdatePosition = true;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("VineClimbAgentScript requires a NavMeshAgent.", gameObject);

            enabled = false;
            return;
        }

        agent.autoTraverseOffMeshLink = false;
    }

    private void OnEnable()
    {
        if (agent != null)
            agent.autoTraverseOffMeshLink = false;
    }

    private void Update()
    {
        if (isClimbing || agent == null)
            return;

        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        if (agent.isOnOffMeshLink)
            StartCoroutine(ClimbVine());
    }

    private IEnumerator ClimbVine()
    {
        isClimbing = true;

        OffMeshLinkData linkData = agent.currentOffMeshLinkData;

        VineClimbLinkScript vine = FindVine(linkData);

        Vector3 start = linkData.startPos;
        Vector3 end = linkData.endPos;

        originalUpdatePosition = agent.updatePosition;
        agent.updatePosition = false;

        float elapsed = 0f;

        while (elapsed < climbDuration)
        {
            if (agent == null || !agent.enabled)
                yield break;

            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / climbDuration);

            transform.position = vine != null ? vine.GetPoint(progress) : Vector3.Lerp(start, end, progress);

            agent.nextPosition = transform.position;

            yield return null;
        }

        transform.position = end;
        agent.nextPosition = end;

        if (agent.enabled && agent.isOnOffMeshLink)
            agent.CompleteOffMeshLink();

        agent.updatePosition = originalUpdatePosition;

        isClimbing = false;
    }

    private VineClimbLinkScript FindVine(OffMeshLinkData linkData)
    {
        VineClimbLinkScript[] vines = FindObjectsByType<VineClimbLinkScript>(FindObjectsSortMode.None);

        foreach (VineClimbLinkScript vine in vines)
        {
            if (vine == null)
                continue;

            if (Vector3.Distance(vine.StartPoint, linkData.startPos) < 1f &&
                Vector3.Distance(vine.EndPoint, linkData.endPos) < 1f)
            {
                return vine;
            }
        }

        return null;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isClimbing = false;

        if (agent != null && agent.enabled)
        {
            agent.updatePosition = true;
            agent.autoTraverseOffMeshLink = false;
        }
    }
}