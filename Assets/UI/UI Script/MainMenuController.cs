using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static System.Net.Mime.MediaTypeNames;
using Debug = UnityEngine.Debug;
using Application = UnityEngine.Application;
using UnityEngine.Video;

public class MainMenuController : MonoBehaviour
{
    private UIDocument uiDocument;
    private Button playButton;
    private Button settingsButton;
    private Button quitButton;
    public VideoClip introVideo;

    [Header("Scene Configuration")]
    [Tooltip("The exact name of your gameplay scene to load.")]
    [SerializeField] private string cutsceneSceneName = "CutsceneScene";

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
    }

    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        // Query the buttons using the names from your UXML
        playButton = root.Q<Button>("play-button");
        settingsButton = root.Q<Button>("settings-button");
        quitButton = root.Q<Button>("quit-button");

        // Hook up the click events
        if (playButton != null) playButton.clicked += OnPlayPressed;
        if (settingsButton != null) settingsButton.clicked += OnSettingsPressed;
        if (quitButton != null) quitButton.clicked += OnQuitPressed;
    }

    void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        if (playButton != null) playButton.clicked -= OnPlayPressed;
        if (settingsButton != null) settingsButton.clicked -= OnSettingsPressed;
        if (quitButton != null) quitButton.clicked -= OnQuitPressed;
    }

    void OnPlayPressed()
    {
        CutsceneController.VideoToPlay = introVideo; // Set the video to play in the cutscene controller

        CutsceneController.NextSceneName = "Tutorial Scene"; // Set the next scene to load after the cutscene
        Debug.Log("Loading Game...");
        // Loads CutsceneScene
        SceneManager.LoadScene(cutsceneSceneName);
    }

    void OnSettingsPressed()
    {
        Debug.Log("Settings button clicked! (Placeholder for menu toggle)");
        // If you decide to add a simple popup panel later, you can toggle its display style here.
    }

    void OnQuitPressed()
    {
        Debug.Log("Exiting Game...");

#if UNITY_EDITOR
            // Allows the quit button to work cleanly inside the Unity Editor
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}