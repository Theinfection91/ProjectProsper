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

    public string GenerateShopIDNumber()
    {
        int totalShops = allShops.Count + 1;

        // "D3" = pad with zeros to 3 digits
        return totalShops.ToString("D3");
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
