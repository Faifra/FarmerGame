using System.Collections.Generic;
using UnityEngine;

public class BlobStackScript : MonoBehaviour
{
    [Header("Blob Threshold")]
    [SerializeField] private int overloadThreshold = 7;

    private int activeOverloadThreshold;

    private readonly List<BlobProjectileScript> attachedBlobs = new List<BlobProjectileScript>();

    private bool detonating;

    private void Awake()
    {
        activeOverloadThreshold = Mathf.Max(1, overloadThreshold);
    }

    public void RegisterBlob(BlobProjectileScript blob, float thresholdMultiplier = 1f)
    {
        if (blob == null || detonating || attachedBlobs.Contains(blob))
            return;

        activeOverloadThreshold = Mathf.Max(1, Mathf.RoundToInt(overloadThreshold * thresholdMultiplier));

        attachedBlobs.Add(blob);

        if (attachedBlobs.Count >= activeOverloadThreshold)
            DetonateAll();
    }

    public void UnregisterBlob(BlobProjectileScript blob)
    {
        attachedBlobs.Remove(blob);
    }

    private void DetonateAll()
    {
        if (detonating)
            return;

        detonating = true;

        BlobProjectileScript[] blobs = attachedBlobs.ToArray();
        attachedBlobs.Clear();

        foreach (BlobProjectileScript blob in blobs)
        {
            if (blob != null)
                blob.Pop(true);
        }

        detonating = false;
    }
}