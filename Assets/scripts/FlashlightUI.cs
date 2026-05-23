using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlashlightUI : MonoBehaviour
{
    public FlashlightSystem flashlightSystem;

    [Header("UI")]
    public Image batteryFill;

    public TMP_Text batteryText;

    void Update()
    {
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