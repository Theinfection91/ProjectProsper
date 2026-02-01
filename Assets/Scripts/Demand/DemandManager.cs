using System.Collections.Generic;
using UnityEngine;

public class DemandManager : MonoBehaviour
{
    public static DemandManager Instance { get; private set; }

    public Dictionary<ItemSO, int> itemDemandLevels = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GenerateDemandLevels();
    }

    public void GenerateDemandLevels()
    {
        itemDemandLevels.Clear();
        var allItems = GameManager.Database.GetAllItems();
        foreach (var item in allItems)
        {
            int demandLevel = Random.Range(65, 101); // Demand level between 65 and 100 to start
            itemDemandLevels[item] = demandLevel;
        }
    }

    public int GetDemandLevel(ItemSO item)
    {
        if (itemDemandLevels.TryGetValue(item, out int demandLevel))
        {
            return demandLevel;
        }
        return 0; // Default demand level if not found
    }
}
