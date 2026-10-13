using TMPro;
using UnityEngine;

public class WaveUIScript : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager; //ref to wave manager
    [SerializeField] private TextMeshProUGUI waveText; //current wave

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        waveText.text = "Wave: " + (waveManager.CurrentWave)
            + "\nEnemies Remaining: " + waveManager.EnemiesRemaining;
    }
}
