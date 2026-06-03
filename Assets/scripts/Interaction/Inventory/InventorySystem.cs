using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    [Header("Inventory Settings")]
    public int maxInventorySize = 10;
    public List<ItemData> items = new List<ItemData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // This keeps the Inventory
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public bool AddItem(ItemData item)
    {
        if (item.isAmmo)
        {
            AmmoSystem.Instance.AddAmmo(item.ammoCount);
            Debug.Log($"Added {item.ammoCount} ammo from {item.itemName}");
            return true;
        }
        if (items.Count >= maxInventorySize)
        {
            Debug.Log("Inventory full!");
            return false;
        }

        items.Add(item);
        Debug.Log($"Picked up: {item.itemName}");
        return true;

    }

    public bool HasItem(ItemData item)
    {
        return items.Contains(item);
    }

    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
    }

}
