// UIManager.cs
using UnityEngine;
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private UIDocument uiDocument;
    private VisualElement pickupPrompt;
    private Label pickupText;

    private VisualElement staminaBarFill;
    private VisualElement staminaContainer;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        uiDocument = GetComponent<UIDocument>();
    }

    void Start()
    {
        var root = uiDocument.rootVisualElement;
        pickupPrompt = root.Q<VisualElement>("pickup-prompt");
        pickupText = root.Q<Label>("pickup-text");
        staminaBarFill = root.Q<VisualElement>("stamina-bar-fill");
        staminaContainer = root.Q<VisualElement>("stamina-container");

        // Hide at start
        HidePickupPrompt();
    }

    public void ShowPickupPrompt(string itemName)
    {
        if (pickupPrompt == null || pickupText == null) return;
        pickupText.text = $"Take it.... Press E  |  {itemName}";
        pickupPrompt.style.display = DisplayStyle.Flex;
        pickupPrompt.RemoveFromClassList("pickup-prompt-hidden");
        pickupPrompt.AddToClassList("pickup-prompt-visible");
    }

    public void HidePickupPrompt()
    {
        if (pickupPrompt == null) return;
        pickupPrompt.RemoveFromClassList("pickup-prompt-visible");
        pickupPrompt.AddToClassList("pickup-prompt-hidden");
        pickupPrompt.style.display = DisplayStyle.None;
    }

    public void UpdateStaminaBar(float percent)
    {
        if (staminaBarFill == null) return;

        staminaBarFill.style.width = Length.Percent(percent * 100);

        if (percent > 0.5f)
            staminaBarFill.style.unityBackgroundImageTintColor = new Color(0f, 1f, 0.3f);
        else if (percent > 0.25f)
            staminaBarFill.style.unityBackgroundImageTintColor = new Color(1f, 0.8f, 0f);
        else
            staminaBarFill.style.unityBackgroundImageTintColor = new Color(1f, 0.1f, 0.1f);
    }
}