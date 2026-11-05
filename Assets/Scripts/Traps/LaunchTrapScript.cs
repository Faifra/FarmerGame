using UnityEngine;

/// <summary>
/// Damages and launches an enemy when the trap is activated.
/// </summary>
public class LaunchTrapScript : TrapBaseScript
{
    // Controls the launch height, duration, and damage dealt.
    [SerializeField] private float launchHeight = 20f;
    [SerializeField] private float launchDuration = 1f;
    [SerializeField] private int damage = 5;

    // Applies damage, launches the target, and removes the trap.
    public override void Activate(GameObject target)
    {
        EnemyHealthScript health = target.GetComponentInParent<EnemyHealthScript>();

        if (health != null)
        {
            health.TakeDamage(damage);
        }

        MoveTo enemyMovement = target.GetComponentInParent<MoveTo>();

        if (enemyMovement != null)
        {
            enemyMovement.Launch(launchHeight, launchDuration);
        }

        Destroy(gameObject);
    }
}