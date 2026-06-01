using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;
using Application = UnityEngine.Application;
using Debug = UnityEngine.Debug;
using Label = UnityEngine.UIElements.Label;


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

        int totalSlots = InventorySystem.Instance.maxInventorySize;
        List<ItemData> items = InventorySystem.Instance.items;

        for (int i = 0; i < totalSlots; i++)
        {
            var slot = new VisualElement();
            slot.AddToClassList("inventory-slot");

            if (i < items.Count)
            {
                ItemData item = items[i];
                // Filled slot
                var icon = new VisualElement();
                icon.AddToClassList("inventory-icon");
                if (items[i].itemIcon != null)
                    icon.style.backgroundImage = new StyleBackground(items[i].itemIcon);

                var nameLabel = new UnityEngine.UIElements.Label(items[i].itemName);
                nameLabel.AddToClassList("inventory-label");

                slot.Add(icon);
                slot.Add(nameLabel);

                if (EquippingSystem.Instance.GetEquippedItem() == item)
                    slot.AddToClassList("inventory-slot-equipped");

                // Click to equip
                slot.RegisterCallback<ClickEvent>(evt =>
                {
                    if (item.isEquippable)
                    {
                        if (EquippingSystem.Instance.GetEquippedItem() == item)
                            EquippingSystem.Instance.UnequipCurrent();
                        else
                            EquippingSystem.Instance.EquipItem(item);

                        RefreshInventory();
                    }
                    else
                    {
                        Debug.Log($"{item.itemName} cannot be equipped!");
                    }
                });
            }
            else
            {
                // Empty slot 
                slot.AddToClassList("inventory-slot-empty");
            }

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