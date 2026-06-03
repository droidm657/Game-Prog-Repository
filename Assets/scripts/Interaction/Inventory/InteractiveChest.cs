using UnityEngine;
using Debug = UnityEngine.Debug;

public class InteractiveChest : MonoBehaviour
{
    // Tracking states for the interaction sequence
    private enum ChestState { Closed, OpenWithItem, Empty }
    private ChestState currentState = ChestState.Closed;

    [Header("Animation Settings")]
    [SerializeField] private Animator chestAnimator;
    [SerializeField] private string openTriggerName = "Open";

    [Header("Interaction Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private string openPromptMessage = "Open Chest";
    [SerializeField] private string collectPromptMessage = "Take Item";

    [Header("Loot Contents")]
    [SerializeField] private ItemData containedItem;

    [Header("Visual Item Spawning")]
    [SerializeField] private Transform itemSpawnPoint;

    private bool isPlayerInRange = false;
    private GameObject spawnedVisualItemInstance;

    void Start()
    {
        if (chestAnimator == null) chestAnimator = GetComponent<Animator>();
        if (containedItem == null) Debug.LogWarning($"[Chest] No item assigned to {gameObject.name}!");
        if (itemSpawnPoint == null) Debug.LogError($"[Chest] Please assign a Spawn Point Transform on {gameObject.name}!");
    }

    void Update()
    {
        // Block actions if player is out of range
        if (!isPlayerInRange) return;

        if (Input.GetKeyDown(interactionKey))
        {
            switch (currentState)
            {
                case ChestState.Closed:
                    OpenChest();
                    break;

                case ChestState.OpenWithItem:
                    CollectItemFromChest();
                    break;

                case ChestState.Empty:
                    // Do nothing, chest is entirely looted
                    break;
            }
        }
    }

    private void OpenChest()
    {
        // Play the cover opening animation sequence
        if (chestAnimator != null)
        {
            chestAnimator.SetTrigger(openTriggerName);
        }

        // Clear out the prompt text layout immediately
        UIManager.Instance?.HidePickupPrompt();

        // Spawn the physical mesh asset inside the container space
        if (containedItem != null && containedItem.objectPrefab != null && itemSpawnPoint != null)
        {
            spawnedVisualItemInstance = Instantiate(
                containedItem.objectPrefab,
                itemSpawnPoint.position,
                itemSpawnPoint.rotation
            );

            // Parent it so it stays anchored firmly inside the basin floor
            spawnedVisualItemInstance.transform.SetParent(itemSpawnPoint);

            // Safety: Disable any item pick up triggers on the mesh so it doesn't double-interact
            if (spawnedVisualItemInstance.TryGetComponent<Collider>(out Collider col))
            {
                col.enabled = false;
            }

            Debug.Log($"[Chest] Opened and revealed {containedItem.itemName} physical mesh.");

            // Advance state and update the prompt text to "Take Item" immediately
            currentState = ChestState.OpenWithItem;
            UIManager.Instance?.ShowPickupPrompt($"{collectPromptMessage} [{interactionKey}]");
        }
        else
        {
            // Fallback if no prefab mesh is assigned to look at
            currentState = ChestState.Empty;
            Debug.LogWarning("[Chest] Opened, but no object prefab was available to display.");
        }
    }

    private void CollectItemFromChest()
    {
        if (containedItem == null || InventorySystem.Instance == null) return;

        // Try pushing the item asset data directly into your global inventory grid
        bool pickupSuccess = InventorySystem.Instance.AddItem(containedItem);

        if (pickupSuccess)
        {
            Debug.Log($"[Chest Looted] Player collected {containedItem.itemName} out of the chest container.");

            // Clear prompt text layout
            UIManager.Instance?.HidePickupPrompt();
            UIManager.Instance?.ShowPickupPrompt($"Found: {containedItem.name}!");

            // DESTROY the spawned model instance instantly to make it disappear from the scene layout
            if (spawnedVisualItemInstance != null)
            {
                Destroy(spawnedVisualItemInstance);
            }

            // Move state to empty so it cannot be interacted with anymore
            currentState = ChestState.Empty;
        }
        else
        {
            Debug.LogWarning("[Chest Looted Failed] Inventory reporting full! Clear space before collecting.");
            UIManager.Instance?.ShowPickupPrompt("Inventory Full!");
        }
    }

    // --- Trigger Handshakes ---

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            if (UIManager.Instance != null)
            {
                if (currentState == ChestState.Closed)
                {
                    UIManager.Instance.ShowPickupPrompt($"{openPromptMessage} [{interactionKey}]");
                }
                else if (currentState == ChestState.OpenWithItem)
                {
                    UIManager.Instance.ShowPickupPrompt($"{collectPromptMessage} [{interactionKey}]");
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            UIManager.Instance?.HidePickupPrompt();
        }
    }
}