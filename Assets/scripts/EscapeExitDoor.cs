using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class EscapeExitDoor : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("The exact name of the victory or next scene")]
    [SerializeField] private string victorySceneName;

    [Header("Interaction Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private string basePromptMessage = "Unlock Gate";

    [Header("Required Progression Keys")]
    [Tooltip("The exact name string given to your escape keys in the ScriptableObject")]
    [SerializeField] private string keyNameIdentifier = "Escape Key";
    [SerializeField] private int requiredKeyCount = 3;

    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            TryEscape();
        }
    }

    private void TryEscape()
    {
        if (InventorySystem.Instance == null) return;

        // Count keys matching our enum type and name identifier
        int keysHeld = CountHeldEscapeKeys();

        if (keysHeld >= requiredKeyCount)
        {
            // SUCCESS: Open the door and advance the scene
            ExecuteEscapeSequence();
        }
        else
        {
            // FAILURE: Deny exit, update prompt with remaining count
            int missingKeys = requiredKeyCount - keysHeld;
            string failMessage = $"Locked! Need {missingKeys} more Escape Key{(missingKeys > 1 ? "s" : "")}.";

            UIManager.Instance?.ShowPickupPrompt(failMessage);
            Debug.Log($"Access Denied: Player only has {keysHeld}/{requiredKeyCount} keys named '{keyNameIdentifier}'.");
        }
    }

    private int CountHeldEscapeKeys()
    {
        int count = 0;

        // Loop through the inventory items list
        foreach (ItemData item in InventorySystem.Instance.items)
        {
            // Verify BOTH that the type enum is KeyItem AND the string name matches perfectly
            if (item != null && item.itemType == ItemData.ItemType.KeyItem && item.itemName == keyNameIdentifier)
            {
                count++;
            }
        }
        return count;
    }

    private void ExecuteEscapeSequence()
    {
        UIManager.Instance?.HidePickupPrompt();

        // Consume and clean up the keys from the list so they don't persist
        for (int i = InventorySystem.Instance.items.Count - 1; i >= 0; i--)
        {
            ItemData item = InventorySystem.Instance.items[i];
            if (item != null && item.itemType == ItemData.ItemType.KeyItem && item.itemName == keyNameIdentifier)
            {
                InventorySystem.Instance.items.RemoveAt(i);
            }
        }

        Debug.Log("Castle escaped successfully! Transitioning scene...");
        SceneManager.LoadScene(victorySceneName);
    }

    // --- Trigger Handshakes ---

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            int keysHeld = CountHeldEscapeKeys();
            if (keysHeld >= requiredKeyCount)
            {
                UIManager.Instance?.ShowPickupPrompt($"{basePromptMessage} [{interactionKey}]");
            }
            else
            {
                UIManager.Instance?.ShowPickupPrompt($"Inspect Lock ({keysHeld}/{requiredKeyCount}) [{interactionKey}]");
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