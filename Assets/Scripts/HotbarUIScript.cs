using TMPro;
using UnityEngine;

public class HotbarUIScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerWeaponsScript playerWeapons;

    [Header("Slot Labels (1 to 9)")]
    [SerializeField] private TextMeshProUGUI[] slotLabels;

    private string[] previousLabels = new string[9];

    void Start()
    {
        RefreshHotbar();
    }

    void Update()
    {
        RefreshHotbar();
    }

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
