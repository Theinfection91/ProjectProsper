using Assets.Scripts.Item;
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
        TimeManager.OnHourChanged += HandleHourChanged;

        // Initialize shop states for the current time
        int currentHour = Mathf.FloorToInt(TimeManager.Instance.currentTimeOfDay * 24f);
        HandleHourChanged(currentHour);
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnEndOfDay -= HandleEndOfDay;
            TimeManager.OnHourChanged -= HandleHourChanged;
        }
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
            ownerName = PlayerCharacter.Instance.name,
            shopSchedule = new ShopSchedule()
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
        CustomerManager.Instance.RegisterShopForCustomers(newShop);
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

    private void HandleHourChanged(int currentHour)
    {
        UpdateShopOperatingStates(currentHour);
    }

    /// <summary>
    /// Updates all shop operating states based on current time and schedules.
    /// Call this when schedules change or time changes.
    /// </summary>
    public void UpdateShopOperatingStates(int currentHour)
    {
        foreach (Shop shop in allShops)
        {
            // Check if shop is within operating hours based on schedule
            bool withinSchedule = shop.shopSchedule.IsShopOpenNow(TimeManager.Instance.currentDayOfWeek, currentHour);
            shop.isWithinOperatingHours = withinSchedule;

            // Shop is "open for business" if it's player-owned, within operating hours, AND actively opened
            // TODO: Add isPlayerOpened or similar flag when you implement the player opening/closing the shop
            shop.isOpenForBusiness = shop.ownership == ShopOwnership.Player && withinSchedule;
        }
    }

    /// <summary>
    /// Call this when a shop's schedule is modified to immediately update its operating state.
    /// </summary>
    public void RefreshShopOperatingState(Shop shop)
    {
        int currentHour = Mathf.FloorToInt(TimeManager.Instance.currentTimeOfDay * 24f);
        bool withinSchedule = shop.shopSchedule.IsShopOpenNow(TimeManager.Instance.currentDayOfWeek, currentHour);
        shop.isWithinOperatingHours = withinSchedule;
        shop.isOpenForBusiness = shop.ownership == ShopOwnership.Player && withinSchedule;
        
        Debug.Log($"[ShopManager] Refreshed '{shop.name}' - Open: {shop.isOpenForBusiness}, Within Hours: {shop.isWithinOperatingHours}");
    }

    private void HandleEndOfDay()
    {
        int totalEarnings = 0;
        foreach (Shop shop in allShops)
        {
            PlayerInventory.Instance.AddGold(shop.earningsToday);
            totalEarnings += shop.earningsToday;
            shop.yesterdayEarnings = shop.earningsToday;
            shop.earningsToday = 0;
            // TODO Update earning history
            //int dayIndex = TimeManager.Instance.GetCurrentDayIndex();
            //shop.previousEarningHistory[dayIndex] = shop.yesterdayEarnings;
            DeductDailyRent(shop);
        }
        Debug.Log($"[ShopManager] End of Day: Added total earnings of {totalEarnings} gold from all shops to player inventory.");
        
        
        int allShopRent = 0;
        foreach (Shop shop in allShops)
        {
            if (shop.ownership == ShopOwnership.Player)
            {
                allShopRent += shop.dailyRent;
            }
        }
        Debug.Log($"Deducted {allShopRent} gold for daily rent from all player-owned shops.");
    }
    
    public void DeductDailyRent(Shop shop)
    {
        if (shop == null) return;

        // Deduct the daily rent from the player's gold if they own the shop
        if (shop.ownership == ShopOwnership.Player)
        {
            PlayerInventory.Instance.RemoveGold(shop.dailyRent);
            
        }
    }

    #region Wholesale Purchasing (Items->Player)
    public bool CanBuyFromWholesaler(Wholesaler wholesaler, ItemData itemData)
    {
        // Check if player has enough gold (Whole sale item price is base value for simplicity)
        int totalCost = itemData.itemSO.baseValue * itemData.quantity;
        if (!PlayerInventory.Instance.HasGoldAmount(totalCost))
        {
            Debug.Log("Not enough gold to buy from wholesaler.");
            return false;
        }
        // Check player inventory weight capacity
        if (!PlayerInventory.Instance.CanCarryItemWeight(itemData.itemSO.weight * itemData.quantity))
        {
            Debug.Log("Not enough inventory capacity to carry items from wholesaler.");
            return false;
        }
        return true;
    }

    public void BuyFromWholesaler(Wholesaler wholesaler, ItemData itemData)
    {
        int totalCost = itemData.itemSO.baseValue * itemData.quantity;
        PlayerInventory.Instance.RemoveGold(totalCost);
        PlayerInventory.Instance.AddItem(itemData.itemSO, itemData.quantity);

        // Find the actual item in the wholesaler's list and decrement its quantity
        var wholesalerItem = wholesaler.itemsForSale.Find(i => i.itemSO == itemData.itemSO);
        if (wholesalerItem != null)
        {
            wholesalerItem.quantity -= itemData.quantity;
            if (wholesalerItem.quantity <= 0)
            {
                wholesaler.RemoveItem(wholesalerItem);
            }
        }

        Debug.Log($"Bought {itemData.quantity} of {itemData.itemSO.itemName} from wholesaler for {totalCost} gold.");
    }
    #endregion

    #region Shop Stock Management (Player Shops)

    #endregion

    #region Getters
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
    #endregion
}
