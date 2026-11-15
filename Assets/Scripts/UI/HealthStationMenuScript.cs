using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthStationMenuScript : MonoBehaviour
{
    public enum FikaEffect
    {
        None,
        StaminaDepletion,
        StaminaRecovery,
        HealthRegeneration
    }

    [Serializable]
    public class FikaItem
    {
        public string itemName;
        [TextArea] public string description;
        public Sprite icon;

        [Min(0)] public int cost;
        [Min(0)] public int healing;

        public FikaEffect effect;

        [Min(0.1f)] public float effectMultiplier = 1f;
        [Min(0f)] public float effectDuration = 20f;
    }

    [Header("Menu")]
    [SerializeField] private GameObject healthStationMenu;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private HealthStationItemCardScript cardPrefab;

    [Header("Items")]
    [SerializeField]
    private List<FikaItem> items = new List<FikaItem>
    {
        new FikaItem
        {
            itemName = "Kanelbulle",
            description = "A classic cinnamon bun. Restores 20 health.",
            cost = 5,
            healing = 20
        },
        new FikaItem
        {
            itemName = "Chokladboll",
            description = "A chocolate treat. Restores 35 health.",
            cost = 8,
            healing = 35
        },
        new FikaItem
        {
            itemName = "Semla",
            description = "A generous cream-filled bun. Restores 60 health.",
            cost = 14,
            healing = 60
        },
        new FikaItem
        {
            itemName = "Prinsesstårta",
            description = "A slice of princess cake. Restores 100 health.",
            cost = 22,
            healing = 100
        },
        new FikaItem {itemName = "Bryggkaffe", description = "Reduces stamina depletion by 50% for 20 seconds.", cost = 12, effect = FikaEffect.StaminaDepletion, effectMultiplier = 0.5f, effectDuration = 20f},
        new FikaItem {itemName = "Varm choklad", description = "Doubles stamina recovery for 20 seconds.", cost = 10, effect = FikaEffect.StaminaRecovery, effectMultiplier = 2f, effectDuration = 20f},
        new FikaItem
{
    itemName = "Kardemummabulle",
    description = "Slowly restores 2 HP per second for 10 seconds.",
    cost = 15,
    effect = FikaEffect.HealthRegeneration,
    effectMultiplier = 2f,
    effectDuration = 10f}};

    [Header("Player")]
    [SerializeField] private PlayerResourceScript playerResources;
    [SerializeField] private PlayerLookScript playerLook;
    [SerializeField] private PlayerWeaponsScript playerWeapons;
    [SerializeField] private PlayerMovementScript playerMovement;

    [Header("HUD")]
    [SerializeField] private GameObject hotbarUI;
    [SerializeField] private GameObject ammoTextUI;

    [Header("Purchase UI")]
    [SerializeField] private TextMeshProUGUI totalCostText;
    [SerializeField] private TextMeshProUGUI totalHealingText;
    [SerializeField] private TextMeshProUGUI availableDropsText;
    [SerializeField] private Button purchaseButton;

    private readonly List<int> selectedQuantities = new List<int>();

    private bool menuOpen;

    public bool IsMenuOpen => menuOpen;

    private void Start()
    {
        if (healthStationMenu != null)
            healthStationMenu.SetActive(false);

        BuildItemCards();
        UpdateTotals();
    }

    private void BuildItemCards()
    {
        if (cardContainer == null || cardPrefab == null)
        {
            Debug.LogError("Health Station: Card Container or Card Prefab is missing.");
            return;
        }

        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        selectedQuantities.Clear();

        for (int i = 0; i < items.Count; i++)
        {
            selectedQuantities.Add(0);

            FikaItem item = items[i];

            HealthStationItemCardScript card =
                Instantiate(cardPrefab, cardContainer);

            card.Setup(
                i,
                item.icon,
                item.itemName,
                item.description,
                OnQuantityChanged
            );
        }
    }

    private void OnQuantityChanged(int itemIndex, int quantity)
    {
        if (itemIndex < 0 || itemIndex >= selectedQuantities.Count)
            return;

        selectedQuantities[itemIndex] = quantity;
        UpdateTotals();
    }

    private int CalculateTotalCost()
    {
        int total = 0;

        for (int i = 0; i < items.Count; i++)
            total += items[i].cost * selectedQuantities[i];

        return total;
    }

    private int CalculateTotalHealing()
    {
        int total = 0;

        for (int i = 0; i < items.Count; i++)
            total += items[i].healing * selectedQuantities[i];

        return total;
    }

    private int CalculateEffectiveHealing()
    {
        int missingHealth = Mathf.Max(
            0,
            GameManagerScript.MaxHP - GameManagerScript.GetHP()
        );

        return Mathf.Min(CalculateTotalHealing(), missingHealth);
    }

    private void UpdateTotals()
    {
        int totalCost = CalculateTotalCost();
        int effectiveHealing = CalculateEffectiveHealing();

        if (totalCostText != null)
            totalCostText.text = "Total Cost: " + totalCost + " Drops";

        if (totalHealingText != null)
            totalHealingText.text = "Health Regained: " + effectiveHealing + " HP";

        if (availableDropsText != null && playerResources != null)
            availableDropsText.text = "Available: " + playerResources.TotalDrops + " Drops";

        if (purchaseButton != null)
        {
            bool hasSelection = totalCost > 0;
            bool canAfford = playerResources != null &&
                             playerResources.TotalDrops >= totalCost;

            purchaseButton.interactable = hasSelection && canAfford;
        }
    }

    public void PurchaseSelection()
    {
        if (playerResources == null)
            return;

        int totalCost = CalculateTotalCost();

        if (totalCost <= 0 || !playerResources.SpendDrops(totalCost))
            return;

        int totalHealing = CalculateTotalHealing();

        if (totalHealing > 0)
            GameManagerScript.RestoreHP(totalHealing);

        for (int i = 0; i < items.Count; i++)
        {
            int quantity = selectedQuantities[i];

            if (quantity <= 0)
                continue;

            ApplyItemEffect(items[i], quantity);
        }

        BuildItemCards();
        UpdateTotals();
    }

    private void ApplyItemEffect(FikaItem item, int quantity)
    {
        float totalDuration = item.effectDuration * quantity;

        switch (item.effect)
        {
            case FikaEffect.StaminaDepletion:

                if (playerMovement != null)
                {
                    playerMovement.ApplyTemporaryStaminaDepletionMultiplier(item.effectMultiplier, totalDuration);
                }

                break;

            case FikaEffect.StaminaRecovery:

                if (playerMovement != null)
                {
                    playerMovement.ApplyTemporaryStaminaRecoveryMultiplier(item.effectMultiplier, totalDuration);
                }

                break;

            case FikaEffect.HealthRegeneration:

                if (GameManagerScript.instance != null)
                {
                    GameManagerScript.instance.ApplyTemporaryHealthRegeneration(Mathf.RoundToInt(item.effectMultiplier), totalDuration);
                }

                break;
        }
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

        UpdateTotals();
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
}