using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponsScript : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;

    private InputAction previousAction;
    private InputAction nextAction;
    private InputAction attackAction;

    private int currentWeapon = 0;

    private void Start()
    {
        previousAction = InputSystem.actions.FindAction("Previous");
        nextAction = InputSystem.actions.FindAction("Next");
        attackAction = InputSystem.actions.FindAction("Attack");

        SelectWeapon(currentWeapon);
    }

    private void Update()
    {
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
    }

    private void SelectPreviousWeapon()
    {
        currentWeapon--;

        if (currentWeapon < 0)
        {
            currentWeapon = weapons.Length - 1;
        }

        SelectWeapon(currentWeapon);
    }

    private void SelectNextWeapon()
    {
        currentWeapon++;

        if (currentWeapon >= weapons.Length)
        {
            currentWeapon = 0;
        }

        SelectWeapon(currentWeapon);
    }

    private void SelectWeapon(int index)
    {
        if (weapons == null || weapons.Length == 0)
            return;

        if (index < 0 || index >= weapons.Length)
            return;

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == index);
        }
    }

    private void Attack()
    {
        WeaponScript weapon = weapons[currentWeapon].GetComponent<WeaponScript>();

        if (weapon != null)
        {
            weapon.Attack();
        }
    }
}
