using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages the upgrade menu and purchases traps using player resources.
/// </summary>
public class UpgradeMenuScript : MonoBehaviour
{
    // Reference to the upgrade menu UI.
    [Header("Menu")]
    [SerializeField] private GameObject upgradeMenu;

    // Player components affected by opening and using the menu.
    [Header("Player")]
    [SerializeField] private PlayerResourceScript playerResources;
    [SerializeField] private PlayerLookScript playerLook;
    [SerializeField] private PlayerWeaponsScript playerWeapons;

    // Launch trap purchase settings and UI references.
    [Header("Launch Trap Upgrade")]
    [SerializeField] private int launchTrapCost = 10;
    [SerializeField] private Button launchTrapButton;
    [SerializeField] private TextMeshProUGUI launchTrapCostText;

    // Pull trap purchase settings and UI references.
    [Header("Pull Trap Upgrade")]
    [SerializeField] private int pullTrapCost = 15;
    [SerializeField] private Button pullTrapButton;
    [SerializeField] private TextMeshProUGUI pullTrapCostText;

    // Tracks whether the upgrade menu is currently open.
    private bool menuOpen = false;

    // Allows other scripts to check the menu state.
    public bool IsMenuOpen => menuOpen;

    // Initializes the menu and displays trap prices.
    private void Start()
    {
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

    // Keeps purchase button availability in sync with player resources.
    private void Update()
    {
        if (menuOpen)
        {
            UpdateButton();
        }
    }

    // Opens the menu and notifies player controls.
    public void OpenMenu()
    {
        if (menuOpen)
            return;

        menuOpen = true;
        upgradeMenu.SetActive(true);

        if (playerLook != null)
            playerLook.SetMenuOpen(true);

        if (playerWeapons != null)
            playerWeapons.SetMenuOpen(true);

    }

    // Closes the menu and restores player controls.
    public void CloseMenu()
    {
        if (!menuOpen)
            return;

        menuOpen = false;
        upgradeMenu.SetActive(false);

        if (playerLook != null)
            playerLook.SetMenuOpen(false);

        if (playerWeapons != null)
            playerWeapons.SetMenuOpen(false);

    }

    // Purchases a launch trap if the player can afford it.
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

    // Purchases a pull trap if the player can afford it.
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

    // Updates purchase button availability based on current resources.
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