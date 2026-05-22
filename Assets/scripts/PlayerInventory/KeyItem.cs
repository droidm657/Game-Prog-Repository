using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [Header("Key")]
    public string keyID;

    private bool playerNearby = false;

    void Update()
    {
        if (playerNearby &&
            Input.GetKeyDown(KeyCode.E))
        {
            PickupKey();
        }
    }

    void PickupKey()
    {
        PlayerInventory inventory =
            FindObjectOfType<PlayerInventory>();

        inventory.AddKey(keyID);

        InteractionUI.instance.HideInteraction();

        Debug.Log(keyID + " picked up");

        // Destroy parent object
        Destroy(transform.parent.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            InteractionUI.instance.ShowInteraction(
                "Press E to pick up"
            );
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            InteractionUI.instance.HideInteraction();
        }
    }
}