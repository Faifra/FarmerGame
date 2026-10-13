using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; //referens enemyprefab
    [SerializeField] private float spawnInterval = 5f; //intervall
    [SerializeField] private bool waveMode; //toggle waves
    [SerializeField] private int enemiesToSpawn = 1;
    [SerializeField] private float spawnRadius = 5f;

    private float spawnTimer;

    private void Update()
    {
        //spawnTimer += Time.deltaTime; //starta timer!

        //if (waveMode)
        //{
        //    spawnInterval = 15f;
        //}
        //else
        //{
        //    spawnInterval = 5f;
        //}

        //if (spawnTimer >= spawnInterval)
        //{
        //    if (waveMode)
        //    {
        //        SpawnWaves();
        //    }
        //    else
        //    {
        //        Spawn();
        //    }
        //    spawnTimer = 0f;
        //}
    }

    public void Spawn(GameObject enemyPrefab) //skapa en fiende i taget
    {
        Vector2 randomPoint = Random.insideUnitCircle * spawnRadius; //random point in circle (spawnradius).

        Vector3 spawnPosition = transform.position + new Vector3(randomPoint.x, 0f, randomPoint.y); 

        Instantiate(enemyPrefab, spawnPosition, transform.rotation); //skapa enemy på spawners position och rotation.
    }

    //void SpawnWaves()
    //{
    //    for (int i = 0; i < enemiesToSpawn; i++)
    //    {
    //        Spawn();
    //    }
    //    enemiesToSpawn++; //increase enemies to spawn each wave
    //}
}
