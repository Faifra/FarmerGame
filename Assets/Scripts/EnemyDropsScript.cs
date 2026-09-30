using UnityEngine;
using System.Collections.Generic;

public class EnemyDropsScript : MonoBehaviour
{
    [System.Serializable]
    public class DropEntry
    {
        public GameObject prefab;
        public int amount = 1;
    }

    [SerializeField] private List<DropEntry> drops = new List<DropEntry>();

    [Header("Drop Burst")]
    [SerializeField] private float burstDistance = 1.5f;
    [SerializeField] private float burstHeight = 0.5f;

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