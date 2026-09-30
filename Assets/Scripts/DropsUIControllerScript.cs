using TMPro;
using UnityEngine;
using System.Collections;

public class DropsUIControllerScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalText;
    [SerializeField] private TextMeshProUGUI pendingText;

    [SerializeField] private float pendingDisplayTime = 1f;

    private int totalDrops = 0;
    private int pendingDrops = 0;

    private Coroutine pendingRoutine;

    private void Start()
    {
        UpdateUI();
    }

    public void AddDrops(int amount)
    {
        pendingDrops += amount;

        UpdateUI();

        if (pendingRoutine != null)
        {
            StopCoroutine(pendingRoutine);
        }

        pendingRoutine = StartCoroutine(FinishPendingDrops());
    }

    private IEnumerator FinishPendingDrops()
    {
        yield return new WaitForSeconds(pendingDisplayTime);

        totalDrops += pendingDrops;
        pendingDrops = 0;

        UpdateUI();

        pendingRoutine = null;
    }

    private void UpdateUI()
    {
        totalText.text = totalDrops.ToString();

        if (pendingDrops > 0)
        {
            pendingText.text = "+" + pendingDrops;
            pendingText.gameObject.SetActive(true);
        }
        else
        {
            pendingText.gameObject.SetActive(false);
        }
    }
}
