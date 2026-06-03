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

    private bool isPlayerInRange = false;

    void Update()
    {
        // If player is nearby and presses E, change scenes
        if (isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            LoadNextArea();
        }
    }

    void LoadNextArea()
    {
        // Clean up UI prompt before leaving so it doesn't get stuck
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HidePickupPrompt();
        }

        Debug.Log($"Loading scene: {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            // Reusing UI manager prompt for the door!
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPickupPrompt(promptMessage);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.HidePickupPrompt();
            }
        }
    }
}
