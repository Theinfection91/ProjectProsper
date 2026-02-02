using Assets.Scripts.Shop;
using System;
using UnityEngine;

[Serializable]
public class Customer
{
    public int wallet;

    public Customer()
    {
        // Generate a random wallet amount (adjust ranges as needed)
        wallet = UnityEngine.Random.Range(50, 500);
    }

    /// <summary>
    /// Attempts to purchase a random item from the shop.
    /// </summary>
    public void AttemptPurchase(Shop shop)
    {
        if (shop.itemsForSale == null || shop.itemsForSale.Count == 0)
        {
            Debug.LogWarning($"[Customer] No items for sale at '{shop.name}'");
            return;
        }

        // Filter items customer can afford
        var affordableItems = shop.itemsForSale.FindAll(item =>
            item.salePrice <= wallet && item.itemData.quantity > 0);

        if (affordableItems.Count == 0)
        {
            Debug.Log($"[Customer] Cannot afford any items at '{shop.name}' (wallet: {wallet})");
            return;
        }

        // Pick a random affordable item
        ShopItemForSale selectedItem = affordableItems[UnityEngine.Random.Range(0, affordableItems.Count)];

        // Calculate purchase chance based on demand, pricing, etc.
        float purchaseChance = shop.CalculatePurchaseChance(selectedItem);

        // Roll for purchase
        float roll = UnityEngine.Random.value;

        if (roll <= purchaseChance)
        {
            // Successful purchase!
            CompletePurchase(shop, selectedItem);
        }
        else
        {
            Debug.Log($"[Customer] Decided not to buy '{selectedItem.itemData.itemSO.itemName}' at '{shop.name}' (roll: {roll:F2} vs chance: {purchaseChance:F2})");
        }
    }

    private void CompletePurchase(Shop shop, ShopItemForSale item)
    {
        // Deduct from customer wallet
        wallet -= item.salePrice;

        // Deduct from shop inventory
        item.itemData.quantity--;

        // Deduct weight from shop capacity
        shop.currentInventoryCapacity -= item.itemData.itemSO.weight;

        // Add earnings to shop
        shop.earningsToday += item.salePrice;
        shop.totalEarnings += item.salePrice;

        // Remove item from sale if quantity reaches 0
        if (item.itemData.quantity <= 0)
        {
            shop.itemsForSale.Remove(item);
        }

        Debug.Log($"[Customer] ✓ PURCHASED '{item.itemData.itemSO.itemName}' for {item.salePrice}g at '{shop.name}' | Shop Earnings: {shop.earningsToday}g today");
    }
}
