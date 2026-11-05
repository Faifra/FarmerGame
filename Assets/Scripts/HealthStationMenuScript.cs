using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthStationMenuScript : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private GameObject healthStationMenu;

    [Header("Player")]
    [SerializeField] private PlayerResourceScript playerResources;
    [SerializeField] private PlayerLookScript playerLook;
    [SerializeField] private PlayerWeaponsScript playerWeapons;

    [Header("HUD")]
    [SerializeField] private GameObject hotbarUI;
    [SerializeField] private GameObject ammoTextUI;

    [Header("Small Fika")]
    [SerializeField] private int smallFikaCost = 5;
    [SerializeField] private int smallFikaHealing = 20;
    [SerializeField] private Button smallFikaButton;
    [SerializeField] private TextMeshProUGUI smallFikaText;

    [Header("Medium Fika")]
    [SerializeField] private int mediumFikaCost = 10;
    [SerializeField] private int mediumFikaHealing = 50;
    [SerializeField] private Button mediumFikaButton;
    [SerializeField] private TextMeshProUGUI mediumFikaText;

    [Header("Large Fika")]
    [SerializeField] private int largeFikaCost = 20;
    [SerializeField] private int largeFikaHealing = 100;
    [SerializeField] private Button largeFikaButton;
    [SerializeField] private TextMeshProUGUI largeFikaText;

    private bool menuOpen;

    public bool IsMenuOpen => menuOpen;

    private void Start()
    {
        healthStationMenu.SetActive(false);

        UpdateButtonStates();
        UpdateOptionTexts();
    }

    private void Update()
    {
        if (menuOpen)
            UpdateButtonStates();
    }

    public void OpenMenu()
    {
        if (menuOpen)
            return;

        menuOpen = true;
        healthStationMenu.SetActive(true);

        if (hotbarUI != null)
            hotbarUI.SetActive(false);

        if (ammoTextUI != null)
            ammoTextUI.SetActive(false);

        if (playerLook != null)
            playerLook.SetMenuOpen(true);

        if (playerWeapons != null)
            playerWeapons.SetMenuOpen(true);

        UpdateButtonStates();
    }

    public void CloseMenu()
    {
        if (!menuOpen)
            return;

        menuOpen = false;
        healthStationMenu.SetActive(false);

        if (hotbarUI != null)
            hotbarUI.SetActive(true);

        if (ammoTextUI != null)
            ammoTextUI.SetActive(true);

        if (playerLook != null)
            playerLook.SetMenuOpen(false);

        if (playerWeapons != null)
            playerWeapons.SetMenuOpen(false);
    }

    public void BuySmallFika()
    {
        PurchaseHealing(smallFikaCost, smallFikaHealing);
    }

    public void BuyMediumFika()
    {
        PurchaseHealing(mediumFikaCost, mediumFikaHealing);
    }

    public void BuyLargeFika()
    {
        PurchaseHealing(largeFikaCost, largeFikaHealing);
    }

    private void PurchaseHealing(int cost, int healing)
    {
        if (playerResources == null)
            return;

        if (GameManagerScript.GetHP() >= GameManagerScript.MaxHP)
            return;

        if (!playerResources.SpendDrops(cost))
            return;

        GameManagerScript.RestoreHP(healing);

        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        if (playerResources == null)
            return;

        int drops = playerResources.TotalDrops;
        bool needsHealing = GameManagerScript.GetHP() < GameManagerScript.MaxHP;

        if (smallFikaButton != null)
            smallFikaButton.interactable = needsHealing && drops >= smallFikaCost;

        if (mediumFikaButton != null)
            mediumFikaButton.interactable = needsHealing && drops >= mediumFikaCost;

        if (largeFikaButton != null)
            largeFikaButton.interactable = needsHealing && drops >= largeFikaCost;
    }

    private void UpdateOptionTexts()
    {
        if (smallFikaText != null)
            smallFikaText.text = smallFikaHealing + " HP  -  " + smallFikaCost + " Drops";

        if (mediumFikaText != null)
            mediumFikaText.text = mediumFikaHealing + " HP  -  " + mediumFikaCost + " Drops";

        if (largeFikaText != null)
            largeFikaText.text = largeFikaHealing + " HP  -  " + largeFikaCost + " Drops";
    }
}