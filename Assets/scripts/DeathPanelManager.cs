using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPanelManager : MonoBehaviour
{
    public static DeathPanelManager Instance;

    [Header("UI")]
    public GameObject deathPanel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        deathPanel.SetActive(false);
    }

    public void ShowDeathPanel()
    {
        deathPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void TryAgain()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenuScene");
    }
}