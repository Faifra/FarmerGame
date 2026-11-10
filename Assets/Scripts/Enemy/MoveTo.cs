using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    public Transform goal;

    private NavMeshAgent agent;
    private bool isMovingSpecial;
    private bool isDestroyed;

    private float originalSpeed;
    private float slowTimer;
    private float currentSlowMultiplier = 1f;

    [SerializeField] private float increaseSpeedRange = 50f; //outside this range their speed will increase

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            originalSpeed = agent.speed;
        }

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

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer >= increaseSpeedRange) //increase speed if enemy is really far away. (more than N units)
        {
            agent.speed = 10;
        }
        else
        {
            agent.speed = originalSpeed; //reset to original speed if within range.
        }

        if (slowTimer > 0f)
        {
            slowTimer -= Time.fixedDeltaTime;

            if (slowTimer <= 0f)
            {
                slowTimer = 0f;
                currentSlowMultiplier = 1f;

                if (agent != null)
                    agent.speed = originalSpeed;
            }
        }
    }

    public void ApplySlow(float multiplier, float duration)
    {
        if (agent == null)
            return;

        multiplier = Mathf.Clamp(multiplier, 0.1f, 1f);

        currentSlowMultiplier = Mathf.Min(currentSlowMultiplier, multiplier);

        slowTimer = Mathf.Max(slowTimer, duration);

        agent.speed = originalSpeed * currentSlowMultiplier;
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

    public void Stun(float duration)
    {
        if (isDestroyed || isMovingSpecial)
            return;

        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isMovingSpecial = true;

        bool wasStopped = false;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            wasStopped = agent.isStopped;
            agent.isStopped = true;
        }

        yield return new WaitForSeconds(duration);

        if (isDestroyed)
            yield break;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = wasStopped;
        }

        isMovingSpecial = false;
    }
}