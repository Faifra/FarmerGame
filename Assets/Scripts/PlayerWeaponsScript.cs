using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponsScript : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private GameObject[] weapons;

    private const int HotbarSize = 9;

    private GameObject[] primaryHotbar = new GameObject[HotbarSize];
    private GameObject[] trapHotbar = new GameObject[HotbarSize];

    private InputAction attackAction;
    private InputAction weaponScrollAction;
    private InputAction placeTrapAction;
    private InputAction toggleHotbarAction;
    private InputAction[] selectActions = new InputAction[HotbarSize];

    private int currentSlot = -1;
    private int activeHotbar = 0;

    private bool menuOpen = false;

    [Header("Trap Prefabs")]
    [SerializeField] private GameObject launchTrapPrefab;
    [SerializeField] private GameObject pullTrapPrefab;

    private TrapPlacementScript trapPlacement;

    private GameObject launchTrapWeapon;
    private GameObject pullTrapWeapon;

    private int launchTrapCount = 0;
    private int pullTrapCount = 0;

    private GameObject[] ActiveHotbar => activeHotbar == 0 ? primaryHotbar : trapHotbar;

    private void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        weaponScrollAction = InputSystem.actions.FindAction("WeaponScroll");
        placeTrapAction = InputSystem.actions.FindAction("PlaceTrap");
        toggleHotbarAction = InputSystem.actions.FindAction("Hotbar Toggle");

        for (int i = 0; i < HotbarSize; i++)
        {
            selectActions[i] = InputSystem.actions.FindAction("Select " + (i + 1));
        }

        trapPlacement = GetComponent<TrapPlacementScript>();

        int nextSlot = 0;

        if (weapons != null)
        {
            foreach (GameObject weapon in weapons)
            {
                if (weapon == null)
                    continue;

                TrapBaseScript trap = weapon.GetComponent<TrapBaseScript>();

                if (trap is LaunchTrapScript)
                {
                    launchTrapWeapon = weapon;
                }
                else if (trap is PullTrapScript)
                {
                    pullTrapWeapon = weapon;
                }
                else if (nextSlot < HotbarSize)
                {
                    primaryHotbar[nextSlot] = weapon;
                    nextSlot++;
                }

                weapon.SetActive(false);
            }
        }

        SelectFirstAvailableSlot();
    }

    private void Update()
    {
        if (menuOpen)
            return;

        if (trapPlacement != null && trapPlacement.IsPlacingTrap())
            return;

        if (toggleHotbarAction != null && toggleHotbarAction.WasPressedThisFrame())
        {
            ToggleHotbar();
        }

        for (int i = 0; i < selectActions.Length; i++)
        {
            if (selectActions[i] != null && selectActions[i].WasPressedThisFrame())
            {
                SelectSlot(i);
            }
        }

        float scroll = weaponScrollAction != null ? weaponScrollAction.ReadValue<float>() : 0f;

        if (scroll > 0f)
        {
            CycleWeapons(1);
        }
        else if (scroll < 0f)
        {
            CycleWeapons(-1);
        }

        if (attackAction != null && attackAction.WasPressedThisFrame())
        {
            Attack();
        }

        if (attackAction != null && attackAction.IsPressed())
        {
            HoldAttack();
        }

        if (placeTrapAction != null && placeTrapAction.WasPressedThisFrame())
        {
            PlaceTrap();
        }
    }

    public void SetMenuOpen(bool open)
    {
        menuOpen = open;
    }

    public void AddLaunchTrap()
    {
        int slot = FindRightmostEmptySlot(primaryHotbar);

        if (slot == -1)
        {
            return;
        }

        if (launchTrapWeapon == null || launchTrapPrefab == null)
        {
            return;
        }

        primaryHotbar[slot] = launchTrapWeapon;
        launchTrapCount++;
    }

    public void AddPullTrap()
    {
        int slot = FindRightmostEmptySlot(primaryHotbar);

        if (slot == -1)
        {
            return;
        }

        if (pullTrapWeapon == null || pullTrapPrefab == null)
        {
            return;
        }

        primaryHotbar[slot] = pullTrapWeapon;
        pullTrapCount++;
    }

    private int FindRightmostEmptySlot(GameObject[] hotbar)
    {
        for (int i = hotbar.Length - 1; i >= 0; i--)
        {
            if (hotbar[i] == null)
                return i;
        }

        return -1;
    }

    private void ToggleHotbar()
    {
        activeHotbar = 1 - activeHotbar;

        SelectFirstAvailableSlot();
    }

    private void SelectFirstAvailableSlot()
    {
        GameObject[] hotbar = ActiveHotbar;

        for (int i = 0; i < hotbar.Length; i++)
        {
            if (CanUseSlot(i))
            {
                SelectSlot(i);
                return;
            }
        }

        currentSlot = -1;
        DeactivateAllWeapons();
    }

    private void SelectSlot(int slot)
    {
        if (!CanUseSlot(slot))
            return;

        currentSlot = slot;
        DeactivateAllWeapons();

        GameObject weapon = ActiveHotbar[currentSlot];

        if (weapon != null)
            weapon.SetActive(true);
    }

    private bool CanUseSlot(int slot)
    {
        if (slot < 0 || slot >= HotbarSize)
            return false;

        GameObject weapon = ActiveHotbar[slot];

        if (weapon == null)
            return false;

        TrapBaseScript trap = weapon.GetComponent<TrapBaseScript>();

        if (trap is LaunchTrapScript)
            return launchTrapCount > 0;

        if (trap is PullTrapScript)
            return pullTrapCount > 0;

        return true;
    }

    private void CycleWeapons(int direction)
    {
        if (currentSlot == -1)
        {
            SelectFirstAvailableSlot();
            return;
        }

        GameObject[] hotbar = ActiveHotbar;

        for (int i = 1; i <= HotbarSize; i++)
        {
            int nextSlot = (currentSlot + direction * i + HotbarSize * 2) % HotbarSize;

            if (CanUseSlot(nextSlot))
            {
                SelectSlot(nextSlot);
                return;
            }
        }
    }

    public string GetHotbarSlotName(int slot)
    {
        if (slot < 0 || slot >= HotbarSize)
            return "";

        GameObject weapon = ActiveHotbar[slot];

        if (weapon == null)
            return "";

        TrapBaseScript trap = weapon.GetComponent<TrapBaseScript>();

        if (trap is LaunchTrapScript)
            return "Launch Trap";

        if (trap is PullTrapScript)
            return "Pull Trap";

        return weapon.name;
    }

    public int GetCurrentHotbarSlot()
    {
        return currentSlot;
    }

    private void DeactivateAllWeapons()
    {
        if (weapons == null)
            return;

        foreach (GameObject weapon in weapons)
        {
            if (weapon != null)
                weapon.SetActive(false);
        }
    }

    private void Attack()
    {
        if (currentSlot < 0)
            return;

        GameObject weapon = ActiveHotbar[currentSlot];

        if (weapon == null)
            return;

        WeaponScript weaponScript = weapon.GetComponent<WeaponScript>();

        if (weaponScript != null)
            weaponScript.Attack();
    }

    private void HoldAttack()
    {
        if (currentSlot < 0)
            return;

        GameObject weapon = ActiveHotbar[currentSlot];

        if (weapon == null)
            return;

        RangedWeaponScript rangedWeapon = weapon.GetComponent<RangedWeaponScript>();

        if (rangedWeapon != null)
            rangedWeapon.HoldAttack();
    }

    private void PlaceTrap()
    {
        if (trapPlacement == null || currentSlot < 0)
            return;

        GameObject weapon = ActiveHotbar[currentSlot];

        if (weapon == null)
            return;

        TrapBaseScript trap = weapon.GetComponent<TrapBaseScript>();

        if (trap is LaunchTrapScript && launchTrapCount > 0)
        {
            trapPlacement.StartPlacement(launchTrapPrefab);
        }
        else if (trap is PullTrapScript && pullTrapCount > 0)
        {
            trapPlacement.StartPlacement(pullTrapPrefab);
        }
    }

    public void TrapPlacementConfirmed()
    {
        if (currentSlot < 0)
            return;

        GameObject[] hotbar = ActiveHotbar;
        GameObject weapon = hotbar[currentSlot];

        if (weapon == null)
            return;

        TrapBaseScript trap = weapon.GetComponent<TrapBaseScript>();

        if (trap is LaunchTrapScript && launchTrapCount > 0)
        {
            launchTrapCount--;
            hotbar[currentSlot] = null;
        }
        else if (trap is PullTrapScript && pullTrapCount > 0)
        {
            pullTrapCount--;
            hotbar[currentSlot] = null;
        }
        else
        {
            return;
        }

        SelectFirstAvailableSlot();
    }
}