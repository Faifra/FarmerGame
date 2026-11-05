using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates a sprinkler, sprays water in two opposite directions,
/// damages enemies, and converts sprinkler kills into walls.
/// </summary>
public class SprinklerTrapScript : TrapBaseScript, IInteractableScript
{
    [Header("Rotation")]
    [SerializeField] private Transform rotatingHead;
    [SerializeField] private float rotationSpeed = 20f;

    [Header("Water Spray")]
    [SerializeField] private Transform nozzleA;
    [SerializeField] private Transform nozzleB;
    [SerializeField] private ParticleSystem waterSprayA;
    [SerializeField] private ParticleSystem waterSprayB;
    [SerializeField] private float sprayRange = 8f;
    [SerializeField, Range(1f, 180f)] private float sprayAngle = 35f;

    [Header("Damage")]
    [SerializeField] private int damagePerTick = 1;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private GameObject enemyWallPrefab;

    [Header("Visibility")]
    [SerializeField] private LayerMask obstacleLayers;

    private float spinDirection = 1f;
    private float damageTimer;
    private bool trapPlaced;

    public void InitializePlacedSprinkler()
    {
        trapPlaced = true;

        if (waterSprayA != null)
            waterSprayA.Play();

        if (waterSprayB != null)
            waterSprayB.Play();
    }

    private void Update()
    {
        if (!trapPlaced)
            return;

        RotateSprinkler();

        damageTimer += Time.deltaTime;

        if (damageTimer >= damageInterval)
        {
            damageTimer = 0f;
            DamageEnemies();
        }
    }

    private void RotateSprinkler()
    {
        if (rotatingHead == null)
            return;

        rotatingHead.Rotate(Vector3.up, rotationSpeed * spinDirection * Time.deltaTime, Space.World);
    }

    private void DamageEnemies()
    {
        if (enemyWallPrefab == null)
            return;

        Collider[] hits = Physics.OverlapSphere(transform.position, sprayRange + 2f, ~0, QueryTriggerInteraction.Ignore);

        HashSet<EnemyHealthScript> damagedEnemies = new HashSet<EnemyHealthScript>();

        foreach (Collider hit in hits)
        {
            EnemyHealthScript enemy = hit.GetComponentInParent<EnemyHealthScript>();

            if (enemy == null || !damagedEnemies.Add(enemy))
                continue;

            if (!IsInWaterSpray(enemy))
                continue;

            enemy.TakeDamageFromSprinkler(damagePerTick, enemyWallPrefab);
        }
    }

    private bool IsInWaterSpray(EnemyHealthScript enemy)
    {
        if (enemy == null)
            return false;

        Vector3 targetPosition = enemy.transform.position + Vector3.up;

        Vector3 originA = nozzleA != null ? nozzleA.position : transform.position;

        Vector3 originB = nozzleB != null ? nozzleB.position : transform.position;

        Vector3 directionA = nozzleA != null ? nozzleA.forward : rotatingHead.forward;

        Vector3 directionB = nozzleB != null ? nozzleB.forward : -rotatingHead.forward;

        directionA.y = 0f;
        directionB.y = 0f;

        directionA.Normalize();
        directionB.Normalize();

        Vector3 toEnemyA = targetPosition - originA;
        Vector3 toEnemyB = targetPosition - originB;

        toEnemyA.y = 0f;
        toEnemyB.y = 0f;

        float distanceA = toEnemyA.magnitude;
        float distanceB = toEnemyB.magnitude;

        bool inRangeA = distanceA <= sprayRange;
        bool inRangeB = distanceB <= sprayRange;

        if (!inRangeA && !inRangeB)
            return false;

        bool inSprayA = inRangeA && Vector3.Angle(directionA, toEnemyA) <= sprayAngle;

        bool inSprayB = inRangeB && Vector3.Angle(directionB, toEnemyB) <= sprayAngle;

        if (inSprayA && IsBlocked(originA, targetPosition))
            inSprayA = false;

        if (inSprayB && IsBlocked(originB, targetPosition))
            inSprayB = false;

        return inSprayA || inSprayB;
    }

    private bool IsBlocked(Vector3 origin, Vector3 target)
    {
        if (obstacleLayers.value == 0)
            return false;

        Vector3 direction = target - origin;
        float distance = direction.magnitude;

        if (distance <= 0.001f)
            return false;

        return Physics.Raycast(origin, direction.normalized, distance, obstacleLayers, QueryTriggerInteraction.Ignore);
    }

    public override void Activate(GameObject target) { }

    public string InteractionPrompt => "[E Interact]";

    public void Interact()
    {
        ReverseSpinDirection();
    }

    public void ReverseSpinDirection()
    {
        if (!trapPlaced)
            return;

        spinDirection *= -1f;
    }
}