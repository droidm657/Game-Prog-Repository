using System.Diagnostics;
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
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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
    }

    public ItemData GetEquippedItem() => currentEquippedItem;
    public bool HasItemEquipped() => currentEquippedObject != null;
}
