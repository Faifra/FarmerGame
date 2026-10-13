using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PullTrapScript : TrapBaseScript
{
    [SerializeField] private float pullDistance = 3f;
    [SerializeField] private float pullDuration = 0.5f;
    [SerializeField] private float launchDistance = 7f;
    [SerializeField] private float launchDuration = 0.4f;
    [SerializeField] private int damage = 0;

    private int enemiesHit = 0;
    private bool trapFinished = false;

    private HashSet<MoveTo> enemiesBeingProcessed = new HashSet<MoveTo>();

    public override void Activate(GameObject target)
    {
        if (trapFinished)
            return;

        MoveTo enemyMovement = target.GetComponentInParent<MoveTo>();

        if (enemyMovement == null)
            return;

        if (enemiesBeingProcessed.Contains(enemyMovement))
            return;

        Vector3 directionToEnemy = target.transform.position - transform.position;

        directionToEnemy.y = 0f;
        directionToEnemy.Normalize();

        float dot = Vector3.Dot(transform.forward, directionToEnemy);

        if (dot <= 0f)
            return;

        enemiesBeingProcessed.Add(enemyMovement);

        enemiesHit++;

        EnemyHealthScript health = target.GetComponentInParent<EnemyHealthScript>();

        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Vector3 trapPosition = transform.position;
        Vector3 launchDirection = -transform.forward;

        StartCoroutine(PullAndLaunch(enemyMovement, trapPosition, launchDirection));

        if (enemiesHit >= 3)
        {
            trapFinished = true;
        }
    }

    private IEnumerator PullAndLaunch(MoveTo enemy, Vector3 trapPosition, Vector3 launchDirection)
    {
        yield return enemy.PullTo(trapPosition,pullDuration
        );

        if (enemy == null)
        {
            yield break;
        }

        yield return enemy.LaunchDirection(launchDirection, launchDistance, launchDuration
        );

        if (enemy == null)
        {
            yield break;
        }

        enemiesBeingProcessed.Remove(enemy);

        if (trapFinished)
        {
            Destroy(gameObject);
        }
    }
}