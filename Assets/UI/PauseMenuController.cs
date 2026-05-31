using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;
using Application = UnityEngine.Application;
using Label = UnityEngine.UIElements.Label;
using Debug = UnityEngine.Debug;


public class PauseMenuController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement menuContainer;
    private VisualElement inventoryGrid;
    private bool isMenuOpen = false;

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
    }
    void Start()
    {
        var root = uiDocument.rootVisualElement;
        menuContainer = root.Q<VisualElement>("menu-container");
        inventoryGrid = root.Q<VisualElement>("inventory-grid");

        var resumeBtn = root.Q<Button>("resume-button");
        var quitBtn = root.Q<Button>("quit-button");

        Debug.Log(resumeBtn != null ? " Resume found" : "Resume NOT found");
        Debug.Log(quitBtn != null ? " Quit found" : " Quit NOT found");

        if (resumeBtn != null) resumeBtn.clicked += () => ResumeGame();
        if (quitBtn != null) quitBtn.clicked += () => QuitGame();

        menuContainer.style.display = DisplayStyle.None;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleMenu();
    }

    void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        menuContainer.style.display = isMenuOpen
            ? DisplayStyle.Flex
            : DisplayStyle.None;
        Time.timeScale = isMenuOpen ? 0f : 1f;
        UnityEngine.Cursor.lockState = isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
        UnityEngine.Cursor.visible = isMenuOpen;
        if (isMenuOpen)
            RefreshInventory();
    }

    void RefreshInventory()
    {
        inventoryGrid.Clear();
        foreach (ItemData itemData in InventorySystem.Instance.items)
        {
            var slot = new VisualElement();
            slot.AddToClassList("inventory-slot");
            var icon = new VisualElement();
            icon.AddToClassList("inventory-icon");
            if (itemData.itemIcon != null)
                icon.style.backgroundImage = new StyleBackground(itemData.itemIcon);
            var nameLabel = new Label(itemData.itemName);
            nameLabel.AddToClassList("inventory-label");
            slot.Add(icon);
            slot.Add(nameLabel);
            inventoryGrid.Add(slot);
        }
    }

    void ResumeGame()  
    {
        ToggleMenu();
    }

    void QuitGame()   
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}