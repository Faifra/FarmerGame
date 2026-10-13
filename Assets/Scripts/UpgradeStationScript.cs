using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Handles player interaction with the upgrade station and its menu.
/// </summary>
public class UpgradeStationScript : MonoBehaviour
{
    // Controls the camera, interaction distance, and valid interaction layers.
    [Header("Interaction")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayers = ~0;

    // References to the interaction prompt and upgrade menu.
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactionPrompt;
    [SerializeField] private UpgradeMenuScript upgradeMenu;

    // Cached input action and station collider.
    private InputAction interactAction;
    private Collider stationCollider;

    // Initializes interaction references and hides the prompt.
    private void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
        stationCollider = GetComponentInChildren<Collider>();

        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(false);
    }

    // Updates the prompt and handles opening or closing the menu.
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

    // Checks whether the camera ray hits the upgrade station within range.
    private bool IsLookingAtStation()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactionLayers, QueryTriggerInteraction.Ignore))
        {
            return hit.collider == stationCollider || hit.collider.transform.IsChildOf(transform);
        }

        return false;
    }

    // Hides the interaction prompt when the station cannot be interacted with.
    private void HidePrompt()
    {
        if (interactionPrompt != null && interactionPrompt.gameObject.activeSelf)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }
}