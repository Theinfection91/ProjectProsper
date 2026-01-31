using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    public int gold = 0;
    public float maxInventoryWeight = 100f;
    public float currentInventoryWeight = 0f;

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
}
