using Assets.Scripts.Shop;
using Assets.Scripts.Worker;
using System.Collections.Generic;
using UnityEngine;

public class Shop
{
    public string id;
    public string name;
    public ShopTypeSO shopType = null;
    public ShopOwnership ownership = ShopOwnership.Unclaimed;
    public string ownerName;

    public List<Worker> employedWorkers = new();

    public int daysRented;

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
