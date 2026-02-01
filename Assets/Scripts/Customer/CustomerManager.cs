using Assets.Scripts.Customer;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }

    [Header("Customer Spawning")]
    [SerializeField] private float baseSpawnInterval = 30f;
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
        foreach (Shop shop in ShopManager.Instance.GetAllShops())
        {
            if (shop.isOpenForBusiness && shop.isWithinOperatingHours)
            {
                UpdateCustomerSpawning(shop);
            }
        }
    }

    private void UpdateCustomerSpawning(Shop shop)
    {
        if (!shopSpawnTimers.ContainsKey(shop))
        {
            shopSpawnTimers[shop] = 0f;
        }

        shopSpawnTimers[shop] += Time.deltaTime;

        if (shopSpawnTimers[shop] >= baseSpawnInterval)
        {
            SpawnCustomer(shop);
            shopSpawnTimers[shop] = 0f;
        }
    }

    private void SpawnCustomer(Shop shop)
    {
        Customer customer = new Customer
        {

        };
    }

    public void RegisterShopForCustomers(Shop shop)
    {
        if (!shopSpawnTimers.ContainsKey(shop))
        {
            shopSpawnTimers[shop] = 0f;
        }
    }

    public void UnregisterShop(Shop shop)
    {
        if (shopSpawnTimers.ContainsKey(shop))
        {
            shopSpawnTimers.Remove(shop);
        }
    }
}
