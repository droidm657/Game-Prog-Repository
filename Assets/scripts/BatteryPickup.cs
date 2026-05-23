using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    public float batteryAmount = 25f;

    private bool playerNear;

    public FlashlightSystem flashlightSystem;

    void Update()
    {
        if (playerNear &&
           Input.GetKeyDown(KeyCode.E))
        {
            flashlightSystem.currentBattery +=
                batteryAmount;

            flashlightSystem.currentBattery =
                Mathf.Clamp(
                    flashlightSystem.currentBattery,
                    0,
                    flashlightSystem.maxBattery
                );

            Destroy(transform.parent.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;

            InteractionUI.instance.ShowInteraction(
                "Press E to Pick Up Battery");
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