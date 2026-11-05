using UnityEngine;

public class HealthStationScript : MonoBehaviour, IInteractableScript
{
    [Header("UI")]
    [SerializeField] private HealthStationMenuScript healthStationMenu;

    public string InteractionPrompt => "[E Interact]";

    public void Interact()
    {
        if (healthStationMenu != null && !healthStationMenu.IsMenuOpen)
            healthStationMenu.OpenMenu();
    }
}