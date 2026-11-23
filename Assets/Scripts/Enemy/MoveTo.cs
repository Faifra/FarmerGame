using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    [Header("Erratic Movement")]
    [SerializeField] private bool useErraticMovement = true;
    [SerializeField] private float movementRadius = 4f;
    [SerializeField] private float minDirectionChangeTime = 2f;
    [SerializeField] private float maxDirectionChangeTime = 4f;
    [SerializeField] private float destinationArrivalDistance = 1.5f;
    [SerializeField] private float navMeshSearchRadius = 5f;
    [SerializeField] private float directChaseDistance = 5f;

    [Header("Flanking")]
    [SerializeField] private float flankDistance = 8f;
    [SerializeField] private float flankAngleVariation = 20f;
    [SerializeField] private float flankDistanceVariation = 1.5f;

    private static readonly List<MoveTo> activeEnemies = new List<MoveTo>();
    private static float groupAngleOffset;

    private float directionChangeTimer;
    private Vector3 currentErraticDestination;
    private bool hasErraticDestination;

    public Transform goal;

    private NavMeshAgent agent;
    private bool isMovingSpecial;
    private bool isDestroyed;

    private float originalSpeed;
    private float slowTimer;
    private float currentSlowMultiplier = 1f;

    [SerializeField] private float increaseSpeedRange = 50f; //outside this range their speed will increase

    private bool hasMovementOverride;
    private bool movementOverrideStopped;
    private Vector3 movementOverrideDestination;

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
        if (!isDestroyed && !isMovingSpecial && agent != null && agent.enabled && agent.isOnNavMesh && !agent.isOnOffMeshLink && goal != null)
        {
            if (hasMovementOverride)
            {
                agent.isStopped = movementOverrideStopped;

                if (!movementOverrideStopped)
                    agent.SetDestination(movementOverrideDestination);
            }
            else if (goal != null)
            {
                agent.isStopped = false;

                float distanceToGoal = Vector3.Distance(transform.position, goal.position);

                if (distanceToGoal <= directChaseDistance)
                {
                    hasErraticDestination = false;
                    agent.SetDestination(goal.position);
                }
                else if (useErraticMovement)
                {
                    directionChangeTimer -= Time.fixedDeltaTime;

                    if (!hasErraticDestination || directionChangeTimer <= 0f || (!agent.pathPending && agent.remainingDistance <= destinationArrivalDistance))
                    {
                        ChooseErraticDestination();
                    }

                    if (hasErraticDestination)
                        agent.SetDestination(currentErraticDestination);
                    else
                        agent.SetDestination(goal.position);
                }
                else
                {
                    agent.SetDestination(goal.position);
                }
            }
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

    public void SetMovementDestination(Vector3 destination)
    {
        hasMovementOverride = true;
        movementOverrideStopped = false;
        movementOverrideDestination = destination;
    }

    public void StopMovementOverride()
    {
        hasMovementOverride = true;
        movementOverrideStopped = true;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    public void ClearMovementOverride()
    {
        hasMovementOverride = false;
        movementOverrideStopped = false;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.isStopped = false;
    }

    private void ChooseErraticDestination()
    {
        if (goal == null || agent == null)
            return;

        activeEnemies.RemoveAll(enemy => enemy == null);

        int enemyCount = activeEnemies.Count;
        int enemyIndex = activeEnemies.IndexOf(this);

        if (enemyCount <= 0 || enemyIndex < 0)
            return;

        float angleBetweenEnemies = 360f / enemyCount;

        float angle = groupAngleOffset + enemyIndex * angleBetweenEnemies + Random.Range(-flankAngleVariation, flankAngleVariation);

        float distance = flankDistance + Random.Range(-flankDistanceVariation, flankDistanceVariation);

        float radians = angle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(Mathf.Sin(radians), 0f, Mathf.Cos(radians)) * distance;

        Vector3 randomPosition = goal.position + offset;

        directionChangeTimer = Random.Range(minDirectionChangeTime, maxDirectionChangeTime);

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, navMeshSearchRadius, agent.areaMask))
        {
            currentErraticDestination = hit.position;
            hasErraticDestination = true;
        }
        else
        {
            hasErraticDestination = false;
        }
    }

    private void OnEnable()
    {
        if (!activeEnemies.Contains(this))
        {
            if (activeEnemies.Count == 0)
                groupAngleOffset = Random.Range(0f, 360f);

            activeEnemies.Add(this);
        }
    }

    private void OnDisable()
    {
        activeEnemies.Remove(this);

        if (activeEnemies.Count == 0)
            groupAngleOffset = 0f;
    }
}