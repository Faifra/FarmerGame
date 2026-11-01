
using TMPro;
using UnityEngine;

public class AmmoUIScript : MonoBehaviour
{
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private PlayerWeaponsScript playerWeapons;

    private void Update()
    {
        if (ammoText == null || playerWeapons == null)
            return;

        RangedWeaponScript weapon = playerWeapons.GetCurrentRangedWeapon();

        if (weapon == null)
        {
            ammoText.text = "";
            return;
        }

        if (weapon.IsReloading)
        {
            ammoText.text = "Reloading...";
            return;
        }

        ammoText.text = $"{weapon.CurrentAmmo} / O-O";
    }
}