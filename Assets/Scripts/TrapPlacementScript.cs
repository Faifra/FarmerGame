using UnityEngine;
using UnityEngine.InputSystem;

public class TrapPlacementScript : MonoBehaviour
{
    [Header("Placement")]
    [SerializeField] private float placementDistance = 2f;
    [SerializeField] private float placementHeight = 2f;
    [SerializeField] private LayerMask groundLayers;

    [Header("Rotation")]
    [SerializeField] private float rotationAmount = 45f;

    [Header("Preview")]
    [SerializeField, Range(0f, 1f)] private float previewTransparency = 0.5f;

    private GameObject previewTrap;

    private bool placingTrap = false;
    private float currentRotation = 0f;

    private InputAction confirmAction;
    private InputAction cancelAction;
    private InputAction rotateAction;

    private void Start()
    {
        confirmAction =
            InputSystem.actions.FindAction("PlaceTrap");

        cancelAction =
            InputSystem.actions.FindAction("CancelTrap");

        rotateAction =
            InputSystem.actions.FindAction("RotateTrap");
    }

    private void Update()
    {
        if (!placingTrap)
            return;

        UpdatePreview();

        if (rotateAction != null && rotateAction.WasPressedThisFrame())
        {
            RotatePreview();
        }

        if (confirmAction != null && confirmAction.WasPressedThisFrame())
        {
            ConfirmPlacement();
        }

        if (cancelAction != null && cancelAction.WasPressedThisFrame())
        {
            CancelPlacement();
        }
    }

    public bool StartPlacement(GameObject trapPrefab)
    {
        if (placingTrap)
            return false;

        if (trapPrefab == null)
            return false;

        previewTrap = Instantiate(trapPrefab);

        TrapBaseScript trap = previewTrap.GetComponent<TrapBaseScript>();

        if (trap != null)
        {
            trap.enabled = false;
        }

        TrapTriggerScript trigger = previewTrap.GetComponent<TrapTriggerScript>();

        if (trigger != null)
        {
            trigger.enabled = false;
        }

        Collider[] colliders = previewTrap.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        SetPreviewTransparency();

        placingTrap = true;
        currentRotation = 0f;

        UpdatePreview();

        return true;
    }

    private void UpdatePreview()
    {
        if (previewTrap == null)
        {
            CancelPlacement();
            return;
        }

        Vector3 startPosition = transform.position + transform.forward * placementDistance + Vector3.up * placementHeight;

        if (!Physics.Raycast(startPosition, Vector3.down, out RaycastHit hit, placementHeight * 2f, groundLayers))
        {
            return;
        }

        Vector3 flatForward = transform.forward;
        flatForward.y = 0f;

        if (flatForward.sqrMagnitude < 0.001f)
            return;

        flatForward.Normalize();

        Quaternion playerRotation = Quaternion.LookRotation(flatForward);

        Quaternion rotation = playerRotation * Quaternion.Euler(0f, currentRotation, 0f);

        previewTrap.transform.SetPositionAndRotation(hit.point, rotation);

        PlacePreviewOnGround(hit.point);
    }

    private void PlacePreviewOnGround(Vector3 groundPoint)
    {
        Renderer[] renderers = previewTrap.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return;

        float lowestPoint = float.MaxValue;

        foreach (Renderer renderer in renderers)
        {
            lowestPoint = Mathf.Min(lowestPoint, renderer.bounds.min.y);
        }

        float offset = groundPoint.y - lowestPoint;

        previewTrap.transform.position += Vector3.up * offset;
    }

    private void SetPreviewTransparency()
    {
        Renderer[] renderers = previewTrap.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;

            foreach (Material material in materials)
            {
                if (material == null)
                    continue;

                SetMaterialTransparent(material, previewTransparency);
            }
        }
    }

    private void SetMaterialTransparent(Material material, float transparency)
    {
        transparency = Mathf.Clamp01(transparency);

        if (material.HasProperty("_Color"))
        {
            Color color = material.color;
            color.a = transparency;
            material.color = color;
        }

        if (material.HasProperty("_BaseColor"))
        {
            Color color = material.GetColor("_BaseColor");
            color.a = transparency;
            material.SetColor("_BaseColor", color);
        }

        if (material.HasProperty("_Surface"))
        {
            material.SetFloat("_Surface", 1f);
        }

        if (material.HasProperty("_Mode"))
        {
            material.SetFloat("_Mode", 3f);
        }

        if (material.HasProperty("_SrcBlend"))
        {
            material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha
            );
        }

        if (material.HasProperty("_DstBlend"))
        {
            material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha
            );
        }

        if (material.HasProperty("_ZWrite"))
        {
            material.SetFloat("_ZWrite", 0f);
        }

        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.EnableKeyword("_ALPHABLEND_ON");

        material.renderQueue = 3000;
    }

    private void RotatePreview()
    {
        currentRotation += rotationAmount;

        if (currentRotation >= 360f)
        {
            currentRotation -= 360f;
        }
    }

    private void ConfirmPlacement()
    {
        if (previewTrap == null)
        {
            CancelPlacement();
            return;
        }

        TrapBaseScript trap = previewTrap.GetComponent<TrapBaseScript>();

        if (trap != null)
        {
            trap.enabled = true;
        }

        TrapTriggerScript trigger = previewTrap.GetComponent<TrapTriggerScript>();

        if (trigger != null)
        {
            trigger.enabled = true;
        }

        Collider[] colliders = previewTrap.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }

        RestoreMaterials();

        previewTrap = null;

        placingTrap = false;
        currentRotation = 0f;

        PlayerWeaponsScript playerWeapons = GetComponent<PlayerWeaponsScript>();

        if (playerWeapons != null)
        {
            playerWeapons.TrapPlacementConfirmed();
        }
    }

    private void RestoreMaterials()
    {
        Renderer[] renderers = previewTrap.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                if (material != null)
                {
                    Destroy(material);
                }
            }
        }
    }

    private void CancelPlacement()
    {
        if (previewTrap != null)
        {
            Destroy(previewTrap);
        }

        previewTrap = null;

        placingTrap = false;
        currentRotation = 0f;
    }

    public bool IsPlacingTrap()
    {
        return placingTrap;
    }
}