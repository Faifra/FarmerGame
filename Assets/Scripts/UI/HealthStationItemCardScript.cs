using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthStationItemCardScript : MonoBehaviour
{
    [Header("Card UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;

    private int itemIndex;
    private int quantity;
    private Action<int, int> quantityChanged;

    public void Setup(int index, Sprite icon, string itemName, string description, Action<int, int> onQuantityChanged)
    {
        itemIndex = index;
        quantityChanged = onQuantityChanged;

        if (itemIcon != null)
        {
            itemIcon.sprite = icon;
            itemIcon.enabled = icon != null;
        }

        if (itemNameText != null)
            itemNameText.text = itemName;

        if (descriptionText != null)
            descriptionText.text = description;

        if (increaseButton != null)
        {
            increaseButton.onClick.RemoveAllListeners();
            increaseButton.onClick.AddListener(IncreaseQuantity);
        }

        if (decreaseButton != null)
        {
            decreaseButton.onClick.RemoveAllListeners();
            decreaseButton.onClick.AddListener(DecreaseQuantity);
        }

        SetQuantity(0);
    }

    private void IncreaseQuantity()
    {
        quantity++;
        SetQuantity(quantity);
        quantityChanged?.Invoke(itemIndex, quantity);
    }

    private void DecreaseQuantity()
    {
        if (quantity <= 0)
            return;

        quantity--;
        SetQuantity(quantity);
        quantityChanged?.Invoke(itemIndex, quantity);
    }

    private void SetQuantity(int newQuantity)
    {
        quantity = Mathf.Max(0, newQuantity);

        if (quantityText != null)
            quantityText.text = quantity.ToString();

        if (decreaseButton != null)
            decreaseButton.interactable = quantity > 0;
    }
}