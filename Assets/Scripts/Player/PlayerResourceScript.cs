using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Tracks player resources, manages pending drops, and updates the resource UI.
/// </summary>
public class PlayerResourceScript : MonoBehaviour
{
    // UI elements displaying confirmed and pending resources.
    [Header("Resource UI")]
    [SerializeField] private TextMeshProUGUI totalText;
    [SerializeField] private TextMeshProUGUI pendingText;

    // Time to wait before adding pending drops to the total.
    [SerializeField] private float pendingDisplayTime = 0.75f;

    // Tracks confirmed resources and newly collected pending drops.
    private int totalDrops = 0;
    private int pendingDrops = 0;

    // Reference to the coroutine that confirms pending drops.
    private Coroutine pendingCoroutine;

    // Public access to the current resource amounts.
    public int TotalDrops => totalDrops;
    public int PendingDrops => pendingDrops;

    // Initializes the resource UI.
    private void Start()
    {
        UpdateUI();
    }

    // Adds collected drops to the pending amount and restarts the confirmation timer.
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

    // Waits before transferring pending drops to the confirmed total.
    private IEnumerator ConfirmPendingAfterDelay()
    {
        yield return new WaitForSeconds(pendingDisplayTime);

        ConfirmPendingDrops();

        pendingCoroutine = null;
    }

    // Transfers all pending drops into the confirmed resource total.
    public void ConfirmPendingDrops()
    {
        totalDrops += pendingDrops;
        pendingDrops = 0;

        UpdateUI();
    }

    // Spends resources if the player has enough available.
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

    // Refreshes the confirmed and pending resource displays.
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