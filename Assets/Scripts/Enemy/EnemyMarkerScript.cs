using UnityEngine;
using UnityEngine.UI;

public class EnemyMarkerScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform enemy;
    [SerializeField] private Transform player;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Image markerImage;

    [Header("Marker Settings")]
    [SerializeField] private float visibleDistance = 25f;
    [SerializeField] private float fadeStartDistance = 40f;
    [SerializeField] private float heightOffset = 2.5f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Canvas parentCanvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void Initialize(Transform targetEnemy, Sprite icon)
    {
        enemy = targetEnemy;

        if (markerImage != null)
            markerImage.sprite = icon;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (enemy == null || player == null ||
            playerCamera == null || parentCanvas == null)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        float distance = Vector3.Distance(player.position, enemy.position);

        float fadeAmount = Mathf.InverseLerp(visibleDistance, fadeStartDistance, distance);

        canvasGroup.alpha = fadeAmount;

        Vector3 worldPosition = enemy.position + Vector3.up * heightOffset;
        Vector3 screenPosition = playerCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z <= 0f)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPosition, null, out Vector2 localPoint);

        rectTransform.localPosition = localPoint;
    }
}