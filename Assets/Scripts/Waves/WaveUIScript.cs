using TMPro;
using UnityEngine;

public class WaveUIScript : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager; //ref to wave manager
    [SerializeField] private TextMeshProUGUI waveText; //current wave
    [SerializeField] private TextMeshProUGUI phaseText; //current phase

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        waveText.text = "Wave: " + (waveManager.CurrentWave)
            + "\nTotal Enemies Remaining: " + waveManager.EnemiesRemaining;

        if (waveManager.IsPrepPhase)
        {
            phaseText.text = "Current phase: Prep Phase";
        }
        else
        {
            phaseText.text = "Current phase: Wave Phase";
        }
    }
}
