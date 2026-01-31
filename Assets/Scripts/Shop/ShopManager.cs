using Assets.Scripts.Shop;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("All Shops in the World")]
    [SerializeField]
    private List<Shop> allShops = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        TimeManager.Instance.OnEndOfDay += HandleEndOfDay;
    }

    public bool CanPlayerOpenNewShop()
    {
        if (PlayerCharacter.Instance.currentShopsCount == PlayerCharacter.Instance.maxShopsCount)
        {
            return false;
        }
        return true;
    }

    public Shop CreatePlayerShop(string id, string shopName, ShopSize shopSize, int rent, float footTraffic, int currentItemSlots = 0, int maxItemSlots = 0)
    {
        Shop newShop = new Shop(id, shopName)
        {
            size = shopSize,
            dailyRent = rent,
            footTrafficScore = footTraffic,
            ownership = ShopOwnership.Player,
            ownerName = PlayerCharacter.Instance.name
        };

        // For street vendor stalls, set item slots if provided
        if (shopSize == ShopSize.StreetVendorStall)
        {
            newShop.SetShopType(GameManager.Instance.database.GetShopByName("Resell Stall"));
        }
        if (currentItemSlots > 0)
        {
            newShop.currentItemSlots = currentItemSlots;
        }
        if (maxItemSlots > 0)
        {
            newShop.maxItemSlots = maxItemSlots;
        }

        RegisterShop(newShop);
        return newShop;
    }

    public void RegisterShop(Shop shop)
    {
        if (!allShops.Contains(shop))
        {
            allShops.Add(shop);
        }
    }

    public void UnregisterShop(Shop shop)
    {
        if (allShops.Contains(shop))
        {
            allShops.Remove(shop);
        }
    }

    private void HandleEndOfDay()
    {
        foreach (Shop shop in allShops)
        {
            shop.yesterdayEarnings = shop.earningsToday;
            shop.earningsToday = 0;
            // TODO Update earning history
            //int dayIndex = TimeManager.Instance.GetCurrentDayIndex();
            //shop.previousEarningHistory[dayIndex] = shop.yesterdayEarnings;
            DeductDailyRent(shop);
        }
    }
    
    public void DeductDailyRent(Shop shop)
    {
        if (shop == null) return;

        // Deduct the daily rent from the player's gold if they own the shop
        if (shop.ownership == ShopOwnership.Player)
        {
            PlayerInventory.Instance.RemoveGold(shop.dailyRent);
            Debug.Log($"Deducted {shop.dailyRent} gold for daily rent of shop {shop.name}.");
        }
    }

    public Shop GetShop(string id)
    {
        return allShops.Find(shop => shop.id == id);
    }

    public List<Shop> GetAllShops()
    {
        return allShops;
    }

    public List<Shop> GetPlayerOwnedShops(string playerName)
    {
        return allShops.FindAll(shop => shop.ownership == ShopOwnership.Player && shop.ownerName == playerName);
    }

    public List<Shop> GetNPCShops()
    {
        return allShops.FindAll(shop => shop.ownership == ShopOwnership.NPC);
    }

    public List<Shop> GetUnclaimedShops()
    {
        return allShops.FindAll(shop => shop.ownership == ShopOwnership.Unclaimed);
    }
}
