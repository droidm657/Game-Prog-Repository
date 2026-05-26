using UnityEngine;
using System.Collections;

public class FlashlightSystem : MonoBehaviour
{
    [Header("FLASHLIGHT REFERENCES")]
    public GameObject flashlightObject;

    public Light flashlightLight;

    [Header("FLASHLIGHT SETTINGS")]
    public KeyCode toggleKey = KeyCode.F;

    public bool startEnabled = true;

    private bool isOn = true;

    private bool isBroken = false;

    [Header("BATTERY SETTINGS")]
    [Range(0, 100)]
    public float maxBattery = 100f;

    [Range(0, 100)]
    public float currentBattery = 100f;

    public float batteryDrainSpeed = 5f;

    public float lowBatteryThreshold = 20f;

    [Header("FLICKER SETTINGS")]
    public bool enableFlicker = true;

    public float minIntensity = 5f;

    public float maxIntensity = 7f;

    public float flickerSpeed = 0.05f;

    [Header("FLASH BURST SETTINGS")]
    public bool enableFlashBurst = true;

    public float requiredBatteryPercent = 50f;

    public float batteryDrainFromBurst = 25f;

    public float flashBurstIntensity = 300f;

    public float flashBurstDuration = 1.5f;

    public float flashFadeDuration = 1f;

    public float flashRange = 8f;

    public float stunDuration = 3f;

    public float flashCooldown = 10f;

    [Header("DEBUG")]
    public bool canFlash = true;

    private float normalIntensity;

    void Start()
    {
        isOn = startEnabled;

        normalIntensity =
            flashlightLight.intensity;

        SetFlashlight(isOn);

        if (enableFlicker)
        {
            StartCoroutine(
                FlickerEffect()
            );
        }
    }

    void Update()
    {
        HandleInput();

        DrainBattery();
    }

    void HandleInput()
    {
        // TOGGLE FLASHLIGHT
        if (Input.GetKeyDown(toggleKey))
        {
            if (currentBattery > 0 &&
               !isBroken)
            {
                isOn = !isOn;

                SetFlashlight(isOn);
            }
        }

        // LEFT CLICK FLASH BURST
        if (Input.GetMouseButtonDown(0))
        {
            if (!enableFlashBurst)
                return;

            // Not enough battery
            if (currentBattery <
               requiredBatteryPercent)
            {
                Debug.Log(
                    "Battery Too Low"
                );

                return;
            }

            if (canFlash &&
               isOn &&
               !isBroken)
            {
                StartCoroutine(
                    FlashBurst()
                );
            }
        }
    }

    IEnumerator FlashBurst()
    {
        canFlash = false;

        // Drain battery amount
        currentBattery -=
            batteryDrainFromBurst;

        currentBattery =
            Mathf.Clamp(
                currentBattery,
                0,
                maxBattery
            );

        // Disable flashlight temporarily
        isBroken = true;

        // Save current intensity
        float previousIntensity =
            flashlightLight.intensity;

        // HUGE FLASH
        flashlightLight.intensity =
            flashBurstIntensity;

        // Detect nearby monsters
        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                flashRange
            );

        foreach (Collider hit in hits)
        {
            MonsterAI monster =
                hit.GetComponent<MonsterAI>();

            if (monster != null)
            {
                monster.StunMonster(
                    stunDuration
                );
            }
        }

        // HOLD FLASH
        yield return new WaitForSeconds(
            flashBurstDuration
        );

        // FADE FLASH
        float timer = 0f;

        while (timer < flashFadeDuration)
        {
            timer += Time.deltaTime;

            flashlightLight.intensity =
                Mathf.Lerp(
                    flashBurstIntensity,
                    previousIntensity,
                    timer / flashFadeDuration
                );

            yield return null;
        }

        flashlightLight.intensity =
            previousIntensity;

        // QUICK UNEQUIP
        flashlightObject.SetActive(false);

        flashlightLight.enabled = false;

        isOn = false;

        Debug.Log(
            "Flashlight Cooling Down..."
        );

        // COOLDOWN
        yield return new WaitForSeconds(
            flashCooldown
        );

        // FLASHLIGHT READY AGAIN
        isBroken = false;

        canFlash = true;

        Debug.Log(
            "Flashlight Ready"
        );
    }

    void DrainBattery()
    {
        if (isOn)
        {
            currentBattery -=
                batteryDrainSpeed *
                Time.deltaTime;

            currentBattery =
                Mathf.Clamp(
                    currentBattery,
                    0,
                    maxBattery
                );

            // DEAD BATTERY
            if (currentBattery <= 0)
            {
                currentBattery = 0;

                isOn = false;

                SetFlashlight(false);
            }
        }
    }

    void SetFlashlight(bool state)
    {
        flashlightObject.SetActive(state);

        flashlightLight.enabled = state;
    }

    IEnumerator FlickerEffect()
    {
        while (true)
        {
            if (isOn)
            {
                // LOW BATTERY FLICKER
                if (currentBattery <
                   lowBatteryThreshold)
                {
                    flashlightLight.intensity =
                        Random.Range(2f, 6f);
                }
                else
                {
                    flashlightLight.intensity =
                        Random.Range(
                            minIntensity,
                            maxIntensity
                        );
                }
            }

            yield return new WaitForSeconds(
                flickerSpeed
            );
        }
    }
}