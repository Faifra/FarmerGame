using UnityEngine;

/// <summary>
/// Handles player interaction with the upgrade station and its menu.
/// </summary>
public class UpgradeStationScript : MonoBehaviour, IInteractableScript
{
    // References to the interaction prompt and upgrade menu.
    [Header("UI")]
    [SerializeField] private UpgradeMenuScript upgradeMenu;

    public string InteractionPrompt => "[E Interact]";

    public void Interact()
    {
        if (upgradeMenu != null && !upgradeMenu.IsMenuOpen)
            upgradeMenu.OpenMenu();
    }
}