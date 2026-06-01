using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ItemPickUp : MonoBehaviour
{
    [Header("Item Info")]
    public ItemData itemData;

    [Header("Pick Up Settings")]
    public float pickUpRange = 2f;
    public KeyCode pickUpKey = KeyCode.E;

    private bool isPlayerInRange = false;
    private Transform playerTransform;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(pickUpKey))
        {
            GateInteraction gate = GetComponent<GateInteraction>();
            if (gate != null)
            {
                gate.TryOpen(playerTransform);
                return;
            }

            PickUpItem();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerTransform = other.transform;
            UIManager.Instance.ShowPickupPrompt(itemData.itemName);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerTransform = null;
            UIManager.Instance.HidePickupPrompt();
        }
    }

    void PickUpItem()
    {
        InventorySystem.Instance.AddItem(itemData);
        Debug.Log($"Picked up {itemData.itemName}");
        UIManager.Instance.HidePickupPrompt();
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickUpRange);
    }

}