// DialogueSystem.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;
using Debug = UnityEngine.Debug;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;

    private VisualElement dialogueContainer;
    private Label dialogueText;
    private Label dialogueContinue;

    private Queue<string> dialogueQueue = new Queue<string>();
    private bool isDialogueOpen = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    [Header("Settings")]
    public float typingSpeed = 0.05f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeDialogueUI();
    }

    void InitializeDialogueUI()
    {
        // Find the active UI Document in the newly loaded scene
        UIDocument uiDoc = FindAnyObjectByType<UIDocument>();

        if (uiDoc == null || uiDoc.rootVisualElement == null)
        {
            Debug.LogWarning("DialogueSystem: No UIDocument found in this scene!");
            return;
        }

        var root = uiDoc.rootVisualElement;

        // Re-target the visual elements in the fresh scene
        dialogueContainer = root.Q<VisualElement>("dialogue-container");
        dialogueText = root.Q<Label>("dialogue-text");
        dialogueContinue = root.Q<Label>("dialogue-continue");

        HideDialogue();
    }

    void Update()
    {
        if (!isDialogueOpen) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogueQueue.Count > 0 ? dialogueQueue.Peek() : dialogueText.text;
                isTyping = false;
                dialogueContinue.style.display = DisplayStyle.Flex;
            }
            else if (dialogueQueue.Count > 0)
            {
                ShowNextLine();
            }
            else
            {
                HideDialogue();
            }
        }
    }

    public void StartDialogue(string[] lines)
    {
        Time.timeScale = 1f;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        if (ShootingSystem.Instance != null)
            ShootingSystem.Instance.enabled = false;

        dialogueQueue.Clear();
        foreach (string line in lines)
            dialogueQueue.Enqueue(line);

        isDialogueOpen = true;
        if (dialogueContainer != null) dialogueContainer.style.display = DisplayStyle.Flex;
        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            HideDialogue();
            return;
        }

        string line = dialogueQueue.Dequeue();
        if (dialogueContinue != null) dialogueContinue.style.display = DisplayStyle.None;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        if (dialogueText != null) dialogueText.text = "";

        foreach (char c in line)
        {
            if (dialogueText != null) dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        if (dialogueContinue != null)
        {
            dialogueContinue.style.display = DisplayStyle.Flex;
            dialogueContinue.text = (dialogueQueue.Count == 0) ? "Press E to close" : "Press E to continue";
        }
    }

    void HideDialogue()
    {
        isDialogueOpen = false;
        if (dialogueContainer != null) dialogueContainer.style.display = DisplayStyle.None;
        dialogueQueue.Clear();

        if (ShootingSystem.Instance != null)
            ShootingSystem.Instance.enabled = true;
    }

    public bool IsDialogueOpen() => isDialogueOpen;
}