using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles waves by holding a list of all waves as scriptable objects, and a list of all spawners. 
/// Divides all enemies in current wave into spawners to evenly distribute enemies across the map.
/// more todo:
/// - buttons to skip wave cooldown (start next wave immediately)
/// - text to display what enemies are spawning in current wave, and enemies remaining in current wave.
/// </summary>

public class WaveManager : MonoBehaviour
{
    [SerializeField] private List<WaveSO> waves; //list of all waves, they are scriptable objects
    [SerializeField] private List<SpawnEnemy> spawners; //list of spawners scattered throughout the map. 
    [SerializeField] private float waveInterval = 5f;
    [SerializeField] private float waveTimer = 0f;

    public int CurrentWave => currentWaveIndex;
    public int EnemiesRemaining => enemiesRemaining;

    private int currentWaveIndex; //current wave
    private int enemiesRemaining; //enemies remaining of current wave

    private void Start()
    {

    }

    private void Update()
    {
        waveTimer += Time.deltaTime;

        if (waveTimer >= waveInterval)
        {
            StartWave();
            waveTimer = 0f;
        }
    }

    private void StartWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            return;
        }

        WaveSO currentWave = waves[currentWaveIndex];

        int spawnerIndex = 0;

        foreach (WaveEnemy enemy in currentWave.enemies)
        {
            for (int i = 0; i < enemy.amount; i++)
            {
                SpawnEnemy spawner = spawners[spawnerIndex];

                spawner.Spawn(enemy.enemyPrefab);

                enemiesRemaining++;

                spawnerIndex = (spawnerIndex + 1) % spawners.Count;
            }
        }

        currentWaveIndex++;
    }

    public void EnemyDied()
    {
        if (enemiesRemaining > 0)
        {
            enemiesRemaining--;
        }
    }
}
