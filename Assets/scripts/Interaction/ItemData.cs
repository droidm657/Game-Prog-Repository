using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public int itemID;
    public string itemName;
    public Sprite itemIcon;
    public GameObject objectPrefab;
    public ItemType itemType;

    [Header("Equip Settings")]
    public bool isEquippable;
    public Vector3 equipPositionOffset;
    public Vector3 equipRotationOffset;

    [Header("Ammo Settings")]
    public bool isAmmo;
    public int ammoCount;

    [Header("Consumable Settings")]
    public bool isConsumable;


    public enum ItemType
    {
        Weapon,
        Ammo,
        KeyItem, 
        Consumable, 
        Miscellaneous 
    }
    

}
