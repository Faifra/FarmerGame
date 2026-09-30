using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerResourceScript : MonoBehaviour
{
    [Header("Resource UI")]
    [SerializeField] private TextMeshProUGUI totalText;
    [SerializeField] private TextMeshProUGUI pendingText;

    [SerializeField] private float pendingDisplayTime = 0.75f;

    private int totalDrops = 0;
    private int pendingDrops = 0;

    private Coroutine pendingCoroutine;

    public int TotalDrops => totalDrops;
    public int PendingDrops => pendingDrops;

    private void Start()
    {
        UpdateUI();
    }

    public void AddDrops(int amount)
    {
        if (amount <= 0)
            return;

        pendingDrops += amount;

        UpdateUI();

        if (pendingCoroutine != null)
        {
            StopCoroutine(pendingCoroutine);
        }

        pendingCoroutine = StartCoroutine(ConfirmPendingAfterDelay());
    }

    private IEnumerator ConfirmPendingAfterDelay()
    {
        yield return new WaitForSeconds(pendingDisplayTime);

        ConfirmPendingDrops();

        pendingCoroutine = null;
    }

    public void ConfirmPendingDrops()
    {
        totalDrops += pendingDrops;
        pendingDrops = 0;

        UpdateUI();
    }

    public bool SpendDrops(int amount)
    {
        if (amount <= 0)
            return false;

        if (totalDrops < amount)
            return false;

        totalDrops -= amount;

        UpdateUI();

        return true;
    }

    private void UpdateUI()
    {
        if (totalText != null)
        {
            totalText.text = totalDrops.ToString();
        }

        if (pendingText != null)
        {
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
}