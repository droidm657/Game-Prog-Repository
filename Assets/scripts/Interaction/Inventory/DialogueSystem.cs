// DialogueSystem.cs
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
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
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
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
                // Skip typing animation
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogueQueue.Count > 0
                    ? dialogueQueue.Peek()
                    : dialogueText.text;
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
        // Freeze player input during dialogue
        Time.timeScale = 1f;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        // Disable shooting while talking
        ShootingSystem.Instance.enabled = false;

        dialogueQueue.Clear();
        foreach (string line in lines)
            dialogueQueue.Enqueue(line);

        isDialogueOpen = true;
        dialogueContainer.style.display = DisplayStyle.Flex;
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
        dialogueContinue.style.display = DisplayStyle.None;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        dialogueContinue.style.display = DisplayStyle.Flex;

        // Auto show continue prompt
        if (dialogueQueue.Count == 0)
            dialogueContinue.text = "Press E to close";
        else
            dialogueContinue.text = "Press E to continue";
    }

    void HideDialogue()
    {
        isDialogueOpen = false;
        dialogueContainer.style.display = DisplayStyle.None;
        dialogueQueue.Clear();

        // Re-enable shooting
        ShootingSystem.Instance.enabled = true;
    }
    public bool IsDialogueOpen() => isDialogueOpen;
}