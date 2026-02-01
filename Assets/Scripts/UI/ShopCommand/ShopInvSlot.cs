using Assets.Scripts.Item;
using TMPro;
using UnityEngine;

public class ShopInvSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemQuantityText;

    public ItemData itemData;

    public void Setup(ItemData item)
    {
        itemData = item;

        if (itemNameText != null)
            itemNameText.text = item.itemSO.itemName; // Assumes Item has a displayName property

        if (itemQuantityText != null)
            itemQuantityText.text = $"Qty: {item.quantity}";
    }
}
