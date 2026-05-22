using UnityEngine;

public class GeneratorSwitch : MonoBehaviour
{
    private bool playerInside;

    [Header("Lights")]
    public Light[] lightsToTurnOn;

    private bool switchedOn = false;

    void Update()
    {
        if (playerInside &&
           Input.GetKeyDown(KeyCode.E) &&
           !switchedOn)
        {
            if (GameManager.Instance.generatorPowered)
            {
                switchedOn = true;

                TurnOnLights();

                GameManager.Instance.ActivateSwitch();

                InteractionUI.instance.HideInteraction();

                Debug.Log("Lights ON");
            }
        }
    }

    void TurnOnLights()
    {
        foreach (Light lightObject in lightsToTurnOn)
        {
            lightObject.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            if (GameManager.Instance.generatorPowered)
            {
                InteractionUI.instance.ShowInteraction(
                    "Press E to Turn On Switch");
            }
            else
            {
                InteractionUI.instance.ShowInteraction(
                    "Generator Has No Power");
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