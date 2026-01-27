using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDatabase", menuName = "Scriptable Objects/Databases/Game Database")]
public class GameDatabase : ScriptableObject
{
    [Header("All Game Data")]
    public List<WorkerTypeSO> allWorkers = new List<WorkerTypeSO>();
    public List<ShopTypeSO> allShops = new List<ShopTypeSO>();
    public List<ItemTypeSO> allItems = new List<ItemTypeSO>();

    #region Worker Queries
    public WorkerTypeSO GetWorkerByName(string name)
    {
        return allWorkers.FirstOrDefault(w => w.workerName == name);
    }

    public List<WorkerTypeSO> GetWorkersByCategory(WorkerCategory category)
    {
        return allWorkers.Where(w => w.category == category).ToList();
    }

    public List<WorkerTypeSO> GetBasicWorkers()
    {
        return GetWorkersByCategory(WorkerCategory.Basic);
    }

    public List<WorkerTypeSO> GetGatherers()
    {
        return GetWorkersByCategory(WorkerCategory.Gatherer);
    }

    public List<WorkerTypeSO> GetServiceWorkers()
    {
        return GetWorkersByCategory(WorkerCategory.Service);
    }
    #endregion

    #region Shop Queries
    public ShopTypeSO GetShopByName(string name)
    {
        return allShops.FirstOrDefault(s => s.shopName == name);
    }
    #endregion

    #region Item Queries
    public ItemTypeSO GetItemByName(string name)
    {
        return allItems.FirstOrDefault(i => i.itemName == name);
    }

    public List<ItemTypeSO> GetRawMaterials()
    {
        return allItems.Where(i => i.isRawMaterial).ToList();
    }

    public List<ItemTypeSO> GetCraftableItems()
    {
        return allItems.Where(i => i.isCraftedGood).ToList();
    }
    #endregion
}