using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; //referens enemyprefab
    [SerializeField] private float spawnInterval = 5f; //intervall

    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime; //starta timer!

        if (spawnTimer >= spawnInterval)
        {
            Spawn();
            spawnTimer = 0f;
        }
    }

    void Spawn() //skapa en fiende i taget
    {
        Instantiate(enemyPrefab, transform.position, transform.rotation); //skapa enemy på spawners position och rotation.
    }
}
