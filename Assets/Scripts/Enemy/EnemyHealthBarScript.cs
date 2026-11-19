using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject healthBarPrefab;

    [Header("Position")]
    [SerializeField] private float heightOffset = 0.3f;

    [Header("Visibility Distance")]
    [SerializeField] private float visibleDistance = 34f;
    [SerializeField] private float fadeEndDistance = 42f;

    private EnemyHealthScript health;
    private EnemyHealthLevels healthLevels;
    private Collider enemyCollider;
    private GameObject healthBarObject;
    private Slider[] healthSliders;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas parentCanvas;
    private Transform canvasTransform;
    private Camera playerCamera;
    private Transform playerTransform;

    private bool secondSliderRequested;

    public void ShowSecondSlider()
    {
        secondSliderRequested = true;

        if (healthSliders != null && healthSliders.Length > 1)
            healthSliders[1].gameObject.SetActive(true);
    }

    private void Start()
    {
        if (GetComponent<BossUIHelperScript>() != null)
            return;

        health = GetComponent<EnemyHealthScript>();
        healthLevels = GetComponent<EnemyHealthLevels>();
        enemyCollider = health != null ? health.BodyCollider : null;
        playerCamera = Camera.main;

        GameObject canvasObject = GameObject.Find("EnemyHealthBarsCanvas");
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            playerTransform = playerObject.transform;

        if (health == null || healthBarPrefab == null || canvasObject == null || playerCamera == null)
            return;

        parentCanvas = canvasObject.GetComponent<Canvas>();

        if (parentCanvas == null)
            return;

        canvasTransform = parentCanvas.transform;

        healthBarObject = Instantiate(healthBarPrefab, canvasTransform);

        rectTransform = healthBarObject.GetComponent<RectTransform>();
        healthSliders = healthBarObject.GetComponentsInChildren<Slider>(true);
        canvasGroup = healthBarObject.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = healthBarObject.AddComponent<CanvasGroup>();

        if (rectTransform == null || healthSliders.Length == 0)
        {
            Destroy(healthBarObject);
            return;
        }

        for (int i = 0; i < healthSliders.Length; i++)
        {
            Slider slider = healthSliders[i];

            slider.gameObject.SetActive(i == 0 || (i == 1 && secondSliderRequested));

            RectTransform sliderRect = slider.GetComponent<RectTransform>();
            sliderRect.sizeDelta = new Vector2(60f, 10f);

            RectTransform fillArea = slider.fillRect.parent as RectTransform;

            if (fillArea != null)
            {
                fillArea.anchorMin = Vector2.zero;
                fillArea.anchorMax = Vector2.one;
                fillArea.offsetMin = new Vector2(5f, 3f);
                fillArea.offsetMax = new Vector2(-5f, -3f);
            }
        }

        healthSliders[0].minValue = 0;
        healthSliders[0].maxValue = health.MaxHealth;
        healthSliders[0].value = health.CurrentHealth();

        if (healthSliders.Length > 1)
        {
            healthSliders[1].minValue = 0;

            if (healthLevels != null && healthLevels.HealthLayerCount > 1)
            {
                healthSliders[1].maxValue = healthLevels.LayerMaxHealth(1);
                healthSliders[1].value = healthLevels.LayerCurrentHealth(1);
            }
            else
            {
                healthSliders[1].maxValue = 70;
                healthSliders[1].value = 70;
            }
        }

        canvasGroup.alpha = 1f;
    }

    private void LateUpdate()
    {
        if (healthBarObject == null || health == null || playerCamera == null || canvasTransform == null || playerTransform == null)
            return;

        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        if (distanceToPlayer >= fadeEndDistance)
        {
            healthBarObject.SetActive(false);
            return;
        }

        healthBarObject.SetActive(true);

        float fadeAmount = 1f - Mathf.InverseLerp(visibleDistance, fadeEndDistance, distanceToPlayer);

        canvasGroup.alpha = fadeAmount;

        Vector3 worldPosition = enemyCollider != null ? new Vector3(enemyCollider.bounds.center.x, enemyCollider.bounds.max.y, enemyCollider.bounds.center.z) : transform.position + Vector3.up;

        worldPosition += Vector3.up * heightOffset;

        Vector3 screenPosition = playerCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z <= 0f)
        {
            healthBarObject.SetActive(false);
            return;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform as RectTransform, screenPosition, null, out Vector2 localPoint);

        rectTransform.localPosition = localPoint;

        healthSliders[0].value = health.CurrentHealth();

        if (healthSliders.Length > 1 && healthLevels != null)
        {
            if (healthSliders.Length > 1)
            {
                bool hasSecondLayer = healthLevels.HealthLayerCount > 1;
                int secondHealth = hasSecondLayer ? healthLevels.LayerCurrentHealth(1) : 0;

                healthSliders[1].gameObject.SetActive(hasSecondLayer && secondHealth > 0);

                if (hasSecondLayer)
                {
                    healthSliders[1].maxValue = healthLevels.LayerMaxHealth(1);
                    healthSliders[1].value = secondHealth;
                }
            }

            if (healthSliders.Length > 2)
            {
                bool hasThirdLayer = healthLevels.HealthLayerCount > 2;
                int thirdHealth = hasThirdLayer ? healthLevels.LayerCurrentHealth(2) : 0;

                healthSliders[2].gameObject.SetActive(hasThirdLayer && thirdHealth > 0);

                if (hasThirdLayer)
                {
                    healthSliders[2].maxValue = healthLevels.LayerMaxHealth(2);
                    healthSliders[2].value = thirdHealth;
                }
            }
        }

        Vector3 targetPosition = enemyCollider != null ? enemyCollider.bounds.center : transform.position;

        Vector3 direction = targetPosition - playerCamera.transform.position;
        float rayDistance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(playerCamera.transform.position, direction.normalized, rayDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

        foreach (RaycastHit hit in hits)
        {
            Transform hitTransform = hit.transform;

            if (hitTransform == transform || hitTransform.IsChildOf(transform))
                continue;

            if (hitTransform == playerTransform || hitTransform.IsChildOf(playerTransform))
                continue;

            healthBarObject.SetActive(false);
            return;
        }
    }

    private void OnDestroy()
    {
        if (healthBarObject != null)
            Destroy(healthBarObject);
    }
}