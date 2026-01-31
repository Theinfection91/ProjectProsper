using UnityEngine;

[CreateAssetMenu(fileName = "NewItemType", menuName = "Scriptable Objects/Item/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string id;
    public string itemName;
    [TextArea]
    public string description;
    public Sprite itemSprite;
    public int baseValue;
    public int weight;
    public int stackSize;
    public bool isRawMaterial;
    public bool isCraftedGood;
}
