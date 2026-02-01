using Assets.Scripts.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WholesaleItemForSaleSlot : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public TMP_Text weightText;
    public TMP_Text quantityText;
    public Button buyButton;
    public ItemData itemData;
    public Wholesaler wholesaler;

    public void SetupItemForSale(ItemData item, Wholesaler wholesaler)
    {
        itemData = item;
        this.wholesaler = wholesaler;
        itemNameText.text = item.itemSO.itemName;
        priceText.text = $"Price: {item.itemSO.baseValue}";
        weightText.text = $"Weight: {item.itemSO.weight}";
        quantityText.text = $"Qty: {item.quantity}";
    }

    public void OnBuyButtonClicked()
    {
        if (wholesaler == null || itemData == null) return;

        // Only buy one at a time
        int buyAmount = 1;

        // Check if there is at least one item left
        if (itemData.quantity < buyAmount)
        {
            Debug.Log("No more items left to buy.");
            return;
        }

        // Check if player can buy one
        ItemData tempItem = new ItemData
        {
            itemSO = itemData.itemSO,
            quantity = buyAmount
        };

        if (!ShopManager.Instance.CanBuyFromWholesaler(wholesaler, tempItem))
        {
            Debug.Log("Cannot buy item from wholesaler. Either you don't have enough money or the item is too heavy.");
            return;
        }

        // Buy one item (ShopManager will decrement the quantity)
        ShopManager.Instance.BuyFromWholesaler(wholesaler, tempItem);

        // Update UI based on the new quantity
        if (itemData.quantity <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            quantityText.text = $"Qty: {itemData.quantity}";
        }
    }
}