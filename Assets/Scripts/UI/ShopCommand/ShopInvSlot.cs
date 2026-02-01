using Assets.Scripts.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopInvSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemQuantityText;
    [SerializeField] private Button listOneButton;
    [SerializeField] private Button listAllButton;
    [SerializeField] private Button listXButton;
    [SerializeField] private TMP_InputField listXInput;

    public ItemData itemData;
    public Shop thisShop;

    public void Setup(ItemData item, Shop shop)
    {
        itemData = item;
        thisShop = shop;

        if (itemNameText != null)
            itemNameText.text = item.itemSO.itemName; // Assumes Item has a displayName property

        if (itemQuantityText != null)
            itemQuantityText.text = $"Qty: {item.quantity}";
    }

    public void OnListOneButtonClicked()
    {
        if (thisShop == null || itemData == null) return;
        int listAmount = 1;
        ItemData tempItem = new ItemData
        {
            itemSO = itemData.itemSO,
            quantity = listAmount
        };

        thisShop.ListItemForSale(tempItem);

        // Refresh both panels to ensure UI is up to date
        UIManager.Instance.PopulateShopInventorySlots();
        UIManager.Instance.PopulateForSaleInventorySlots();
    }
}
