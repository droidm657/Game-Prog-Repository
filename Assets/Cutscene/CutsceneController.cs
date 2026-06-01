using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using Debug = UnityEngine.Debug;

public class CutsceneController : MonoBehaviour
{
    public static VideoClip VideoToPlay;
    public static string NextSceneName;

    [Tooltip("If you press Play directly inside this scene, what scene should load next?")]
    public string defaultNextScene = "Tutorial Scene";

    private VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        // If a video was passed from the Main Menu, use it.
        // Otherwise, it will just play whatever video you put in the Inspector!
        if (VideoToPlay != null)
        {
            videoPlayer.clip = VideoToPlay;
        }

        videoPlayer.Play();
    }

    void OnEnable() => videoPlayer.loopPointReached += OnVideoFinished;
    void OnDisable() => videoPlayer.loopPointReached -= OnVideoFinished;

    void OnVideoFinished(VideoPlayer vp) => LoadNextScene();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        VideoToPlay = null;

        // Use the menu destination, or fallback to the default if testing directly
        string sceneToLoad = string.IsNullOrEmpty(NextSceneName) ? defaultNextScene : NextSceneName;
        SceneManager.LoadScene(sceneToLoad);
    }
}