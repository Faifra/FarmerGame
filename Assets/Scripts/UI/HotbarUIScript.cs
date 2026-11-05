using TMPro;
using UnityEngine;

/// <summary>
/// Updates the hotbar UI to display the assigned weapon in each slot.
/// </summary>
public class HotbarUIScript : MonoBehaviour
{
    // Reference to the player's weapon and hotbar management system.
    [Header("References")]
    [SerializeField] private PlayerWeaponsScript playerWeapons;

    // UI labels for hotbar slots 1 through 9.
    [Header("Slot Labels (1 to 9)")]
    [SerializeField] private TextMeshProUGUI[] slotLabels;

    // Stores previous label values to avoid unnecessary UI updates.
    private string[] previousLabels = new string[9];

    // Initializes the hotbar display.
    void Start()
    {
        RefreshHotbar();
    }

    // Keeps the displayed slots synchronized with the player's hotbar.
    void Update()
    {
        RefreshHotbar();
    }

    // Updates slot labels when their assigned weapon changes.
    private void RefreshHotbar()
    {
        if (playerWeapons == null || slotLabels == null)
            return;

        for (int i = 0; i < slotLabels.Length && i < 9; i++)
        {
            if (slotLabels[i] == null)
                continue;

            string weaponName = playerWeapons.GetHotbarSlotName(i);

            string labelText = string.IsNullOrEmpty(weaponName) ? (i + 1).ToString() : (i + 1) + "\n" + weaponName;

            if (previousLabels[i] != labelText)
            {
                slotLabels[i].text = labelText;
                previousLabels[i] = labelText;
            }
        }
    }
}