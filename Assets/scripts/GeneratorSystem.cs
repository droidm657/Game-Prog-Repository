using UnityEngine;

public class GeneratorSystem : MonoBehaviour
{
    private bool playerInside;
    private bool activated;

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            if (!GameManager.Instance.generatorFound)
            {
                GameManager.Instance.FoundGenerator();
            }

            if (GameManager.Instance.HasEnoughFuel() &&
               !activated)
            {
                activated = true;

                GameManager.Instance.ActivateGenerator();

                Debug.Log("Generator Fueled!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            if (!GameManager.Instance.generatorFound)
            {
                InteractionUI.instance.ShowInteraction(
                    "Press E to Inspect Generator");
            }
            else if (!GameManager.Instance.HasEnoughFuel())
            {
                InteractionUI.instance.ShowInteraction(
                    "Find More Fuel");
            }
            else
            {
                InteractionUI.instance.ShowInteraction(
                    "Press E to Fuel Generator");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            InteractionUI.instance.HideInteraction();
        }
    }
}