using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages the upgrade menu and purchases traps using player resources.
/// </summary>
public class UpgradeMenuScript : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private GameObject hotbarUI;
    [SerializeField] private GameObject ammoTextUI;

    // Reference to the upgrade menu UI.
    [Header("Menu")]
    [SerializeField] private GameObject upgradeMenu;

    [Header("Tabs")]
    [SerializeField] private Button purchaseTrapsTabButton;
    [SerializeField] private Button upgradeTrapsTabButton;
    [SerializeField] private Button upgradeWeaponsTabButton;

    [SerializeField] private GameObject purchaseTrapsPanel;
    [SerializeField] private GameObject upgradeTrapsPanel;
    [SerializeField] private GameObject upgradeWeaponsPanel;

    [Header("Tab Colors")]
    [SerializeField] private Color selectedTabColor = new Color(183f / 255f, 121f / 255f, 54f / 255f);
    [SerializeField] private Color normalTabColor = new Color(52f / 255f, 70f / 255f, 79f / 255f);

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

    // Beehive trap purchase settings and UI references.
    [Header("Beehive Trap Upgrade")]
    [SerializeField] private int beehiveTrapCost = 10;
    [SerializeField] private Button beehiveTrapButton;
    [SerializeField] private TextMeshProUGUI beehiveTrapCostText;

    // Magnifying glass trap purchase settings and UI references.
    [Header("Magnifying Glass Trap Upgrade")]
    [SerializeField] private int magnifyingGlassTrapCost = 10;
    [SerializeField] private Button magnifyingGlassTrapButton;
    [SerializeField] private TextMeshProUGUI magnifyingGlassTrapCostText;

    // Magnifying glass trap purchase settings and UI references.
    [Header("Sprinkler Trap Upgrade")]
    [SerializeField] private int sprinklerTrapCost = 10;
    [SerializeField] private Button sprinklerTrapButton;
    [SerializeField] private TextMeshProUGUI sprinklerTrapCostText;

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

        if (beehiveTrapCostText != null)
        {
            beehiveTrapCostText.text = beehiveTrapCost.ToString();
        }

        if (magnifyingGlassTrapCostText != null)
        {
            magnifyingGlassTrapCostText.text = magnifyingGlassTrapCost.ToString();
        }

        if (sprinklerTrapCostText != null)
        {
            sprinklerTrapCostText.text = sprinklerTrapCost.ToString();
        }

        UpdateButton();

        ShowPurchaseTrapsTab();
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

        if (hotbarUI != null)
            hotbarUI.SetActive(false);

        if (ammoTextUI != null)
            ammoTextUI.SetActive(false);

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

        if (hotbarUI != null)
            hotbarUI.SetActive(true);

        if (ammoTextUI != null)
            ammoTextUI.SetActive(true);

        if (playerLook != null)
            playerLook.SetMenuOpen(false);

        if (playerWeapons != null)
            playerWeapons.SetMenuOpen(false);

    }

    public void ShowPurchaseTrapsTab()
    {
        SetActiveTab(purchaseTrapsPanel, purchaseTrapsTabButton);
    }

    public void ShowUpgradeTrapsTab()
    {
        SetActiveTab(upgradeTrapsPanel, upgradeTrapsTabButton);
    }

    public void ShowUpgradeWeaponsTab()
    {
        SetActiveTab(upgradeWeaponsPanel, upgradeWeaponsTabButton);
    }

    private void SetActiveTab(GameObject selectedPanel, Button selectedButton)
    {
        purchaseTrapsPanel.SetActive(false);
        upgradeTrapsPanel.SetActive(false);
        upgradeWeaponsPanel.SetActive(false);

        selectedPanel.SetActive(true);

        purchaseTrapsTabButton.image.color = normalTabColor;
        upgradeTrapsTabButton.image.color = normalTabColor;
        upgradeWeaponsTabButton.image.color = normalTabColor;

        selectedButton.image.color = selectedTabColor;
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

    // Purchases a beehive trap if the player can afford it.
    public void BuyBeehiveTrap()
    {
        if (playerResources == null)
            return;

        if (playerWeapons == null)
            return;

        bool purchased = playerResources.SpendDrops(beehiveTrapCost);

        if (!purchased)
        {
            return;
        }

        playerWeapons.AddBeehiveTrap();

        UpdateButton();
    }

    // Purchases a magnifying glass trap if the player can afford it.
    public void BuyMagnifyingGlassTrap()
    {
        if (playerResources == null)
            return;

        if (playerWeapons == null)
            return;

        bool purchase = playerResources.SpendDrops(magnifyingGlassTrapCost);

        if (!purchase)
        {
            return;
        }

        playerWeapons.AddMagnifyingGlassTrap();

        UpdateButton();
    }

    // Purchases a magnifying glass trap if the player can afford it.
    public void BuySprinklerTrap()
    {
        if (playerResources == null)
            return;

        if (playerWeapons == null)
            return;

        bool purchase = playerResources.SpendDrops(sprinklerTrapCost);

        if (!purchase)
        {
            return;
        }

        playerWeapons.AddSprinklerTrap();

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

        if (beehiveTrapButton == null || playerResources == null)
            return;

        beehiveTrapButton.interactable = playerResources.TotalDrops >= beehiveTrapCost;

        if (magnifyingGlassTrapButton == null || playerResources == null)
            return;

        magnifyingGlassTrapButton.interactable = playerResources.TotalDrops >= magnifyingGlassTrapCost;

        if (sprinklerTrapButton == null || playerResources == null)
            return;

        sprinklerTrapButton.interactable = playerResources.TotalDrops >= sprinklerTrapCost;
    }
}