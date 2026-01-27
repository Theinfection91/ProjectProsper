using UnityEngine;

[CreateAssetMenu(fileName = "NewItemType", menuName = "Scriptable Objects/Item/ItemTypeSO")]
public class ItemTypeSO : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite itemSprite;
    public int baseValue;
    public bool isRawMaterial;
    public bool isCraftedGood;
}
