using Assets.Scripts.Item;
using Assets.Scripts.Shop;
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

    public ShopItemForSale itemData;
    public Shop shop;

    private void Awake()
    {
        if (setPriceInput != null)
        {
            setPriceInput.onEndEdit.AddListener(OnSetPriceInputEditEnd);
        }
    }

    private void OnDestroy()
    {
        if (setPriceInput != null)
        {
            setPriceInput.onEndEdit.RemoveListener(OnSetPriceInputEditEnd);
        }

    }

    public void Setup(ShopItemForSale item, Shop shop)
    {
        itemData = item;
        this.shop = shop;
        if (itemNameText != null)
            itemNameText.text = item.itemData.itemSO.itemName; // Assumes Item has a displayName property
        if (baseValueText != null)
            baseValueText.text = $"Base Value: {item.itemData.itemSO.baseValue}";
        if (itemQuantityText != null)
            itemQuantityText.text = $"Qty: {item.itemData.quantity}";
        demandPercentText.text = $"Demand%: {DemandManager.Instance.GetDemandLevel(item.itemData.itemSO)}";

        // Set Price Panel is active by default
        setPriceInput.text = item.salePrice.ToString();
        UpdatePriceFairness(item.salePrice);
    }

    public void OnSetPriceInputEditEnd(string newValue)
    {
        if (int.TryParse(newValue, out int newPrice))
        {
            itemData.salePrice = newPrice;
            // Update price fairness text
            int baseValue = itemData.itemData.itemSO.baseValue;
            UpdatePriceFairness(newPrice);
        }
    }

    private void UpdatePriceFairness(int currentPrice)
    {
        if (priceFairnessText == null) return;

        int baseValue = itemData.itemData.itemSO.baseValue;
        float priceRatio = (float)currentPrice / baseValue;

        string fairnessText;
        Color fairnessColor;

        if (priceRatio < 0.8f)
        {
            fairnessText = "Losing Money!";
            fairnessColor = Color.red;
        }
        else if (priceRatio < 1.0f)
        {
            fairnessText = "Below Cost";
            fairnessColor = new Color(1f, 0.5f, 0f); // Orange
        }
        else if (priceRatio <= 1.5f)
        {
            fairnessText = "Fair Price";
            fairnessColor = Color.green;
        }
        else if (priceRatio <= 2.0f)
        {
            fairnessText = "High Markup";
            fairnessColor = Color.yellow;
        }
        else
        {
            fairnessText = "Overpriced";
            fairnessColor = Color.red;
        }

        int markupPercent = Mathf.RoundToInt((priceRatio - 1f) * 100f);
        priceFairnessText.text = $"{fairnessText} ({markupPercent:+0;-0;0}%)";
        priceFairnessText.color = fairnessColor;
    }
}
