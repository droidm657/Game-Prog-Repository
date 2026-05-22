using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    public HorrorFPSController playerController;

    public Image staminaFill;

    void Update()
    {
        staminaFill.fillAmount =
            playerController.currentStamina /
            playerController.maxStamina;
    }
}