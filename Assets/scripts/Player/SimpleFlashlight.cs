using UnityEngine;
using System.Collections;

public class SimpleFlashlight : MonoBehaviour
{
    // Monster AI access
    public static bool FlashlightOn = false;
    public static Transform FlashlightTransform;

    [Header("References")]
    public GameObject flashlightObject;
    public Light flashlightLight;

    [Header("Controls")]
    public KeyCode toggleKey = KeyCode.F;
    public KeyCode burstKey = KeyCode.Q;

    [Header("Flicker Settings")]
    public bool enableFlicker = true;

    public float normalIntensity = 7f;
    public float weakIntensity = 2f;

    [Range(0, 100)]
    public float blackoutChance = 10f;

    [Header("Light Burst")]
    public float burstIntensity = 150f;
    public float burstDuration = 0.25f;
    public float burstCooldown = 10f;

    [Header("Monster Stun")]
    public float stunRange = 8f;
    public float stunDuration = 3f;

    private bool isOn = false;
    private bool canBurst = true;

    void Start()
    {
        SetFlashlight(false);

        if (enableFlicker)
        {
            StartCoroutine(FlickerRoutine());
        }
    }

    void Update()
    {
        // Important for inventory-equipped clones
        if (flashlightLight != null)
        {
            FlashlightTransform = flashlightLight.transform;
        }

        FlashlightOn = isOn;

        // Toggle flashlight
        if (Input.GetKeyDown(toggleKey) && canBurst)
        {
            isOn = !isOn;
            SetFlashlight(isOn);
        }

        // Light burst
        if (Input.GetKeyDown(burstKey))
        {
            if (isOn && canBurst)
            {
                StartCoroutine(LightBurst());
            }
        }
    }

    void SetFlashlight(bool state)
    {
        if (flashlightObject != null)
            flashlightObject.SetActive(state);

        if (flashlightLight != null)
            flashlightLight.enabled = state;
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            if (isOn &&
                flashlightLight != null &&
                canBurst)
            {
                flashlightLight.intensity =
                    Random.Range(
                        weakIntensity,
                        normalIntensity
                    );

                if (Random.Range(0f, 100f)
                    <= blackoutChance)
                {
                    flashlightLight.enabled = false;

                    yield return new WaitForSeconds(
                        Random.Range(
                            0.05f,
                            0.25f
                        )
                    );

                    if (isOn)
                    {
                        flashlightLight.enabled = true;
                    }
                }
            }

            yield return new WaitForSeconds(
                Random.Range(
                    0.05f,
                    0.15f
                )
            );
        }
    }

    IEnumerator LightBurst()
    {
        canBurst = false;

        flashlightLight.enabled = true;
        flashlightLight.intensity = burstIntensity;

        // Stun nearby monsters
        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                stunRange
            );

        foreach (Collider hit in hits)
        {
            BasicMonsterAI monster =
                hit.GetComponent<BasicMonsterAI>();

            if (monster != null)
            {
                monster.StunMonster(
                    stunDuration
                );
            }
        }

        yield return new WaitForSeconds(
            burstDuration
        );

        // Burn out flashlight
        flashlightLight.enabled = false;

        isOn = false;
        FlashlightOn = false;

        yield return new WaitForSeconds(
            burstCooldown
        );

        flashlightLight.intensity =
            normalIntensity;

        flashlightLight.enabled = true;

        isOn = true;
        FlashlightOn = true;

        canBurst = true;
    }
}