using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private float waveInterval = 120f;
    [SerializeField] private float waveTimer = 0f;

    [SerializeField] private float prepPhaseTime = 30f;
    [SerializeField] private float prepTimer = 0f;
    [SerializeField] private bool isPrepPhase = false;

    public bool IsPrepPhase
    {
        get => isPrepPhase;
    }

    private HashSet<EnemyType> firstEncounters = new(); //hashset has "unique" values so it can keep track of each unique type.
    //each time a unique enemy is added they become a "boss":
    public int CurrentWave => currentWaveIndex;
    public int EnemiesRemaining => enemiesRemaining;

    private int currentWaveIndex; //current wave
    private int enemiesRemaining; //enemies remaining of current wave

    private void Start()
    {   
        StartWave(); //start first wave immediately
    }

    private void Update()
    {
        
        if (isPrepPhase) //if is prep phase     
        {
            waveTimer = 0f; //reset wave timer so it doesnt continue during prep phase

            prepTimer += Time.deltaTime; //start prep timer

            if (prepTimer >= prepPhaseTime) //once prep timer reaches prep phase time:
            {
                isPrepPhase = false; //prep phase is over
                prepTimer = 0f; //reset prep timer
                StartWave(); //start wave
            }
        }
        else //when prep phase = false, we're in "wave phase" or something :P
        {
            prepTimer = 0f; //we already reset prep timer, but doing it again for safety measures i suppose

            waveTimer += Time.deltaTime; //start wave timer

            if (waveTimer >= waveInterval) //when wave timer reaches wave interval: 
            {
                isPrepPhase = true; //start prep phase
                waveTimer = 0f; //reset wave timer to prevent it from continuing to count during prep phase.
            }
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
                Debug.Log($"enemies left of current wave: {currentWave.enemies.Count}");

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
            Debug.Log($"total enemies left: {enemiesRemaining}");

            if (enemiesRemaining <= 0) //this works two ways. 
                //if player is behind, prep phase will start once all enemies are dead.
                //if player is "ahead", prep phase will start early. works the same way, prep phase starts regardless when all enemies are dead.
            {
                Debug.Log("starting prep phase");
                isPrepPhase = true;
                waveTimer = 0f;
            }
        }
    }

    public void CreateBoss(GameObject enemy)
    {
        EnemyHealthScript health = enemy.GetComponent<EnemyHealthScript>();

        if (health == null)
            return;

        health.MaxHealth *= 3;
        
        enemy.AddComponent<BossUIHelperScript>();

    }

}
