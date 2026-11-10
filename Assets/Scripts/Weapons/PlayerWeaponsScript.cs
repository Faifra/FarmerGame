using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages weapon selection, hotbars, attacks, and trap placement.
/// </summary>
public class PlayerWeaponsScript : MonoBehaviour
{
    // Weapons available to the player at startup.
    [Header("Weapons")]
    [SerializeField] private GameObject[] weapons;

    // Number of slots available in each hotbar.
    private const int HotbarSize = 9;

    // Stores regular weapons and traps separately.
    private GameObject[] primaryHotbar = new GameObject[HotbarSize];
    private GameObject[] trapHotbar = new GameObject[HotbarSize];

    // Input actions for combat, hotbar switching, and slot selection.
    private InputAction attackAction;
    private InputAction weaponScrollAction;
    private InputAction placeTrapAction;
    private InputAction toggleHotbarAction;
    private InputAction reloadAction;
    private InputAction[] selectActions = new InputAction[HotbarSize];

    // Tracks the selected slot and currently active hotbar.
    private int currentSlot = -1;
    private int activeHotbar = 0;

    // Prevents weapon input while a menu is open.
    private bool menuOpen = false;

    // Trap prefabs used when placing purchased traps.
    [Header("Trap Prefabs")]
    [SerializeField] private GameObject launchTrapPrefab;
    [SerializeField] private GameObject pullTrapPrefab;
    [SerializeField] private GameObject beehiveTrapPrefab;
    [SerializeField] private GameObject magnifyingGlassTrapPrefab;
    [SerializeField] private GameObject sprinklerTrapPrefab;

    // References to the trap placement system and trap weapon objects.
    private TrapPlacementScript trapPlacement;

    private GameObject launchTrapWeapon;
    private GameObject pullTrapWeapon;
    private GameObject beehiveTrapWeapon;
    private GameObject magnifyingGlassTrapWeapon;
    private GameObject sprinklerTrapWeapon;

    // Tracks the number of each trap available to place.
    private int launchTrapCount = 0;
    private int pullTrapCount = 0;
    private int beehiveTrapCount = 0;
    private int magnifyingGlassTrapCount = 0;
    private int sprinklerTrapCount = 0;

    private GrappleControllerScript grappleController;

    // Returns the hotbar currently selected by the player.
    private GameObject[] ActiveHotbar => activeHotbar == 0 ? primaryHotbar : trapHotbar;

    // Initializes input actions, sorts weapons into slots, and selects the first usable weapon.
    private void Start()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        weaponScrollAction = InputSystem.actions.FindAction("WeaponScroll");
        placeTrapAction = InputSystem.actions.FindAction("PlaceTrap");
        toggleHotbarAction = InputSystem.actions.FindAction("Hotbar Toggle");
        reloadAction = InputSystem.actions.FindAction("Reload");

        for (int i = 0; i < HotbarSize; i++)
        {
            selectActions[i] = InputSystem.actions.FindAction("Select " + (i + 1));
        }

        trapPlacement = GetComponent<TrapPlacementScript>();
        grappleController = GetComponent<GrappleControllerScript>();

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
                else if (trap is BeehiveTrapScript)
                {
                    beehiveTrapWeapon = weapon;
                }
                else if (trap is MagnifyingGlassTrapScript)
                {
                    magnifyingGlassTrapWeapon = weapon;
                }
                else if (trap is SprinklerTrapScript)
                {
                    sprinklerTrapWeapon = weapon;
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

    // Processes weapon input while gameplay is active and trap placement is inactive.
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

        if (reloadAction != null && reloadAction.WasPressedThisFrame())
        {
            ReloadCurrentWeapon();
        }

        if (attackAction != null && attackAction.WasPressedThisFrame())
        {
            Attack();
        }

        if (attackAction != null && attackAction.IsPressed())
        {
            HoldAttack();
        }

        if (attackAction != null && attackAction.WasReleasedThisFrame())
        {
            StopAttack();
        }

        if (placeTrapAction != null && placeTrapAction.WasPressedThisFrame())
        {
            PlaceTrap();
        }
    }

    // Reloads the currently selected ranged weapon.
    private void ReloadCurrentWeapon()
    {
        if (currentSlot < 0)
            return;

        GameObject weapon = ActiveHotbar[currentSlot];

        if (weapon == null)
            return;

        RangedWeaponScript rangedWeapon = weapon.GetComponent<RangedWeaponScript>();

        if (rangedWeapon != null)
            rangedWeapon.Reload();
    }

    // Enables or disables weapon input when menus open or close.
    public void SetMenuOpen(bool open)
    {
        menuOpen = open;
    }

    // Adds a launch trap to the rightmost available trap hotbar slot.
    public void AddLaunchTrap()
    {
        int slot = FindRightmostEmptySlot(trapHotbar);

        if (slot == -1)
            return;

        if (launchTrapWeapon == null || launchTrapPrefab == null)
            return;

        trapHotbar[slot] = launchTrapWeapon;
        launchTrapCount++;
    }

    // Adds a pull trap to the rightmost available trap hotbar slot.
    public void AddPullTrap()
    {
        int slot = FindRightmostEmptySlot(trapHotbar);

        if (slot == -1)
            return;

        if (pullTrapWeapon == null || pullTrapPrefab == null)
            return;

        trapHotbar[slot] = pullTrapWeapon;
        pullTrapCount++;
    }

    // Adds a beehive trap to the rightmost available trap hotbar slot.
    public void AddBeehiveTrap()
    {
        int slot = FindRightmostEmptySlot(trapHotbar);

        if (slot == -1)
            return;

        if (beehiveTrapWeapon == null || beehiveTrapPrefab == null)
            return;

        trapHotbar[slot] = beehiveTrapWeapon;
        beehiveTrapCount++;
    }

