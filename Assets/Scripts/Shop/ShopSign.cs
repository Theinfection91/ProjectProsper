using Assets.Scripts.Interactions;
using Assets.Scripts.Shop;
using System.Collections.Generic;
using UnityEngine;

public class ShopSign : MonoBehaviour, IInteractable
{
    // Basic Info
    public string id;
    public Vector3 location;
    public bool isClaimed;
    //public Shop claimedShop = null;

    // Type and Size
    public List<ShopTypeSO> availableShopTypes;
    public ShopSize size;

    // Inventory
    public float maxInventoryCapacity;

    // Workers
    public int maxWorkers;

    // Other
    public int dailyRent;
    public int initialDepositAmount;
    public float footTrafficScore;

    public void Interact()
    {
        if (isClaimed)
        {
            Shop claimedShop = ShopManager.Instance.GetShop(id);
            if (claimedShop.ownership == ShopOwnership.Player)
            {
                Debug.Log($"Interacted with Player's Shop: {claimedShop.id}");
                if (size == ShopSize.StreetVendorStall)
                {
                    Shop stall = ShopManager.Instance.GetShop(claimedShop.id);
                    UIManager.Instance.PopulateStallShopCommand(this);
                    UIManager.Instance.OpenStallShopCommand();
                }
                return;
            }
            if (claimedShop.ownership == ShopOwnership.NPC)
            {
                Debug.Log($"Interacted with NPC's Shop: ID {claimedShop.id} - Owner: {claimedShop.ownerName} - Type: {claimedShop.shopType.shopName}");
                return;
            }
        }

        // Open Shop Claiming UI if not claimed
        Debug.Log($"Interacted with unclaimed Shop Sign: {id}");
        UIManager.Instance.PopulateShopSignData(this);
        UIManager.Instance.OpenShopSignUI();
    }

    public bool CanInteract()
    {
        return true; 
    }

    public void ClaimProperty(int claimedSignLayer)
    {
        isClaimed = true;
        gameObject.layer = claimedSignLayer;
    }
}
