using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlashlightUI : MonoBehaviour
{
    public GameObject flashlightUI;

    public Image batteryFill;
    public TMP_Text batteryText;

    private FlashlightSystem flashlightSystem;

    void Update()
    {
        flashlightSystem =
            FindFirstObjectByType<FlashlightSystem>();

        if (flashlightSystem == null)
        {
            flashlightUI.SetActive(false);
            return;
        }

        flashlightUI.SetActive(true);

        float batteryPercent =
            flashlightSystem.currentBattery /
            flashlightSystem.maxBattery;

        batteryFill.fillAmount =
            batteryPercent;

        batteryText.text =
            Mathf.RoundToInt(
                flashlightSystem.currentBattery
            ) + "%";
    }
}