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
        if (!ShopManager.Instance.CanBuyFromWholesaler(wholesaler, itemData))
        {
            Debug.Log("Cannot buy item from wholesaler. Either you don't have enough money or the item is too heavy.");
            return;
        }
        ShopManager.Instance.BuyFromWholesaler(wholesaler, itemData);
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