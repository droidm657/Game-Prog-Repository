using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPanelManager : MonoBehaviour
{
    public static DeathPanelManager Instance;

    [Header("UI")]
    public GameObject deathPanel;

    private bool playerIsDead = false;

    private void Awake()
    {
        Instance = this;

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    private void Update()
    {
        if (!playerIsDead)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            RetryGame();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ReturnToMenu();
        }
    }

    public void ShowDeathScreen()
    {
        playerIsDead = true;

        if (deathPanel != null)
            deathPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

public void RetryGame()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.ResetForRetry();
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene("Interior Castle Scene");
    }

    public void ReturnToMenu()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.ResetEverything();
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenuScene");
    }


}