using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    public Transform goal;

    private NavMeshAgent agent;
    private bool isMovingSpecial;
    private bool isDestroyed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            goal = player.transform;
    }

    private void OnDestroy()
    {
        isDestroyed = true;
    }

    void FixedUpdate()
    {
        if (!isDestroyed && !isMovingSpecial && agent != null && agent.enabled && agent.isOnNavMesh && goal != null)
        {
            agent.destination = goal.position;
        }
    }

    public IEnumerator Knockback(Vector3 direction, float speed, float duration)
    {
        if (isDestroyed || isMovingSpecial || agent == null || !agent.enabled || !agent.isOnNavMesh)
            yield break;

        isMovingSpecial = true;
        agent.isStopped = true;

        float elapsed = 0f;

        while (elapsed < duration && !isDestroyed)
        {
            if (agent == null || !agent.enabled || !agent.isOnNavMesh)
                break;

            agent.Move(direction.normalized * speed * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (isDestroyed)
            yield break;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }

        isMovingSpecial = false;
    }

    public void Launch(float height, float duration)
    {
        if (!isMovingSpecial)
            StartCoroutine(LaunchRoutine(height, duration));
    }

    private IEnumerator LaunchRoutine(float height, float duration)
    {
        isMovingSpecial = true;

        agent.updatePosition = false;

        Vector3 startPosition = transform.position;
        float time = 0f;

        while (time < duration)
        {
            if (isDestroyed)
                yield break;

            time += Time.deltaTime;
            float progress = time / duration;
            float verticalOffset = Mathf.Sin(progress * Mathf.PI) * height;

            transform.position = startPosition + Vector3.up * verticalOffset;

            yield return null;
        }

        if (isDestroyed)
            yield break;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.Warp(transform.position);
            agent.updatePosition = true;
        }

        isMovingSpecial = false;
    }

    public IEnumerator PullTo(Vector3 targetPosition, float duration)
    {
        if (isDestroyed || agent == null)
            yield break;

        isMovingSpecial = true;
        agent.updatePosition = false;

        Vector3 startPosition = transform.position;
        float time = 0f;

        while (time < duration)
        {
            if (isDestroyed)
                yield break;

            time += Time.deltaTime;
            float progress = time / duration;

            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

            yield return null;
        }

        if (isDestroyed)
            yield break;

        transform.position = targetPosition;

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.Warp(transform.position);
            agent.updatePosition = true;
        }

        isMovingSpecial = false;
    }

    public IEnumerator LaunchDirection(
        Vector3 direction, float distance, float duration)
    {
        if (isDestroyed || agent == null)
            yield break;

        isMovingSpecial = true;
        agent.updatePosition = false;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + direction.normalized * distance;
        float time = 0f;

        while (time < duration)
        {
            if (isDestroyed)
                yield break;

            time += Time.deltaTime;
            float progress = time / duration;

            transform.position = Vector3.Lerp(startPosition, endPosition, progress);

            yield return null;
        }

        if (isDestroyed)
            yield break;

        transform.position = endPosition;

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.Warp(transform.position);
            agent.updatePosition = true;
        }

        isMovingSpecial = false;
    }
}