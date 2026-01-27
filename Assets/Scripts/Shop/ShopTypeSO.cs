using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopTypeSO", menuName = "Scriptable Objects/Shop/ShopTypeSO")]
public class ShopTypeSO : ScriptableObject
{
    [Header("Basic Info")]
    public string shopName;
    public string description;
    public Sprite shopIcon;

    [Header("Inventory")]
    public List<ItemTypeSO> sellableItems = new();

    [Header("Worker Info")]
    public WorkerTypeSO specialistType;
}