using Assets.Scripts.Customer;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }

    [Header("Customer Spawning")]
    [SerializeField] private float baseSpawnInterval = 30f; // In-game seconds
    [SerializeField] private bool debugLogging = true;
    
    [Header("Debug Display")]
    [SerializeField] private bool displayTimersInInspector = true;
    [SerializeField] private List<string> timerDebugDisplay = new();
    
    private Dictionary<Shop, float> shopSpawnTimers = new();

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

    private void Update()
    {
        // Always update debug display, even when paused
        if (displayTimersInInspector)
        {
            UpdateDebugDisplay();
        }

        // Don't update timers when paused
        if (GameManager.IsGamePaused || TimeManager.Instance.currentSpeed == TimeManager.TimeSpeed.Paused)
        {
            return;
        }

        foreach (Shop shop in ShopManager.Instance.GetAllShops())
        {
            if (shop.isOpenForBusiness && shop.isWithinOperatingHours)
            {
                UpdateCustomerSpawning(shop);
            }
        }
    }

    /// <summary>
    /// Calculates the effective spawn interval for a shop based on its foot traffic.
    /// Every +0.1 foot traffic above 1.0 reduces the interval by 5%, up to 50% max reduction.
    /// </summary>
    private float GetEffectiveInterval(Shop shop)
    {
        float reduction = Mathf.Clamp01((shop.footTrafficScore - 1f) * 0.5f); // 0.5f = 5% per 0.1
        return baseSpawnInterval * (1f - reduction);
    }

    private void UpdateDebugDisplay()
    {
        timerDebugDisplay.Clear();

        string pauseState = GameManager.IsGamePaused ? "PAUSED (Menu)" : "Running";
        string timeSpeed = TimeManager.Instance.currentSpeed.ToString();
        float multiplier = TimeManager.Instance.GetSpeedMultiplier();

        timerDebugDisplay.Add($"Game State: {pauseState}");
        timerDebugDisplay.Add($"Time Speed: {timeSpeed} ({multiplier}x)");
        timerDebugDisplay.Add("---");

        if (shopSpawnTimers.Count == 0)
        {
            timerDebugDisplay.Add("No shops registered for customer spawning");
        }
        else
        {
            foreach (var kvp in shopSpawnTimers)
            {
                Shop shop = kvp.Key;
                float timer = kvp.Value;
                float effectiveInterval = GetEffectiveInterval(shop);
                float progress = Mathf.Clamp01(timer / effectiveInterval);
                string progressBar = GenerateProgressBar(progress);

                string shopState = "";
                if (!shop.isOpenForBusiness)
                    shopState = " [CLOSED]";
                else if (!shop.isWithinOperatingHours)
                    shopState = " [OUTSIDE HOURS]";

                timerDebugDisplay.Add($"{shop.name}{shopState}: {timer:F1}s / {effectiveInterval:F1}s {progressBar} (Foot Traffic: {shop.footTrafficScore:F2}x)");
            }
        }
    }

    private string GenerateProgressBar(float progress)
    {
        int filled = Mathf.RoundToInt(progress * 10f);
        return $"[{new string('█', filled)}{new string('░', 10 - filled)}]";
    }

    private void UpdateCustomerSpawning(Shop shop)
    {
        if (!shopSpawnTimers.ContainsKey(shop))
        {
            shopSpawnTimers[shop] = 0f;
            if (debugLogging)
            {
                Debug.Log($"[CustomerManager] Started tracking '{shop.name}'");
            }
        }

        float timeMultiplier = TimeManager.Instance.GetSpeedMultiplier();
        float deltaTime = Time.deltaTime * timeMultiplier;
        shopSpawnTimers[shop] += deltaTime;

        float effectiveInterval = GetEffectiveInterval(shop);

        if (debugLogging && (int)(shopSpawnTimers[shop] / 10f) > (int)((shopSpawnTimers[shop] - deltaTime) / 10f))
        {
            Debug.Log($"[CustomerManager] {shop.name} - Progress: {shopSpawnTimers[shop]:F1}s / {effectiveInterval:F1}s ({(shopSpawnTimers[shop]/effectiveInterval)*100:F0}%)");
        }

        if (shopSpawnTimers[shop] >= effectiveInterval)
        {
            if (debugLogging)
            {
                Debug.Log($"[CustomerManager] {shop.name} - Timer reached! Attempting spawn...");
            }
            SpawnCustomer(shop);
            shopSpawnTimers[shop] = 0f;
        }
    }

    private void SpawnCustomer(Shop shop)
    {
        // Validate shop can attract customers
        if (!IsShopAttractive(shop))
        {
            if (debugLogging)
            {
                Debug.LogWarning($"[CustomerManager] ❌ Spawn blocked for '{shop.name}'");
            }
            return;
        }

        Customer customer = new Customer();
        
        if (debugLogging)
        {
            Debug.Log($"[CustomerManager] ✓✓✓ CUSTOMER SPAWNED for '{shop.name}' ✓✓✓");
        }
    }

    /// <summary>
    /// Determines if a shop should spawn customers based on operational requirements.
    /// </summary>
    private bool IsShopAttractive(Shop shop)
    {
        // Check if shop is open for business
        if (!shop.isOpenForBusiness)
        {
            if (debugLogging) Debug.LogWarning($"[CustomerManager] FAILED: '{shop.name}' - isOpenForBusiness = FALSE");
            return false;
        }

        // Check if within operating hours
        if (!shop.isWithinOperatingHours)
        {
            if (debugLogging) Debug.LogWarning($"[CustomerManager] FAILED: '{shop.name}' - isWithinOperatingHours = FALSE");
            return false;
        }

        // Check if shop has items for sale
        if (shop.itemsForSale == null || shop.itemsForSale.Count == 0)
        {
            if (debugLogging) Debug.LogWarning($"[CustomerManager] FAILED: '{shop.name}' - NO ITEMS FOR SALE");
            return false;
        }

        // Check if someone is working the shop
        if (!HasStaffWorking(shop))
        {
            if (debugLogging) Debug.LogWarning($"[CustomerManager] FAILED: '{shop.name}' - NO STAFF ({shop.employedWorkers.Count} employees)");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Determines if a shop has staff working (including the player).
    /// </summary>
    private bool HasStaffWorking(Shop shop)
    {
        if (shop.employedWorkers != null && shop.employedWorkers.Count > 0 || shop.hasPlayerWorking)
        {
            return true;
        }

        // TODO: Check if player is working in this shop
        // return PlayerCharacter.Instance.currentlyWorkingInShop == shop;

        return false;
    }

    public float GetShopSpawnTimer(Shop shop)
    {
        return shopSpawnTimers.ContainsKey(shop) ? shopSpawnTimers[shop] : 0f;
    }

    public void RegisterShopForCustomers(Shop shop)
    {
        if (!shopSpawnTimers.ContainsKey(shop))
        {
            shopSpawnTimers[shop] = 0f;
            if (debugLogging) Debug.Log($"[CustomerManager] ✓ Registered '{shop.name}' for customer spawning");
        }
    }

    public void UnregisterShop(Shop shop)
    {
        if (shopSpawnTimers.ContainsKey(shop))
        {
            shopSpawnTimers.Remove(shop);
            if (debugLogging) Debug.Log($"[CustomerManager] Unregistered '{shop.name}'");
        }
    }
}
