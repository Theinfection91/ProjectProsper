using Assets.Scripts.Shop;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Customer
{
    public int wallet;
    // D&D-like persuasion modifier used when attempting purchases (can be tuned)
    public int persuasionModifier;

    public Customer()
    {
        // Generate a random wallet amount (adjust ranges as needed)
        wallet = UnityEngine.Random.Range(50, 500);

        // Small persuasion modifier to simulate customer personality / negotiation
        // Range: -2 .. +5 (tweak to taste)
        persuasionModifier = UnityEngine.Random.Range(-2, 6);
    }

    /// <summary>
    /// Initial purchase resolved by customer's D20 check vs a DC derived from purchaseChance.
    /// After an initial successful purchase, the shop can attempt up to two upsells (shop vs customer contested d20 rolls).
    /// Maximum purchases per visit = 3.
    /// </summary>
    public void AttemptPurchase(Shop shop)
    {
        if (shop.itemsForSale == null || shop.itemsForSale.Count == 0)
        {
            Debug.LogWarning($"[Customer] No items for sale at '{shop.name}'");
            return;
        }

        const int maxPurchases = 3;
        int purchases = 0;

        // Build affordable candidate list
        var affordableInitial = shop.itemsForSale.Where(item => item.salePrice <= wallet && item.itemData.quantity > 0).ToList();
        if (affordableInitial.Count == 0)
        {
            Debug.Log($"[Customer] Cannot afford any items at '{shop.name}' (wallet: {wallet})");
            return;
        }

        // Choose initial item (weighted by purchase chance)
        var initial = SelectWeightedRandom(affordableInitial, shop);

        // Resolve initial purchase with D&D-style check (customer tries to decide to buy)
        float initialChance = shop.CalculatePurchaseChance(initial);
        int initialDC = Mathf.RoundToInt(Mathf.Lerp(20f, 6f, Mathf.Clamp01(initialChance)));
        int customerInitialRoll = UnityEngine.Random.Range(1, 21) + persuasionModifier;

        if (customerInitialRoll >= initialDC)
        {
            CompletePurchase(shop, initial);
            purchases++;
        }
        else
        {
            Debug.Log($"[Customer] Rejected initial purchase '{initial.itemData.itemSO.itemName}' at '{shop.name}' (roll: {customerInitialRoll} vs DC: {initialDC})");
            return;
        }

        // If initial purchase succeeded, allow the shop to try upsells (contested rolls).
        // Shop rolls d20 + shopUpsellModifier; customer rolls d20 + persuasionModifier.
        // If shop wins, it forces another purchase (if affordable/available). Repeat until maxPurchases.
        while (purchases < maxPurchases)
        {
            // Check for affordable remaining items
            var affordable = shop.itemsForSale.Where(item => item.salePrice <= wallet && item.itemData.quantity > 0).ToList();
            if (affordable.Count == 0) break;

            int shopRoll = UnityEngine.Random.Range(1, 21) + shop.GetUpsellModifier();
            int customerRoll = UnityEngine.Random.Range(1, 21) + persuasionModifier;

            if (shopRoll > customerRoll)
            {
                // Shop wins the contested roll — pick a next item and force a purchase if affordable
                var selected = SelectWeightedRandom(affordable, shop);

                // Final safety check: ensure wallet can still cover selected item
                if (selected.salePrice > wallet)
                {
                    // Shouldn't happen because we filtered affordable, but guard anyway
                    break;
                }

                CompletePurchase(shop, selected);
                purchases++;
            }
            else
            {
                // Customer resists the upsell
                Debug.Log($"[Customer] Resisted upsell at '{shop.name}' (shop: {shopRoll} vs customer: {customerRoll})");
                break;
            }
        }

        if (purchases > 0)
        {
            Debug.Log($"[Customer] Total purchases this visit: {purchases} at '{shop.name}'. Wallet left: {wallet} (Persuasion mod: {persuasionModifier})");
        }
    }

    /// <summary>
    /// Select a candidate item from the list using purchase-chance as weight.
    /// Falls back to uniform random if all weights are zero.
    /// </summary>
    private ShopItemForSale SelectWeightedRandom(List<ShopItemForSale> candidates, Shop shop)
    {
        // Build weights (ensure tiny non-zero minimum so selection is stable)
        float[] weights = candidates.Select(c => Mathf.Max(0.001f, shop.CalculatePurchaseChance(c))).ToArray();
        float total = weights.Sum();

        if (total <= 0f)
            return candidates[UnityEngine.Random.Range(0, candidates.Count)];

        float r = UnityEngine.Random.value * total;
        for (int i = 0; i < candidates.Count; i++)
        {
            if (r <= weights[i])
                return candidates[i];
            r -= weights[i];
        }

        // Fallback
        return candidates[candidates.Count - 1];
    }

    private void CompletePurchase(Shop shop, ShopItemForSale item)
    {
        if (item == null || item.itemData == null || item.itemData.quantity <= 0)
        {
            Debug.LogWarning("[Customer] Attempted to complete purchase for invalid item.");
            return;
        }

        // Deduct from customer wallet
        wallet -= item.salePrice;

        // Deduct from shop inventory (one unit)
        item.itemData.quantity--;

        // Deduct weight from shop capacity (one unit)
        shop.currentInventoryCapacity -= item.itemData.itemSO.weight;
        if (shop.currentInventoryCapacity < 0f) shop.currentInventoryCapacity = 0f;

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
