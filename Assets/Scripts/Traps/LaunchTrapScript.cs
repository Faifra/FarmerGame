using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Damages and launches an enemy when the trap is activated.
/// </summary>
public class LaunchTrapScript : TrapBaseScript
{
    // Controls the launch height, duration, and damage dealt.
    [Header("Launch")]
    [SerializeField] private float launchHeight = 20f;
    [SerializeField] private float launchDuration = 1f;
    [SerializeField] private int damage = 5;

    [Header("Upgrades")]
    [SerializeField] private LaunchTrapUpgradeScript trapUpgrades;

    private int activationCount;
    private bool trapDestroyed;

    private readonly HashSet<int> hitEnemies = new HashSet<int>();

    // Applies damage, launches the target, and removes the trap.
    public override void Activate(GameObject target)
    {
        if (trapDestroyed || target == null)
            return;

        EnemyHealthScript health =
            target.GetComponentInParent<EnemyHealthScript>();

        if (health == null || health.IsDead)
            return;

        // Prevent the same enemy from activating the trap twice.
        if (hitEnemies.Contains(health.GetInstanceID()))
            return;

        hitEnemies.Add(health.GetInstanceID());

        float heightMultiplier = trapUpgrades != null
            ? trapUpgrades.LaunchHeightMultiplier
            : 1f;

        float damageMultiplier = trapUpgrades != null
            ? trapUpgrades.DamageMultiplier
            : 1f;

        int finalDamage = Mathf.Max(
            1,
            Mathf.RoundToInt(damage * damageMultiplier)
        );

        float finalLaunchHeight = launchHeight * heightMultiplier;

        health.TakeDamage(finalDamage);

        MoveTo enemyMovement = health.GetComponent<MoveTo>();

        if (enemyMovement != null && !health.IsDead)
        {
            enemyMovement.Launch(finalLaunchHeight, launchDuration);
        }

        if (trapUpgrades != null && trapUpgrades.ConcussiveBlast)
        {
            StunNearbyEnemies(health);
        }

        activationCount++;

        int maximumActivations =
            trapUpgrades != null && trapUpgrades.DoubleTrigger ? 2 : 1;

        if (activationCount >= maximumActivations)
        {
            trapDestroyed = true;
            Destroy(gameObject);
        }
    }

    private void StunNearbyEnemies(EnemyHealthScript launchedEnemy)
    {
        if (trapUpgrades == null)
            return;

        Collider[] nearbyColliders = Physics.OverlapSphere(
            transform.position,
            trapUpgrades.StunRadius
        );

        HashSet<EnemyHealthScript> stunnedEnemies =
            new HashSet<EnemyHealthScript>();

        foreach (Collider nearbyCollider in nearbyColliders)
        {
            if (nearbyCollider == null)
                continue;

            EnemyHealthScript health =
                nearbyCollider.GetComponentInParent<EnemyHealthScript>();

            if (health == null ||
                health.IsDead ||
                health == launchedEnemy ||
                !stunnedEnemies.Add(health))
                continue;

            MoveTo enemyMovement = health.GetComponent<MoveTo>();

            if (enemyMovement != null)
            {
                enemyMovement.Stun(trapUpgrades.StunDuration);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (trapUpgrades == null || !trapUpgrades.ConcussiveBlast)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, trapUpgrades.StunRadius);
    }
}