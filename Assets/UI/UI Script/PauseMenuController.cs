using System.Collections.Generic;
using System.Reflection.Emit;
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

        Debug.Log(resumeBtn != null ? "Resume button found" : "Resume button NOT found");
        Debug.Log(quitBtn != null ? "Quit button found" : "Quit button NOT found");

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

    public void RefreshInventory()
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

                // Filled slot visuals
                var icon = new VisualElement();
                icon.AddToClassList("inventory-icon");
                if (item.itemIcon != null)
                    icon.style.backgroundImage = new StyleBackground(item.itemIcon);

                var nameLabel = new Label(item.itemName);
                nameLabel.AddToClassList("inventory-label");

                slot.Add(icon);
                slot.Add(nameLabel);

                // Highlight if weapon is equipped
                if (EquippingSystem.Instance != null && EquippingSystem.Instance.GetEquippedItem() == item)
                    slot.AddToClassList("inventory-slot-equipped");

                // --- INTEGRATED MULTI-TYPE INTERACTION CALLBACK ---
                slot.RegisterCallback<ClickEvent>(evt =>
                {
                    if (item.isEquippable)
                    {
                        if (EquippingSystem.Instance != null)
                        {
                            if (EquippingSystem.Instance.GetEquippedItem() == item)
                                EquippingSystem.Instance.UnequipCurrent();
                            else
                                EquippingSystem.Instance.EquipItem(item);
                        }

                        // Redraw UI to show updated borders
                        RefreshInventory();
                    }
                    else if (item.isConsumable)
                    {
                        Debug.Log($"[UI Click] Clicked consumable item: {item.itemName}. Sending request to InventorySystem.");

                        // Fired directly into your safe validation system!
                        InventorySystem.Instance.UseConsumableItem(item);

                        // Redraw inventory layout slots immediately so consumed item disappears
                        RefreshInventory();
                    }
                    else
                    {
                        Debug.Log($"[UI Click] {item.itemName} is neither equippable nor consumable (e.g. Progression Key).");
                    }
                });
            }
            else
            {
                // Empty slot visuals
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