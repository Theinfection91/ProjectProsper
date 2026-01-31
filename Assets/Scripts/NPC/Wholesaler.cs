using Assets.Scripts.Item;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Wholesaler : NPC
{
    public List<ItemData> itemsForSale = new();

    public override void Interact()
    {
        Debug.Log("Welcome to my wholesale store! Take a look at our bulk deals.");
        UIManager.Instance.PopulateWholesaleItems(this);
        UIManager.Instance.OpenWholesaleUI();
    }

    public void RemoveItem(ItemData itemData)
    {
        itemsForSale.Remove(itemData);
    }
}
