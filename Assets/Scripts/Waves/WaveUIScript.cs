using TMPro;
using UnityEngine;

public class WaveUIScript : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager; //ref to wave manager
    [SerializeField] private TextMeshProUGUI waveText; //current wave
    // [SerializeField] private TextMeshProUGUI phaseText; //current phase

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //waveText.text = "Wave: " + (waveManager.CurrentWave) + "\nTotal Enemies Remaining: " + waveManager.EnemiesRemaining;
        /*
        if (waveManager.IsPrepPhase)
        {
            phaseText.text = "Current phase: Prep Phase";
        }
        else
        {
            phaseText.text = "Current phase: Wave Phase";
        }*/


        waveText.text = (waveManager.CurrentWave).ToString();

        if (waveManager.IsPrepPhase)
        {
            waveText.color = Color.white;
            if(waveManager.TenSecondsLeft())
            {
                waveText.outlineColor = Color.red;
            }
            else
            {
                waveText.outlineColor = Color.white;
            }
        }
        else
        {
            waveText.color = Color.red;
            waveText.outlineColor = Color.red;
        }
    }
}
