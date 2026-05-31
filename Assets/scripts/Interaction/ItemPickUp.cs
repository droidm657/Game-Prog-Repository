using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [Header("Item Info")]
    public ItemData itemData;

    [Header("Pick Up Settings")]
    public float pickUpRange = 2f;
    public KeyCode pickUpKey = KeyCode.E;

    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(pickUpKey))
        {
            PickUpItem();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log($"Press E to Pick Up {itemData.itemName}");
            // show ui prompt to pick up item here
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // hide ui prompt to pick up item here
        }
    }

    void PickUpItem()
    {
        // Add item to player's inventory
        InventorySystem.Instance.AddItem(itemData);
        Debug.Log($"Picked up {itemData.itemName}");
        // Destroy the item in the world
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickUpRange);
    }

}
