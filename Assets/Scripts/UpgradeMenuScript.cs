using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class UpgradeMenuScript : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private GameObject upgradeMenu;

    [Header("Player")]
    [SerializeField] private PlayerResourceScript playerResources;
    [SerializeField] private PlayerLookScript playerLook;
    [SerializeField] private PlayerWeaponsScript playerWeapons;

    [Header("Input")]
    private InputAction upgradeMenuAction;

    [Header("Launch Trap Upgrade")]
    [SerializeField] private int launchTrapCost = 10;
    [SerializeField] private Button launchTrapButton;
    [SerializeField] private TextMeshProUGUI launchTrapCostText;

    [Header("Pull Trap Upgrade")]
    [SerializeField] private int pullTrapCost = 15;
    [SerializeField] private Button pullTrapButton;
    [SerializeField] private TextMeshProUGUI pullTrapCostText;

    private bool menuOpen = false;

    private void Start()
    {
        upgradeMenuAction = InputSystem.actions.FindAction("Interact");

        upgradeMenu.SetActive(false);

        if (launchTrapCostText != null)
        {
            launchTrapCostText.text = launchTrapCost.ToString();
        }

        if (pullTrapCostText != null)
        {
            pullTrapCostText.text = pullTrapCost.ToString();
        }

        UpdateButton();
    }

    private void Update()
    {
        if (upgradeMenuAction.WasPressedThisFrame())
        {
            ToggleMenu();
        }

        if (menuOpen)
        {
            UpdateButton();
        }
    }

    private void ToggleMenu()
    {
        menuOpen = !menuOpen;

        upgradeMenu.SetActive(menuOpen);

        if (playerLook != null)
        {
            playerLook.SetMenuOpen(menuOpen);
        }

        if (playerWeapons != null)
        {
            playerWeapons.SetMenuOpen(menuOpen);
        }
    }

    public void BuyLaunchTrap()
    {
        if (playerResources == null)
            return;
        
        if (playerWeapons == null)
            return;
        
        bool purchased = playerResources.SpendDrops(launchTrapCost);
        
        if (!purchased)
        {
            return;
        }
        
        playerWeapons.AddLaunchTrap();

        UpdateButton();
    }

    public void BuyPullTrap()
    {
        if (playerResources == null)
            return;
        
        if (playerWeapons == null)
            return;
        
        bool purchased = playerResources.SpendDrops(pullTrapCost);
        
        if (!purchased)
        {
            return;
        }
        
        playerWeapons.AddPullTrap();

        UpdateButton();
    }

    private void UpdateButton()
    {
        if (launchTrapButton == null || playerResources == null)
            return;

        launchTrapButton.interactable = playerResources.TotalDrops >= launchTrapCost;

        if (pullTrapButton == null || playerResources == null)
            return;

        pullTrapButton.interactable = playerResources.TotalDrops >= pullTrapCost;
    }
}