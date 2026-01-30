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

    // Workers
    public List<Worker> employedWorkers = new();

    // Earnings Tracking
    public int earningsToday = 0;
    public int yesterdayEarnings = 0;
    public int totalEarnings = 0;
    public Dictionary<int, int> previousEarningHistory = new(); // Key: Day number, Value: Earnings

    // Resell Stall Specific
    public int currentItemSlots = 0;
    public int maxItemSlots = 0;
    public float footTrafficScore = 1.0f;

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
}
