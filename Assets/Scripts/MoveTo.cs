// MoveTo.cs
using UnityEngine;
using UnityEngine.AI;

using System.Collections;

public class MoveTo : MonoBehaviour
{
    public Transform goal;

    private NavMeshAgent agent;
    private bool isMovingSpecial;
    private bool isDestroyed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnDestroy()
    {
        isDestroyed = true;
    }

    void FixedUpdate()
    {
        if (!isDestroyed && !isMovingSpecial && agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.destination = goal.position;
        }
    }

    public void Launch(float height, float duration)
    {
        if (!isMovingSpecial)
        {
            StartCoroutine(LaunchRoutine(height, duration));
        }
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
        if (isDestroyed)
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

            transform.position =
                Vector3.Lerp(startPosition, targetPosition, progress);

            yield return null;
        }

        if (isDestroyed)
            yield break;

        transform.position = targetPosition;
    }

    public IEnumerator LaunchDirection(Vector3 direction, float distance, float duration)
    {
        if (isDestroyed)
            yield break;

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

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.Warp(transform.position);
            agent.updatePosition = true;
        }

        isMovingSpecial = false;
    }
}