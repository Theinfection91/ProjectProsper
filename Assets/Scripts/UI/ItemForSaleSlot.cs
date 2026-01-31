using Assets.Scripts.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemForSaleSlot : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public TMP_Text weightText;
    public TMP_Text quantityText;

    private ItemData itemData;

    public void SetupItemForSale(ItemData item)
    {
        itemData = item;
        itemNameText.text = item.itemSO.itemName;
        priceText.text = $"Price: {item.itemSO.baseValue}";
        weightText.text = $"Weight: {item.itemSO.weight}";
        quantityText.text = $"Qty: {item.quantity}";
    }

    // Optionally, add a method for handling buy button clicks
    public void OnBuyButtonClicked()
    {
        // Call ShopManager.Instance.BuyFromWholesaler(...) or similar
        
    }
}