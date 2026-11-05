using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteractionScript : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayers = ~0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactionPrompt;

    [Header("Menu")]
    [SerializeField] private UpgradeMenuScript upgradeMenu;

    private InputAction interactAction;
    private IInteractableScript currentInteractable;

    private void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");

        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (playerCamera == null || interactAction == null)
        {
            HidePrompt();
            return;
        }

        if (upgradeMenu != null && upgradeMenu.IsMenuOpen)
        {
            HidePrompt();

            if (interactAction.WasPressedThisFrame())
                upgradeMenu.CloseMenu();

            return;
        }

        FindInteractable();

        if (currentInteractable != null)
        {
            if (interactionPrompt != null)
            {
                interactionPrompt.text = currentInteractable.InteractionPrompt;
                interactionPrompt.gameObject.SetActive(true);
            }

            if (interactAction.WasPressedThisFrame())
                currentInteractable.Interact();
        }
        else
        {
            HidePrompt();
        }
    }

    private void FindInteractable()
    {
        currentInteractable = null;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactionLayers, QueryTriggerInteraction.Ignore))
        {
            return;
        }

        MonoBehaviour[] components = hit.collider.GetComponentsInParent<MonoBehaviour>();

        foreach (MonoBehaviour component in components)
        {
            if (component is IInteractableScript interactable)
            {
                currentInteractable = interactable;
                return;
            }
        }
    }

    private void HidePrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(false);
    }
}