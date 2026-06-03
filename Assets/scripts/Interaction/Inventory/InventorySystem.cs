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

    // An event your Inventory Grid UI can listen to for updating the visual squares instantly
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

    public void UseConsumableItem(ItemData consumableItem)
    {
        // 1. Validation checks
        if (consumableItem == null) return;
        if (!consumableItem.isConsumable || !items.Contains(consumableItem)) return;

        if (HealthSystem.Instance != null)
        {
            // 2. The Full Health Safety Net
            if (HealthSystem.Instance.GetHealth() >= HealthSystem.Instance.maxHealth)
            {
                Debug.Log("Cannot use potion/herb: Health is already maxed out!");

                // Let the player know visually via the UI prompt overlay
                UIManager.Instance?.ShowPickupPrompt("Health already full!");
                return; // EXIT EARLY: Potion is saved!
            }

            // 3. Match this variable name to your exact ScriptableObject field declaration
            HealthSystem.Instance.Heal(consumableItem.healthRestoreAmount);

            // 4. Consume item and refresh
            RemoveItem(consumableItem);
            Debug.Log($"Used {consumableItem.itemName}. Restored {consumableItem.healthRestoreAmount} Health.");

            // Clear prompt text overlay after using
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

        // Alert UI grid screens to redraw their slots
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

            // Alert UI grid screens to redraw their slots
            OnInventoryChanged?.Invoke();
        }
    }
}