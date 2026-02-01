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
    public List<ItemData> itemsForSale = new();

    // Workers
    public List<Worker> employedWorkers = new();

    // Earnings Tracking
    public int earningsToday = 0;
    public int yesterdayEarnings = 0;
    public int totalEarnings = 0;
    public Dictionary<int, int> previousEarningHistory = new(); // Key: Day number, Value: Earnings

    // Resell Stall Specific
    public int currentItemSlots = 0; // Phasing out hardcoded slots in favor of dynamic item management with weight
    public int maxItemSlots = 0;
    public float maxWeightLoad = 200.0f; // Default max weight load, double the player carrying capacity
    public float currentWeightLoad = 0.0f;


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

    public bool HasRoomForItemWeight(ItemData itemData)
    {
        // Check if the shop has room for the item based on its weight and current load
        float incomingItemWeight = itemData.itemSO.weight * itemData.quantity;
        if (currentWeightLoad + incomingItemWeight <= maxWeightLoad)
        {
            return true;
        }
        return false;
    }

    public void AddItem(ItemData itemData)
    {
        // Check if item already exists in stockroom
        ItemData existingItem = stockroomItems.Find(i => i.itemSO == itemData.itemSO);
        if (existingItem != null)
        {
            // If it exists, update the quantity
            existingItem.quantity += itemData.quantity;
        }
        else
        {
            // If it doesn't exist, add it to the stockroom
            stockroomItems.Add(itemData);
        }
        currentWeightLoad += itemData.itemSO.weight * itemData.quantity;
    }

    public void RemoveItem(ItemData itemData)
    {
        ItemData existingItem = stockroomItems.Find(i => i.itemSO == itemData.itemSO);
        if (existingItem != null)
        {
            if (existingItem.quantity >= itemData.quantity)
            {
                existingItem.quantity -= itemData.quantity;
                currentWeightLoad -= itemData.itemSO.weight * itemData.quantity;
                if (existingItem.quantity == 0)
                {
                    stockroomItems.Remove(existingItem);
                }
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
        // Move item from stockroom to items for sale
        RemoveItem(itemData);
        ItemData existingItem = itemsForSale.Find(i => i.itemSO == itemData.itemSO);
        if (existingItem != null)
        {
            existingItem.quantity += itemData.quantity;
            if (existingItem.quantity <= 0)
            {
                itemsForSale.Remove(existingItem);
            }
        }
        else if (itemData.quantity > 0)
        {
            itemsForSale.Add(itemData);
        }
    }
}
