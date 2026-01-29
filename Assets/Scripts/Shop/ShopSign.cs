using Assets.Scripts.Interactions;
using System.Collections.Generic;
using UnityEngine;

public class ShopSign : MonoBehaviour, IInteractable
{
    // Basic Info
    public string id;
    public Vector3 location;
    public bool isClaimed;
    public Shop claimedShop = null;

    // Type and Size
    public List<ShopTypeSO> availableShopTypes;
    public ShopSize size;

    // Inventory
    public int maxItems;
    public int currentItems;

    // Workers
    public int maxWorkers;

    // Other
    public int dailyRent;
    public int initialDepositAmount;
    public float footTrafficRating;

    public void Interact()
    {
        // Open Shop Claiming UI if not claimed
        Debug.Log("Interacted with Shop Sign: " + id);
    }

    public bool CanInteract()
    {
        return !isClaimed;
    }

    public void ClaimProperty(int claimedSignLayer)
    {
        isClaimed = true;
        gameObject.layer = claimedSignLayer;
    }
}
