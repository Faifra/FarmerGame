using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class UpgradeStationScript : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayers = ~0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactionPrompt;
    [SerializeField] private UpgradeMenuScript upgradeMenu;

    private InputAction interactAction;
    private Collider stationCollider;

    private void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
        stationCollider = GetComponentInChildren<Collider>();

        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (playerCamera == null || interactAction == null || upgradeMenu == null || stationCollider == null)
        {
            HidePrompt();
            return;
        }

        if (upgradeMenu.IsMenuOpen)
        {
            HidePrompt();

            if (interactAction.WasPressedThisFrame())
                upgradeMenu.CloseMenu();

            return;
        }

        bool lookingAtStation = IsLookingAtStation();

        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(lookingAtStation);

        if (lookingAtStation && interactAction.WasPressedThisFrame())
            upgradeMenu.OpenMenu();
    }

    private bool IsLookingAtStation()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactionLayers, QueryTriggerInteraction.Ignore))
        {
            return hit.collider == stationCollider || hit.collider.transform.IsChildOf(transform);
        }

        return false;
    }

    private void HidePrompt()
    {
        if (interactionPrompt != null && interactionPrompt.gameObject.activeSelf)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }
}