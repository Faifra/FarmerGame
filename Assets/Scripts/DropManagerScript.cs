using System.Collections.Generic;
using UnityEngine;

public class DropManagerScript : MonoBehaviour
{
    public static DropManagerScript Instance { get; private set; }

    [Header("Drop Merging")]
    [SerializeField] private float mergeDistance = 1.5f;

    private readonly List<PickupDropScript> settledPiles = new List<PickupDropScript>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterSettledDrop(PickupDropScript drop)
    {
        if (drop == null)
            return;

        settledPiles.RemoveAll(pile => pile == null);

        foreach (PickupDropScript pile in settledPiles)
        {
            if (pile == drop)
                continue;

            if (pile.CanMergeWith(drop, mergeDistance))
            {
                pile.AbsorbDrop(drop);
                return;
            }
        }

        settledPiles.Add(drop);
    }
}