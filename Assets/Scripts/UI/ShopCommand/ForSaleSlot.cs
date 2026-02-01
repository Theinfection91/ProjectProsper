using Assets.Scripts.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForSaleSlot : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text baseValueText;
    [SerializeField] private TMP_Text demandPercentText; // TODO
    [SerializeField] private TMP_Text itemQuantityText;
    [SerializeField] private Button cancelOneButton;
    [SerializeField] private Button cancelXButton;
    [SerializeField] private TMP_InputField cancelXInput;
    [SerializeField] private Button cancelAllButton;

    [Header("Item Pricing Types")]
    [SerializeField] private TMP_Dropdown itemPricingTypeDropdown;

    [Header("Set Price Panel")]
    [SerializeField] private Transform setPricePanel;
    [SerializeField] private TMP_InputField setPriceInput;
    [SerializeField] private TMP_Text priceFairnessText;

    [Header("Markup Percentage Panel")]
    [SerializeField] private Transform markupPercentagePanel;

    [Header("Haggle Price Panel")]
    [SerializeField] private Transform hagglePricePanel;

    public ItemData itemData;
    public Shop shop;

    public void Setup(ItemData item, Shop shop)
    {
        itemData = item;
        this.shop = shop;
        if (itemNameText != null)
            itemNameText.text = item.itemSO.itemName; // Assumes Item has a displayName property
        if (baseValueText != null)
            baseValueText.text = $"Base Value: {item.itemSO.baseValue}";
        if (itemQuantityText != null)
            itemQuantityText.text = $"Qty: {item.quantity}";
    }
}
