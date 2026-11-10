using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Pulls enemies toward the trap before launching them away.
/// </summary>
public class PullTrapScript : TrapBaseScript
{
    [Header("Pull and Launch")]
    [SerializeField] private float pullRange = 5f;
    [SerializeField] private float pullDuration = 0.5f;
    [SerializeField] private float launchDistance = 7f;
    [SerializeField] private float launchDuration = 0.4f;
    [SerializeField] private int damage = 0;

    [Header("Upgrades")]
    [SerializeField] private PullTrapUpgradeScript trapUpgrades;

    private int enemiesHit = 0;
    private bool trapFinished = false;

    private readonly HashSet<MoveTo> enemiesBeingProcessed = new HashSet<MoveTo>();

    public override void Activate(GameObject target)
    {
        if (trapFinished || target == null)
            return;

        MoveTo enemyMovement = target.GetComponentInParent<MoveTo>();

        if (enemyMovement == null)
            return;

        if (enemiesBeingProcessed.Contains(enemyMovement))
            return;

        int maximumEnemies = trapUpgrades != null
            ? trapUpgrades.MaximumEnemies
            : 3;

        if (enemiesHit >= maximumEnemies)
            return;

        Vector3 directionToEnemy = target.transform.position - transform.position;
        directionToEnemy.y = 0f;

        if (directionToEnemy.sqrMagnitude <= 0.001f)
            return;

        directionToEnemy.Normalize();

        float dot = Vector3.Dot(transform.forward, directionToEnemy);

        if (dot <= 0f)
            return;

        float distanceToEnemy = Vector3.Distance(
            transform.position,
            target.transform.position
        );

        if (distanceToEnemy > GetPullRange())
            return;

        EnemyHealthScript health = enemyMovement.GetComponent<EnemyHealthScript>();

        if (health != null && health.IsDead)
            return;

        enemiesBeingProcessed.Add(enemyMovement);
        enemiesHit++;

        float damageMultiplier = trapUpgrades != null
            ? trapUpgrades.DamageMultiplier
            : 1f;

        int finalDamage = Mathf.Max(0, Mathf.RoundToInt(damage * damageMultiplier));

        if (health != null && finalDamage > 0)
            health.TakeDamage(finalDamage);

        Vector3 trapPosition = transform.position;
        Vector3 launchDirection = -transform.forward;

        float finalPullDuration = pullDuration * (
            trapUpgrades != null ? trapUpgrades.PullDurationMultiplier : 1f
        );

        float finalLaunchDistance = launchDistance * (
            trapUpgrades != null ? trapUpgrades.LaunchDistanceMultiplier : 1f
        );

        StartCoroutine(PullAndLaunch(
            enemyMovement,
            trapPosition,
            launchDirection,
            finalPullDuration,
            finalLaunchDistance
        ));

        if (enemiesHit >= maximumEnemies)
            trapFinished = true;
    }

    private IEnumerator PullAndLaunch(
        MoveTo enemy,
        Vector3 trapPosition,
        Vector3 launchDirection,
        float finalPullDuration,
        float finalLaunchDistance)
    {
        yield return enemy.PullTo(trapPosition, finalPullDuration);

        if (enemy != null)
        {
            yield return enemy.LaunchDirection(
                launchDirection,
                finalLaunchDistance,
                launchDuration
            );
        }

        if (enemy != null)
            enemiesBeingProcessed.Remove(enemy);

        if (trapFinished && enemiesBeingProcessed.Count == 0)
            Destroy(gameObject);
    }

    private void Update()
    {
        if (trapFinished)
            return;

        int maximumEnemies = trapUpgrades != null
            ? trapUpgrades.MaximumEnemies
            : 3;

        if (enemiesHit >= maximumEnemies)
            return;

        Collider[] nearbyEnemies = Physics.OverlapSphere(
            transform.position,
            GetPullRange()
        );

        foreach (Collider nearbyCollider in nearbyEnemies)
        {
            if (trapFinished)
                break;

            Activate(nearbyCollider.gameObject);
        }
    }

    private float GetPullRange()
    {
        float multiplier = trapUpgrades != null
            ? trapUpgrades.PullRangeMultiplier
            : 1f;

        return pullRange * multiplier;
    }
}