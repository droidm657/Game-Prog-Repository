// UIManager.cs
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Label = UnityEngine.UIElements.Label;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private UIDocument uiDocument;
    private VisualElement pickupPrompt;
    private Label pickupText;

    private VisualElement staminaBarFill;
    private VisualElement staminaContainer;

    private VisualElement healthBarFill;
    private VisualElement healthContainer;

    private VisualElement ammoContainer;
    private UnityEngine.UIElements.Label ammoText;


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
            return; // Ensure only one instance exists
        }

        uiDocument = GetComponent<UIDocument>();
    }

    void OnEnabled  ()
    {
        // Listen for scene changes to re-query UI elements
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeUI();
    }

    void InitializeUI()
    {
        if (uiDocument == null) 
        { 
            uiDocument = FindAnyObjectByType<UIDocument>(); 
        }

        if (uiDocument == null || uiDocument.rootVisualElement == null) 
        { 
            Debug.LogError("UIManager: UIDocument or rootVisualElement not found!"); return; 
        }

        var root = uiDocument.rootVisualElement;
        pickupPrompt = root.Q<VisualElement>("pickup-prompt");
        pickupText = root.Q<Label>("pickup-text");
        staminaBarFill = root.Q<VisualElement>("stamina-bar-fill");
        staminaContainer = root.Q<VisualElement>("stamina-container");
        ammoContainer = root.Q<VisualElement>("ammo-container");
        ammoText = root.Q<UnityEngine.UIElements.Label>("ammo-text");

        // Hide at start
        HidePickupPrompt();
        HideAmmo();
    }

    public void UpdateHealthBar(float percent)
    {
        if (healthBarFill == null) return;

        percent = Mathf.Clamp01(percent);

       
        healthBarFill.style.width = Length.Percent(percent * 100);

        if (percent > 0.25f)
        {
            healthBarFill.style.unityBackgroundImageTintColor = new Color(1f, 0.2f, 0.2f);
        }
        else
        {
           
            healthBarFill.style.unityBackgroundImageTintColor = new Color(0.5f, 0f, 0.05f); 
        }
    }

    public void ShowPickupPrompt(string itemName)
    {
        if (pickupPrompt == null || pickupText == null) return;
        pickupText.text = $"Press E  |  {itemName}";
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

    public void UpdateAmmoDisplay(int current, int chamber, int reserve)
    {
        if (ammoText == null) return;

        ammoText.text = $"{chamber} / {reserve}";

        // Show only when weapon equipped
        ammoContainer.style.display = DisplayStyle.Flex;

        // Flash red when low
        if (chamber <= 1)
            ammoText.style.color = new Color(1f, 0.2f, 0.2f);
        else if (chamber <= 2)
            ammoText.style.color = new Color(1f, 0.7f, 0f);
        else
            ammoText.style.color = Color.white;
    }

    public void HideAmmo()
    {
        if (ammoContainer == null) return;
        ammoContainer.style.display = DisplayStyle.None;
    }

}