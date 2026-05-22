using UnityEngine;

public class FuelCollectible : MonoBehaviour
{
    private bool playerNear;

    void Update()
    {
        if (playerNear &&
           Input.GetKeyDown(KeyCode.E))
        {
            if (GameManager.Instance.generatorFound)
            {
                GameManager.Instance.CollectFuel();

                Destroy(transform.parent.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;

            if (GameManager.Instance.generatorFound)
            {
                InteractionUI.instance.ShowInteraction(
                    "Press E to Collect Fuel");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;

            InteractionUI.instance.HideInteraction();
        }
    }
}