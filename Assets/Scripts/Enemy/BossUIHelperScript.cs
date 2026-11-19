using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUIHelperScript : MonoBehaviour
{
    [Header("Health Bar Settings")]
    [SerializeField] private float heightOffset = 0.5f;
    [SerializeField] private float referenceDistance = 10f;

    private GameObject bossBarUIObject;
    private TMP_Text bossText;
    private Slider bossHPBar;
    private RectTransform barRect;
    private RectTransform collectionRect;
    private Canvas parentCanvas;
    private EnemyHealthScript healthScript;
    private Camera playerCamera;
    private Collider enemyCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        GameObject collection = GameObject.FindGameObjectWithTag("BossBarCollection");

        if (collection == null)
            return;
        
        parentCanvas = collection.GetComponentInParent<Canvas>();
        collectionRect = collection.GetComponent<RectTransform>();

        healthScript = GetComponent<EnemyHealthScript>();
        enemyCollider = healthScript.BodyCollider;
        playerCamera = Camera.main;

        bossBarUIObject = Instantiate(Resources.Load<GameObject>("BossBar"), collection.transform);

        barRect = bossBarUIObject.GetComponent<RectTransform>();

        bossText = bossBarUIObject.GetComponentInChildren<TMP_Text>();
        bossHPBar = bossBarUIObject.GetComponentInChildren<Slider>();
    }

    private void LateUpdate()
    {
        if (bossBarUIObject == null || healthScript == null || playerCamera == null || collectionRect == null)
            return;

        float enemyHeight = enemyCollider != null ? enemyCollider.bounds.max.y - transform.position.y : heightOffset;

        Vector3 worldPosition = transform.position + Vector3.up * (enemyHeight + heightOffset);

        Vector3 screenPosition = playerCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z <= 0f)
        {
            bossBarUIObject.SetActive(false);
            return;
        }

        bossBarUIObject.SetActive(true);

        Camera canvasCamera = null;

        if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            canvasCamera = parentCanvas.worldCamera;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(collectionRect, screenPosition, canvasCamera, out Vector2 localPoint);

        barRect.localPosition = localPoint;

        float distanceFromCamera = Vector3.Distance(playerCamera.transform.position, worldPosition);

        float scaleFactor = referenceDistance / distanceFromCamera;
        scaleFactor = Mathf.Clamp(scaleFactor, 0.3f, 1f);

        barRect.localScale = Vector3.one * scaleFactor;

        bossText.text = healthScript.Type.ToString();
        bossHPBar.maxValue = healthScript.MaxHealth;
        bossHPBar.value = healthScript.CurrentHealth();
    }

    public void RemoveHealthBar()
    {
        if (bossBarUIObject != null)
            Destroy(bossBarUIObject);
    }

}
