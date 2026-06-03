//WorldItemPickUp.cs
using UnityEngine;
using Debug = UnityEngine.Debug;
public class WorldItemPickUp : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [Tooltip("Custom message like 'Pick up Herb' or 'Take Key'")]
    [SerializeField] private string promptMessage = "Pick up Herb";

    [Header("Item Data")]
    [Tooltip("Assign the ScriptableObject asset this world object represents")]
    [SerializeField] private ItemData itemData;

    private bool isPlayerInRange = false;

    void Start()
    {
        if (itemData == null)
        {
            Debug.LogError($"[Pickup Error] No ItemData ScriptableObject assigned to {gameObject.name}!");
        }
    }

    void Update()
    {
        // If the player is nearby and presses the interaction key, harvest the item
        if (isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        if (itemData == null || InventorySystem.Instance == null) return;

        // Try adding the item asset data directly to the global persistent inventory
        bool pickupSuccess = InventorySystem.Instance.AddItem(itemData);

        if (pickupSuccess)
        {
            Debug.Log($"[World Pickup] Player successfully collected {itemData.itemName} from the environment.");

            // Clear out the interaction prompt layout instantly
            UIManager.Instance?.HidePickupPrompt();

            // Optional: Briefly show what they found (e.g., "Found: Herb!")
            UIManager.Instance?.ShowPickupPrompt($"Found: {itemData.name}!");

            // Destroy this entire world object (including its meshes/pot) so it disappears
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("[World Pickup Failed] Cannot harvest item: Inventory system reports full storage capacity.");
            UIManager.Instance?.ShowPickupPrompt("Inventory Full!");
        }
    }

    // --- Trigger Handshakes ---

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            if (UIManager.Instance != null && itemData != null)
            {
                UIManager.Instance.ShowPickupPrompt($"{promptMessage} [{interactionKey}]");
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
