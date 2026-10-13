using UnityEngine;

public class LaunchTrapScript : TrapBaseScript
{
    [SerializeField] private float launchHeight = 20f;
    [SerializeField] private float launchDuration = 1f;
    [SerializeField] private int damage = 5;

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