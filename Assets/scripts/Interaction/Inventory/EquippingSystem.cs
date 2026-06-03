using UnityEngine;
using Debug = UnityEngine.Debug;

public class EquippingSystem : MonoBehaviour
{
    public static EquippingSystem Instance;

    [Header("Hand Transform")]
    public Transform handTransform;

    private GameObject currentEquippedObject;
    private ItemData currentEquippedItem;

    public GameObject GetEquippedObject() => currentEquippedObject;

    void Awake()
    {
        // Force the active player instance to always be THIS local script instance
        Instance = this;
    }

    public void EquipItem(ItemData item)
    {
        if (!item.isEquippable)
        {
            Debug.Log($"{item.itemName} is not equippable!");
            return;
        }

        if (currentEquippedObject != null)
            UnequipCurrent();

        if (item.objectPrefab != null)
        {
            currentEquippedObject = Instantiate(item.objectPrefab, handTransform);
            currentEquippedObject.transform.localPosition = item.equipPositionOffset;
            currentEquippedObject.transform.localRotation = Quaternion.Euler(item.equipRotationOffset);

            foreach (Collider col in currentEquippedObject.GetComponentsInChildren<Collider>())
                col.enabled = false;

            currentEquippedItem = item;
            Debug.Log($"Equipped: {item.itemName}");
        }

        // Note: Make sure ShootingSystem.Instance also updates its Awake() like this script 
        // so it doesn't return null errors when changing scenes!
        if (item.itemType == ItemData.ItemType.Weapon)
        {
            if (ShootingSystem.Instance != null && AmmoSystem.Instance != null)
            {
                UIManager.Instance?.UpdateAmmoDisplay(
                    ShootingSystem.Instance.GetCurrentChamber(),
                    ShootingSystem.Instance.GetCurrentChamber(),
                    AmmoSystem.Instance.GetAmmo());
            }
        }
    }

    public void UnequipCurrent()
    {
        if (currentEquippedObject != null)
        {
            Destroy(currentEquippedObject);
            currentEquippedObject = null;
            Debug.Log($"Unequipped: {currentEquippedItem.itemName}");
            currentEquippedItem = null;
        }
        UIManager.Instance?.HideAmmo();
    }

    public ItemData GetEquippedItem() => currentEquippedItem;
    public bool HasItemEquipped() => currentEquippedObject != null;
}
