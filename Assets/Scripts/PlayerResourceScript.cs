using UnityEngine;

public class PlayerResourceScript : MonoBehaviour
{
    private int totalDrops = 0;
    private int pendingDrops = 0;

    public int TotalDrops => totalDrops;
    public int PendingDrops => pendingDrops;

    public void AddDrops(int amount)
    {
        pendingDrops += amount;
    }

    public void ConfirmPendingDrops()
    {
        totalDrops += pendingDrops;
        pendingDrops = 0;
    }
}
