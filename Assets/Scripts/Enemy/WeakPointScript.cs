
using System.Collections.Generic;
using UnityEngine;

public class WeakPointScript : MonoBehaviour
{
    private static readonly List<WeakPointScript> allWeakPoints = new List<WeakPointScript>();

    private static bool playerIsScoped;

    [Header("Weak Point")]
    [SerializeField, Min(1f)]
    private float damageMultiplier = 2f;

    [Header("Visual")]
    [SerializeField] private GameObject weakPointVisual;

    public float DamageMultiplier => damageMultiplier;

    public bool IsUnlocked => SniperUpgradeScript.Instance != null && SniperUpgradeScript.Instance.ExposedWeakPoints;

    private void OnEnable()
    {
        if (!allWeakPoints.Contains(this))
            allWeakPoints.Add(this);

        UpdateVisual();
    }

    private void OnDisable()
    {
        allWeakPoints.Remove(this);
    }

    public static void SetScoped(bool scoped)
    {
        playerIsScoped = scoped;
        UpdateAllVisuals();
    }

    public static void RefreshAll()
    {
        UpdateAllVisuals();
    }

    private static void UpdateAllVisuals()
    {
        for (int i = allWeakPoints.Count - 1; i >= 0; i--)
        {
            if (allWeakPoints[i] == null)
            {
                allWeakPoints.RemoveAt(i);
                continue;
            }

            allWeakPoints[i].UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
        if (weakPointVisual != null)
        {
            weakPointVisual.SetActive(IsUnlocked && playerIsScoped);
        }
    }
}