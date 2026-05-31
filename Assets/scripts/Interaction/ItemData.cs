using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public int itemID;
    public string itemName;
    public Sprite itemIcon;
    public GameObject objectPrefab;
    public ItemType itemType;

    public enum ItemType
    {
        Weapon,
        Ammo,
        KeyItem, 
        Consumable, 
        Miscellaneous 
    }
    

}
