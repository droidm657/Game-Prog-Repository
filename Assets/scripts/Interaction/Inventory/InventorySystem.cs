using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    [Header("Inventory Settings")]
    public int maxInventorySize = 10;
    public List<ItemData> items = new List<ItemData>();

    // Event for refreshing inventory UI
    public event Action OnInventoryChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // NEW: Reset inventory on Retry/Menu
public void ResetForRetry()
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (!items[i].keepOnRetry)
            {
                items.RemoveAt(i);
            }
        }

        Debug.Log("Retry Inventory Reset");

        OnInventoryChanged?.Invoke();
    }

    public void ResetEverything()
    {
        items.Clear();

        Debug.Log("Full Inventory Reset");

        OnInventoryChanged?.Invoke();
    }



    public void UseConsumableItem(ItemData consumableItem)
    {
        if (consumableItem == null) return;
        if (!consumableItem.isConsumable || !items.Contains(consumableItem)) return;

        if (HealthSystem.Instance != null)
        {
            if (HealthSystem.Instance.GetHealth() >= HealthSystem.Instance.maxHealth)
            {
                Debug.Log("Cannot use potion/herb: Health is already maxed out!");

                UIManager.Instance?.ShowPickupPrompt("Health already full!");
                return;
            }

            HealthSystem.Instance.Heal(consumableItem.healthRestoreAmount);

            RemoveItem(consumableItem);

            Debug.Log($"Used {consumableItem.itemName}. Restored {consumableItem.healthRestoreAmount} Health.");

            UIManager.Instance?.HidePickupPrompt();
        }
        else
        {
            Debug.LogWarning("Cannot use consumable: No active Player HealthSystem found in this scene!");
        }
    }

    public bool AddItem(ItemData item)
    {
        if (item == null) return false;

        if (item.isAmmo)
        {
            if (AmmoSystem.Instance != null)
            {
                AmmoSystem.Instance.AddAmmo(item.ammoCount);
                Debug.Log($"Added {item.ammoCount} ammo from {item.itemName}");
                return true;
            }

            return false;
        }

        if (items.Count >= maxInventorySize)
        {
            Debug.Log("Inventory full!");
            return false;
        }

        items.Add(item);

        Debug.Log($"Picked up: {item.itemName}");

        OnInventoryChanged?.Invoke();

        return true;
    }

    public bool HasItem(ItemData item)
    {
        return items.Contains(item);
    }

    public void RemoveItem(ItemData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);

            OnInventoryChanged?.Invoke();
        }
    }
}

