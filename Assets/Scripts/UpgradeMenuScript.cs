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

    [Header("Launch Trap Upgrade")]
    [SerializeField] private int launchTrapCost = 10;
    [SerializeField] private Button launchTrapButton;
    [SerializeField] private TextMeshProUGUI launchTrapCostText;

    [Header("Pull Trap Upgrade")]
    [SerializeField] private int pullTrapCost = 15;
    [SerializeField] private Button pullTrapButton;
    [SerializeField] private TextMeshProUGUI pullTrapCostText;

    private bool menuOpen = false;

    public bool IsMenuOpen => menuOpen;

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

    private void Update()
    {
        if (menuOpen)
        {
            UpdateButton();
        }
    }

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