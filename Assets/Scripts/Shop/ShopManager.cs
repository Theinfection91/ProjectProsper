using Assets.Scripts.Shop;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("All Shops in the World")]
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
