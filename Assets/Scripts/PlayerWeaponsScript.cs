using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponsScript : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;

    private InputAction previousAction;
    private InputAction nextAction;
    private InputAction attackAction;
    private InputAction weaponScrollAction;

    private int currentWeapon = 0;

    private bool menuOpen = false;

    [Header("Trap Prefabs")]
    [SerializeField] private GameObject launchTrapPrefab;
    [SerializeField] private GameObject pullTrapPrefab;

    private TrapPlacementScript trapPlacement;
    private InputAction placeTrapAction;

    [Header("Trap Inventory")]
    private int launchTrapCount = 0;
    private int pullTrapCount = 0;

    private void Start()
    {
        previousAction = InputSystem.actions.FindAction("Previous");
        nextAction = InputSystem.actions.FindAction("Next");
        attackAction = InputSystem.actions.FindAction("Attack");
        weaponScrollAction = InputSystem.actions.FindAction("WeaponScroll");

        trapPlacement = GetComponent<TrapPlacementScript>();
        placeTrapAction = InputSystem.actions.FindAction("Attack");

        SelectWeapon(currentWeapon);
    }

    private void Update()
    {
        if (menuOpen)
            return;

        if (previousAction.WasPressedThisFrame())
        {
            SelectPreviousWeapon();
        }

        if (nextAction.WasPressedThisFrame())
        {
            SelectNextWeapon();
        }

        if (attackAction.WasPressedThisFrame())
        {
            Attack();
        }

        if (attackAction.IsPressed())
        {
            HoldAttack();
        }

        float scroll = weaponScrollAction.ReadValue<float>();

        if (scroll > 0)
        {
            SelectNextWeapon();
        }
        else if (scroll < 0)
        {
            SelectPreviousWeapon();
        }

        if (placeTrapAction.WasPressedThisFrame())
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
        launchTrapCount++;
    }

    public void AddPullTrap()
    {
        pullTrapCount++;
    }

    private bool CanUseWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length)
            return false;

        // Launch Trap
        if (index == 3)
        {
            return launchTrapCount > 0;
        }

        // Pull Trap
        if (index == 4)
        {
            return pullTrapCount > 0;
        }

        return true;
    }

    private void SelectPreviousWeapon()
    {
        int newWeapon = currentWeapon - 1;

        if (newWeapon < 0)
        {
            newWeapon = weapons.Length - 1;
        }

        for (int i = 0; i < weapons.Length; i++)
        {
            if (CanUseWeapon(newWeapon))
            {
                currentWeapon = newWeapon;
                SelectWeapon(currentWeapon);
                return;
            }

            newWeapon--;

            if (newWeapon < 0)
            {
                newWeapon = weapons.Length - 1;
            }
        }
    }

    private void SelectNextWeapon()
    {
        int newWeapon = currentWeapon + 1;

        if (newWeapon >= weapons.Length)
        {
            newWeapon = 0;
        }

        for (int i = 0; i < weapons.Length; i++)
        {
            if (CanUseWeapon(newWeapon))
            {
                currentWeapon = newWeapon;
                SelectWeapon(currentWeapon);
                return;
            }

            newWeapon++;

            if (newWeapon >= weapons.Length)
            {
                newWeapon = 0;
            }
        }
    }

    private void SelectWeapon(int index)
    {
        if (weapons == null || weapons.Length == 0)
            return;

        if (!CanUseWeapon(index))
            return;

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == index);
        }
    }

    private void Attack()
    {
        if (weapons == null || currentWeapon < 0 || currentWeapon >= weapons.Length)
        {
            return;
        }

        WeaponScript weapon = weapons[currentWeapon].GetComponent<WeaponScript>();

        if (weapon != null)
        {
            weapon.Attack();
        }
    }

    private void HoldAttack()
    {
        if (weapons == null || currentWeapon < 0 || currentWeapon >= weapons.Length)
        {
            return;
        }

        RangedWeaponScript rangedWeapon = weapons[currentWeapon].GetComponent<RangedWeaponScript>();

        if (rangedWeapon != null)
        {
            rangedWeapon.HoldAttack();
        }
    }

    private void PlaceTrap()
    {
        if (trapPlacement == null)
            return;

        TrapBaseScript trap = weapons[currentWeapon].GetComponent<TrapBaseScript>();

        if (trap == null)
            return;

        if (trap is LaunchTrapScript)
        {
            if (launchTrapCount <= 0)
                return;

            trapPlacement.PlaceTrap(launchTrapPrefab);

            launchTrapCount--;

            if (launchTrapCount <= 0)
            {
                SelectWeaponAfterTrapUsed();
            }
        }
        else if (trap is PullTrapScript)
        {
            if (pullTrapCount <= 0)
                return;

            trapPlacement.PlaceTrap(pullTrapPrefab);

            pullTrapCount--;

            if (pullTrapCount <= 0)
            {
                SelectWeaponAfterTrapUsed();
            }
        }
    }

    private void SelectWeaponAfterTrapUsed()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (CanUseWeapon(i))
            {
                currentWeapon = i;
                SelectWeapon(currentWeapon);
                return;
            }
        }
    }
}