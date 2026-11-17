using UnityEngine;

public class EnemyMarkerSpawnerScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject markerPrefab;

    [Header("Enemy Icon")]
    [SerializeField] private Sprite enemyIcon;

    private void Start()
    {
        GameObject canvasObject = GameObject.Find("EnemyMarkersCanvas");

        if (markerPrefab == null || canvasObject == null)
        {
            return;
        }

        Canvas markerCanvas = canvasObject.GetComponent<Canvas>();

        if (markerCanvas == null)
        {
            return;
        }

        GameObject markerObject = Instantiate(markerPrefab, markerCanvas.transform);

        EnemyMarkerScript marker = markerObject.GetComponent<EnemyMarkerScript>();

        if (marker != null)
            marker.Initialize(transform, enemyIcon);
    }
}