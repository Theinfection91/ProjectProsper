using Assets.Scripts.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInvSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemQuantityText;
    [SerializeField] private Button addOneButton;
    [SerializeField] private Button addAllButton;
    [SerializeField] private Button addXButton;
    [SerializeField] private TMP_InputField addXInput;

    public ItemData itemData;
    public Shop targetShop;

    public void Setup(ItemData item, Shop shop)
    {
        itemData = item;
        targetShop = shop;

        if (itemNameText != null)
            itemNameText.text = item.itemSO.itemName; // Assumes Item has a displayName property

        if (itemQuantityText != null)
            itemQuantityText.text = $"Qty: {item.quantity}";
    }

    public void OnAddOneButtonClicked()
    {
        if (targetShop == null || itemData == null) return;
        int buyAmount = 1;
        ItemData tempItem = new ItemData
        {
            itemSO = itemData.itemSO,
            quantity = buyAmount
        };
        if (!targetShop.HasRoomForItemWeight(tempItem))
        {
            Debug.LogWarning("Not enough room in shop for item.");
            return;
        }
        targetShop.AddItem(tempItem);
        PlayerInventory.Instance.RemoveItem(itemData.itemSO, buyAmount);
        if (itemData.quantity <= 0)
        {
            Destroy(gameObject);
            return;
        }
        Setup(itemData, targetShop);
    }

    public void OnAddAllButtonClicked()
    {
        if (targetShop == null || itemData == null) return;
        int buyAmount = itemData.quantity;
        ItemData tempItem = new ItemData
        {
            itemSO = itemData.itemSO,
            quantity = buyAmount
        };
        if (!targetShop.HasRoomForItemWeight(tempItem))
        {
            Debug.LogWarning("Not enough room in shop for all items.");
            return;
        }
        targetShop.AddItem(tempItem);
        PlayerInventory.Instance.RemoveItem(itemData.itemSO, buyAmount);
        Destroy(gameObject);
    }

    public void OnAddXButtonClicked()
    {
        if (targetShop == null || itemData == null) return;
        if (int.TryParse(addXInput.text, out int buyAmount))
        {
            if (buyAmount <= 0 || buyAmount > itemData.quantity)
            {
                Debug.LogWarning("Invalid quantity entered.");
                return;
            }
            ItemData tempItem = new ItemData
            {
                itemSO = itemData.itemSO,
                quantity = buyAmount
            };
            if (!targetShop.HasRoomForItemWeight(tempItem))
            {
                Debug.LogWarning("Not enough room in shop for items.");
                return;
            }
            targetShop.AddItem(tempItem);
            PlayerInventory.Instance.RemoveItem(itemData.itemSO, buyAmount);
            if (itemData.quantity <= 0)
            {
                Destroy(gameObject);
                return;
            }
            Setup(itemData, targetShop);
        }
        else
        {
            Debug.LogWarning("Please enter a valid number.");
        }
    }
}
