using System.Collections.Generic;
using UnityEngine;

public class ShopSign : MonoBehaviour
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
