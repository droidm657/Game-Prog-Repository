using UnityEngine;
using System.Collections;

public class FlashlightSystem : MonoBehaviour
{
    [Header("Flashlight")]
    public GameObject flashlightObject;

    public Light flashlightLight;

    private bool isOn = true;

    private bool isBroken = false;

    [Header("Battery")]
    public float maxBattery = 100f;

    public float currentBattery;

    public float batteryDrainSpeed = 5f;

    public float lowBatteryThreshold = 20f;

    [Header("Flicker")]
    public bool enableFlicker = true;

    public float minIntensity = 5f;
    public float maxIntensity = 7f;

    public float flickerSpeed = 0.05f;

    [Header("Random Shutdown")]
    public bool enableRandomShutdowns = true;

    public float shutdownChance = 0.002f;

    public float shutdownDuration = 1.5f;

    [Header("Startup Delay")]
    public float startupDelay = 0.3f;

    void Start()
    {
        currentBattery = maxBattery;

        SetFlashlight(isOn);

        if (enableFlicker)
        {
            StartCoroutine(FlickerEffect());
        }
    }

    void Update()
    {
        HandleInput();

        DrainBattery();

        RandomShutdown();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (currentBattery > 0 && !isBroken)
            {
                if (isOn)
                {
                    isOn = false;

                    SetFlashlight(false);
                }
                else
                {
                    StartCoroutine(TurnOnWithDelay());
                }
            }
        }
    }

    IEnumerator TurnOnWithDelay()
    {
        yield return new WaitForSeconds(startupDelay);

        isOn = true;

        SetFlashlight(true);
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

            // Battery dead
            if (currentBattery <= 0)
            {
                currentBattery = 0;

                isOn = false;

                SetFlashlight(false);
            }
        }
    }

    void RandomShutdown()
    {
        if (enableRandomShutdowns &&
           isOn &&
           currentBattery < lowBatteryThreshold)
        {
            if (Random.value < shutdownChance)
            {
                StartCoroutine(TemporaryShutdown());
            }
        }
    }

    IEnumerator TemporaryShutdown()
    {
        isBroken = true;

        SetFlashlight(false);

        yield return new WaitForSeconds(
            shutdownDuration
        );

        if (currentBattery > 0)
        {
            SetFlashlight(true);
        }

        isBroken = false;
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
                // Stronger flicker on low battery
                if (currentBattery < lowBatteryThreshold)
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