using Assets.Scripts.Item;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    public int gold = 0;
    public float maxInventoryWeight = 100f;
    public float currentInventoryWeight = 0f;

    public List<ItemData> items = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool HasGoldAmount(int amount)
    {
        return gold >= amount;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"Gold added: {amount}. Total gold: {gold}");
    }

    public void RemoveGold(int amount)
    {
        // Gold may go negative
        gold -= amount;
    }

    public bool CanCarryItemWeight(float weight)
    {
        return (currentInventoryWeight + weight) <= maxInventoryWeight;
    }

    public void AddItem(ItemSO item, int quantity)
    {
        // Check if item already exists in inventory
        ItemData existingItem = items.Find(i => i.itemSO == item);
        if (existingItem != null)
        {
            existingItem.quantity += quantity;
        }
        else
        {
            items.Add(new ItemData { itemSO = item, quantity = quantity });
        }
        currentInventoryWeight += item.weight * quantity;
        Debug.Log($"Added {quantity} x {item.itemName} to inventory.");
    }

    public void RemoveItem(ItemSO item, int quantity)
    {
        ItemData existingItem = items.Find(i => i.itemSO == item);
        if (existingItem != null)
        {
            existingItem.quantity -= quantity;
            if (existingItem.quantity <= 0)
            {
                items.Remove(existingItem);
            }
            currentInventoryWeight -= item.weight * quantity;
            if (currentInventoryWeight < 0)
            {
                currentInventoryWeight = 0;
            }
            Debug.Log($"Removed {quantity} x {item.itemName} from inventory.");
        }
    }
}
