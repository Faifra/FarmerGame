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

    private HashSet<EnemyType> firstEncounters = new(); //hashset has "unique" values so it can keep track of each unique type.
    //each time a unique enemy is added they become a "boss":
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

                GameObject spawnedEnemy = spawner.Spawn(enemy.enemyPrefab);

                EnemyHealthScript enemyHealth = spawnedEnemy.GetComponent<EnemyHealthScript>(); //we retrieve enemyhealthscript for each enemy that spawns.
                //we do this because this script uses the EnemyType enum to identify which type of enemy is spawning. 
                //this allows us to track unique encounters, hashset is only able to store one of each unique type, so if a new type is added to hashset
                //we can turn that enemy into a boss, if it already exists nothing will happen.

                if (enemyHealth != null)
                {
                    EnemyType type = enemyHealth.Type;

                    if (firstEncounters.Add(type)) //try to add the enemy to hashset. if it already exsits it will return false and skip the rest of the code within
                        //this block.
                    {
                        Debug.Log($"Unique enemy appearance: {type}!");
                        CreateBoss(spawnedEnemy);
                    }
                }

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

    public void CreateBoss(GameObject enemy)
    {
        EnemyHealthScript health = enemy.GetComponent<EnemyHealthScript>();

        if (health == null)
            return;

        health.MaxHealth *= 3;
    }

}
