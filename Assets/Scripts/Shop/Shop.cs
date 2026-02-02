using Assets.Scripts.Item;
using Assets.Scripts.Shop;
using Assets.Scripts.Worker;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Shop
{
    // Basic Info
    public string id;
    public string name;
    public ShopTypeSO shopType = null;
    public ShopSize size;
    public ShopOwnership ownership = ShopOwnership.Unclaimed;
    public string ownerName;
    public int dailyRent;
    public int daysRented;
    public float footTrafficScore = 1.0f;

    // Inventory
    public List<ItemData> stockroomItems = new();
    public List<ShopItemForSale> itemsForSale = new();

    // Schedule
    [SerializeField] public ShopSchedule shopSchedule = new();

    // Workers
    public List<Worker> employedWorkers = new();
    public bool hasPlayerWorking = false;

    // Earnings Tracking
    public int earningsToday = 0;
    public int yesterdayEarnings = 0;
    public int totalEarnings = 0;
    public Dictionary<int, int> previousEarningHistory = new(); // Key: Day number, Value: Earnings

    // Resell Stall Specific
    public float maxInventoryCapacity;
    public float currentInventoryCapacity;


    // States
    public bool isOpenForBusiness = false;
    public bool isWithinOperatingHours = false;

    public Shop(string id, string name)
    {
        this.id = id;
        this.name = name;
    }

    public void ClaimForPlayer(string playerName)
    {
        ownership = ShopOwnership.Player;
        ownerName = playerName;
    }

    public void SetShopType(ShopTypeSO type)
    {
        shopType = type;
    }

    public void HireWorker(Worker worker)
    {
        employedWorkers.Add(worker);
    }

    public void FireWorker(Worker worker)
    {
        employedWorkers.Remove(worker);
    }

    /// <summary>
    /// Returns an integer upsell modifier representing the shop's ability to upsell customers.
    /// Aggregates employed workers' sales skill. If no workers, returns a small base modifier.
    /// Mapping: None=0, Apprentice=1, Journeyman=2, Expert=3, Master=4
    /// </summary>
    public int GetUpsellModifier()
    {
        if (employedWorkers == null || employedWorkers.Count == 0)
        {
            // base shop pitch when no staff are present
            return 1;
        }

        int total = 0;
        foreach (var w in employedWorkers)
        {
            if (w == null) continue;
            total += SkillLevelToModifier(w.skillLevel);
        }

        float avg = (float)total / employedWorkers.Count;
        // Add a small base so shops with low-level staff still have some chance
        return Mathf.RoundToInt(avg) + 1;
    }

    private int SkillLevelToModifier(SkillLevel level)
    {
        return level switch
        {
            SkillLevel.None => 0,
            SkillLevel.Apprentice => 1,
            SkillLevel.Journeyman => 2,
            SkillLevel.Expert => 3,
            SkillLevel.Master => 4,
            _ => 0
        };
    }

    public bool HasRoomForItemWeight(ItemData itemData)
    {
        // Check if the shop has room for the item based on its weight and current load
        float incomingItemWeight = itemData.itemSO.weight * itemData.quantity;
        if (currentInventoryCapacity + incomingItemWeight <= maxInventoryCapacity)
        {
            return true;
        }
        return false;
    }

    public void AddItem(ItemData itemData)
    {
        // Add items to the shop's stockroom.
        // Ensure we don't accidentally keep a reference to a caller-owned ItemData instance.
        // Merge into an existing stockroom entry if present.
        ItemData existingItem = stockroomItems.Find(i => i.itemSO == itemData.itemSO);
        if (existingItem != null)
        {
            existingItem.quantity += itemData.quantity;
        }
        else
        {
            // Add a fresh ItemData record to avoid shared references
            stockroomItems.Add(new ItemData { itemSO = itemData.itemSO, quantity = itemData.quantity });
        }

        // Update weight
        currentInventoryCapacity += itemData.itemSO.weight * itemData.quantity;

        // Safety clamp (avoid tiny negative or overflows)
        if (currentInventoryCapacity < 0f) currentInventoryCapacity = 0f;

        Debug.Log($"[Shop] Added {itemData.quantity} x {itemData.itemSO.itemName} to '{name}'. CurrentWeight: {currentInventoryCapacity:F2}/{maxInventoryCapacity:F2}");
    }

    public void RemoveItem(ItemData itemData)
    {
        ItemData existingItem = stockroomItems.Find(i => i.itemSO == itemData.itemSO);
        if (existingItem != null)
        {
            if (existingItem.quantity >= itemData.quantity)
            {
                existingItem.quantity -= itemData.quantity;
                // Update weight by actual removed amount
                currentInventoryCapacity -= itemData.itemSO.weight * itemData.quantity;

                if (existingItem.quantity == 0)
                {
                    stockroomItems.Remove(existingItem);
                }

                // Safety clamp
                if (currentInventoryCapacity < 0f) currentInventoryCapacity = 0f;
            }
            else
            {
                Debug.LogWarning("Attempted to remove more items than available in stockroom.");
            }
        }
        else
        {
            Debug.LogWarning("Attempted to remove an item that does not exist in stockroom.");
        }
    }

    public void ListItemForSale(ItemData itemData)
    {
        // Move item from stockroom to items for sale.
        // IMPORTANT: items for sale are still physically in the shop, so total shop weight should not change.
        if (itemData == null || itemData.itemSO == null || itemData.quantity <= 0)
        {
            Debug.LogWarning("[Shop] Invalid item data provided to ListItemForSale.");
            return;
        }

        // Find matching stockroom entry
        ItemData stockItem = stockroomItems.Find(i => i.itemSO == itemData.itemSO);
        if (stockItem == null)
        {
            Debug.LogWarning($"[Shop] No stockroom entry found for {itemData.itemSO.itemName} when attempting to list for sale.");
            return;
        }

        if (stockItem.quantity < itemData.quantity)
        {
            Debug.LogWarning($"[Shop] Not enough quantity in stockroom to list {itemData.quantity} x {itemData.itemSO.itemName} for sale.");
            return;
        }

        // Decrement stockroom quantity (do NOT change currentWeightLoad here because item remains in shop)
        stockItem.quantity -= itemData.quantity;
        if (stockItem.quantity == 0)
        {
            stockroomItems.Remove(stockItem);
        }

        // Add or merge into itemsForSale
        ShopItemForSale existingForSale = itemsForSale.Find(i => i.itemData.itemSO == itemData.itemSO);
        if (existingForSale != null)
        {
            existingForSale.itemData.quantity += itemData.quantity;
        }
        else
        {
            // Add a new ShopItemForSale with a new ItemData instance
            itemsForSale.Add(new ShopItemForSale
            {
                itemData = new ItemData { itemSO = itemData.itemSO, quantity = itemData.quantity },
                salePrice = itemData.itemSO.baseValue
            });
        }

        Debug.Log($"[Shop] Listed {itemData.quantity} x {itemData.itemSO.itemName} for sale in '{name}'. (Stockroom -> ForSale). CurrentWeight remains {currentInventoryCapacity:F2}/{maxInventoryCapacity:F2}");
    }

    public float CalculatePurchaseChance(ShopItemForSale shopItemForSale)
    {
        // Get demand level from DemandManager
        float demandModifier = DemandManager.Instance.GetDemandLevel(shopItemForSale.itemData.itemSO) / 100f;

        // Calculate price fairness (how reasonable the price is)
        float priceRatio = shopItemForSale.salePrice / (float)shopItemForSale.itemData.itemSO.baseValue;
        float priceFairness;

        if (priceRatio <= 1.0f)
        {
            // Selling at or below base value - great deal
            priceFairness = 1.2f;
        }
        else if (priceRatio <= 1.5f)
        {
            // Reasonable markup (100-150%)
            priceFairness = 1.0f;
        }
        else if (priceRatio <= 2.0f)
        {
            // High markup (150-200%)
            priceFairness = 0.6f;
        }
        else
        {
            // Overpriced (>200%)
            priceFairness = 0.3f / priceRatio; // Diminishing returns
        }

        // Get worker skill modifier
        //float workerModifier = WorkerManager.Instance.GetAverageSalesWorkerSkill(this);

        // Calculate base chance with weighted factors
        float baseChance =
            (demandModifier * 0.5f) +        // 50% from demand
            (priceFairness * 0.5f); //+         // 50% from pricing
            //(workerModifier * 0.2f);         // 20% from worker skill

        // Foot traffic affects spawn rate, not purchase chance
        // But you could add a small bonus for premium locations if desired
        // baseChance += (footTrafficScore - 1.0f) * 0.05f; // Optional

        return Mathf.Clamp01(baseChance);
    }
}
