using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionDoor : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("The exact name of the scene you want to load")]
    public string sceneToLoad;

    [Header("Interaction Settings")]
    public KeyCode interactionKey = KeyCode.E;
    public string promptMessage = "Enter";

    [Header("Spawn Position Settings")]
    [Tooltip("Type the EXACT Hierarchy name of the empty GameObject the player should spawn at in the next scene")]
    public string targetSpawnPointName;

    [Header("Progression Gate (Optional)")]
    public bool requiresEscapeKeys = false;
    public ItemData escapeKeyItemData;
    public int requiredKeyCount = 3;

    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            if (requiresEscapeKeys)
            {
                TryUnlockAndLoad();
            }
            else
            {
                PrepareAndLoadNextArea();
            }
        }
    }

    void TryUnlockAndLoad()
    {
        if (InventorySystem.Instance == null || escapeKeyItemData == null) return;

        int keysHeld = 0;
        foreach (ItemData item in InventorySystem.Instance.items)
        {
            if (item == escapeKeyItemData) keysHeld++;
        }

        if (keysHeld >= requiredKeyCount)
        {
            // Clear items out of inventory on complete escape
            for (int i = InventorySystem.Instance.items.Count - 1; i >= 0; i--)
            {
                if (InventorySystem.Instance.items[i] == escapeKeyItemData)
                    InventorySystem.Instance.items.RemoveAt(i);
            }

            PrepareAndLoadNextArea();
        }
        else
        {
            int missingKeys = requiredKeyCount - keysHeld;
            string lockedMessage = $"Locked! Need {missingKeys} more Escape Key{(missingKeys > 1 ? "s" : "")}.";
            UIManager.Instance?.ShowPickupPrompt(lockedMessage);
        }
    }

    void PrepareAndLoadNextArea()
    {
        if (UIManager.Instance != null) UIManager.Instance.HidePickupPrompt();

        // Save the target location name string into global local memory before changing scenes
        if (!string.IsNullOrEmpty(targetSpawnPointName))
        {
            PlayerPrefs.SetString("NextSpawnPoint", targetSpawnPointName);
            PlayerPrefs.Save();
            Debug.Log($"[Scene Transition] Saved destination spawn tag: '{targetSpawnPointName}'");
        }

        Debug.Log($"Loading scene: {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            UIManager.Instance?.ShowPickupPrompt(requiresEscapeKeys ? $"Unlock Gate [{interactionKey}]" : $"{promptMessage} [{interactionKey}]");
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
