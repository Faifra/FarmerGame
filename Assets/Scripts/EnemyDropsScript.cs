using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles spawning item drops when an enemy is defeated.
/// </summary>
public class EnemyDropsScript : MonoBehaviour
{
    // Defines an item prefab and the quantity to spawn.
    [System.Serializable]
    public class DropEntry
    {
        public GameObject prefab;
        public int amount = 1;
    }

    // List of items this enemy can drop.
    [SerializeField] private List<DropEntry> drops = new List<DropEntry>();

    // Controls the distance and height of spawned drops.
    [Header("Drop Burst")]
    [SerializeField] private float burstDistance = 1.5f;
    [SerializeField] private float burstHeight = 0.5f;

    // Spawns the configured quantity of each valid drop.
    public void Drop()
    {
        foreach (DropEntry drop in drops)
        {
            if (drop.prefab == null || drop.amount <= 0)
                continue;

            for (int i = 0; i < drop.amount; i++)
            {
                SpawnDrop(drop.prefab);
            }
        }
    }

    // Spawns a drop in a random direction and launches it outward.
    private void SpawnDrop(GameObject prefab)
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        Vector3 direction = new Vector3(randomDirection.x, 0f, randomDirection.y);

        Vector3 spawnPosition = transform.position + direction * Random.Range(0.2f, burstDistance) + Vector3.up * burstHeight;

        GameObject drop = Instantiate(prefab, spawnPosition, Quaternion.identity
        );

        Rigidbody body = drop.GetComponent<Rigidbody>();

        if (body != null)
        {
            body.linearVelocity = direction * 1.5f + Vector3.up * 1f;
        }
    }
}