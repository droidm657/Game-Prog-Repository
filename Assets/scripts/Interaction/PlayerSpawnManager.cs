using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerSpawnManager : MonoBehaviour
{
    void OnEnable()
    {
        // Execute placement check on startup
        CheckAndPositionPlayer();
    }

    public void CheckAndPositionPlayer()
    {
        // 1. Check if a specific target spawn point was stored in memory
        if (!PlayerPrefs.HasKey("NextSpawnPoint")) return;

        string targetPointName = PlayerPrefs.GetString("NextSpawnPoint");
        if (string.IsNullOrEmpty(targetPointName)) return;

        // 2. Locate the player prefab in the scene
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[Spawn Manager] Could not find Player object with tag 'Player' to teleport!");
            return;
        }

        // 3. Find all potential spawn anchors in this scene map
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        GameObject targetAnchor = null;

        foreach (GameObject point in spawnPoints)
        {
            if (point.name == targetPointName)
            {
                targetAnchor = point;
                break;
            }
        }

        // 4. Execute the position warp if a name match is resolved
        if (targetAnchor != null)
        {
            Debug.Log($"[Spawn Manager] Teleporting player to destination anchor: {targetPointName}");

            // Disable character controllers briefly if using physics movement scripts to prevent warp snapping resistance
            if (player.TryGetComponent<CharacterController>(out CharacterController cc))
            {
                cc.enabled = false;
                player.transform.position = targetAnchor.transform.position;
                player.transform.rotation = targetAnchor.transform.rotation;
                cc.enabled = true;
            }
            else
            {
                player.transform.position = targetAnchor.transform.position;
                player.transform.rotation = targetAnchor.transform.rotation;
            }

            // Clear out the tracking memory token so it doesn't loop accidentally next restart
            PlayerPrefs.DeleteKey("NextSpawnPoint");
        }
        else
        {
            Debug.LogWarning($"[Spawn Manager] Expected spawn location anchor '{targetPointName}' but it was not found in this scene!");
        }
    }
}