    // Adds a magnifying glass trap to the rightmost available trap hotbar slot.
    public void AddMagnifyingGlassTrap()
    {
        int slot = FindRightmostEmptySlot(trapHotbar);

        if (slot == -1)
            return;

        if (magnifyingGlassTrapWeapon == null || magnifyingGlassTrapPrefab == null)
            return;

        trapHotbar[slot] = magnifyingGlassTrapWeapon;
        magnifyingGlassTrapCount++;
    }

    // Adds a sprinkler trap to the rightmost available trap hotbar slot.
    public void AddSprinklerTrap()
    {
        int slot = FindRightmostEmptySlot(trapHotbar);

        if (slot == -1)
            return;

        if (sprinklerTrapWeapon == null || sprinklerTrapPrefab == null)
            return;

        trapHotbar[slot] = sprinklerTrapWeapon;
        sprinklerTrapCount++;
    }

    // Finds the last empty slot so newly added traps appear from right to left.
    private int FindRightmostEmptySlot(GameObject[] hotbar)
    {
        for (int i = hotbar.Length - 1; i >= 0; i--)
        {
            if (hotbar[i] == null)
                return i;
        }

        return -1;
    }

    // Switches between the primary and trap hotbars.
    private void ToggleHotbar()
    {
        activeHotbar = 1 - activeHotbar;
        SelectFirstAvailableSlot();
    }

    // Selects the first usable slot or clears the selection if none are available.
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

    // Activates the selected weapon and deactivates all others.
    private void SelectSlot(int slot)
    {
        if (!CanUseSlot(slot))
            return;

        GameObject[] oldHotbar = ActiveHotbar;
        GameObject currentWeapon = null;

        if (currentSlot >= 0 && currentSlot < oldHotbar.Length)
            currentWeapon = oldHotbar[currentSlot];

        GameObject newWeapon = oldHotbar[slot];

        if (currentWeapon != newWeapon && currentWeapon != null && currentWeapon.GetComponent<GrapplingWeaponScript>() != null && grappleController != null)
        {
            grappleController.OnWeaponSwitch();
        }

        currentSlot = slot;

        DeactivateAllWeapons();

        if (newWeapon != null)
            newWeapon.SetActive(true);
    }

    // Checks whether a slot contains a weapon or a trap with remaining inventory.
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

        if (trap is BeehiveTrapScript)
            return beehiveTrapCount > 0;

        if (trap is MagnifyingGlassTrapScript)
            return magnifyingGlassTrapCount > 0;

        if (trap is SprinklerTrapScript)
            return sprinklerTrapCount > 0;

        return true;
    }

    // Cycles through the hotbar until a usable slot is found.
    private void CycleWeapons(int direction)
    {
        if (currentSlot == -1)
        {
            SelectFirstAvailableSlot();
            return;
        }

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

    // Returns the display name of a weapon or trap for the hotbar UI.
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

        if (trap is BeehiveTrapScript)
            return "Beehive";

        if (trap is MagnifyingGlassTrapScript)
            return "Magnifying Glass";

        if (trap is SprinklerTrapScript)
            return "Sprinkler Trap";

        return weapon.name;
    }

    // Returns the index of the currently selected hotbar slot.
    public int GetCurrentHotbarSlot()
    {
        return currentSlot;
    }

    public RangedWeaponScript GetCurrentRangedWeapon()
    {
        if (currentSlot < 0)
            return null;

        GameObject[] hotbar = ActiveHotbar;

        if (currentSlot >= hotbar.Length)
            return null;

        GameObject weapon = hotbar[currentSlot];

        if (weapon == null)
            return null;

        return weapon.GetComponent<RangedWeaponScript>();
    }

    // Disables all weapon objects before another weapon is selected.
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

    // Executes the selected weapon's attack.
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

    // Handles continuous attacks for ranged weapons while the attack input is held.
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

    // Stops continuous firing when the attack button is released.
    private void StopAttack()
    {
        if (currentSlot < 0)
            return;

        GameObject weapon = ActiveHotbar[currentSlot];

        if (weapon == null)
            return;

        RangedWeaponScript rangedWeapon = weapon.GetComponent<RangedWeaponScript>();

        if (rangedWeapon != null)
            rangedWeapon.StopAttack();

        MeleeWeaponScript meleeWeapon = weapon.GetComponent<MeleeWeaponScript>();

        if (meleeWeapon != null)
        {
            meleeWeapon.ReleaseChargeAttack();
            return;
        }
    }

    // Starts placement using the prefab matching the selected trap type.
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
        else if (trap is BeehiveTrapScript && beehiveTrapCount > 0)
        {
            trapPlacement.StartPlacement(beehiveTrapPrefab);
        }
        else if (trap is MagnifyingGlassTrapScript && magnifyingGlassTrapCount > 0)
        {
            trapPlacement.StartPlacement(magnifyingGlassTrapPrefab);
        }
        else if (trap is SprinklerTrapScript &&  sprinklerTrapCount > 0)
        {
            trapPlacement.StartPlacement(sprinklerTrapPrefab);
        }
    }

    // Consumes one trap after placement and selects the next available slot.
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
        else if (trap is BeehiveTrapScript && beehiveTrapCount > 0)
        {
            beehiveTrapCount--;
            hotbar[currentSlot] = null;
        }
        else if (trap is MagnifyingGlassTrapScript && magnifyingGlassTrapCount > 0)
        {
            magnifyingGlassTrapCount--;
            hotbar[currentSlot] = null;
        }
        else if (trap is SprinklerTrapScript && sprinklerTrapCount > 0)
        {
            sprinklerTrapCount--;
            hotbar[currentSlot] = null;
        }
        else
        {
            return;
        }

        SelectFirstAvailableSlot();
    }
}