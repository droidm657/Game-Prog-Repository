using System;
using System.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class MainMenuLightning : MonoBehaviour
{
    private Light lightningLight;

    [Header("Timing (Seconds)")]
    public float minTimeBetweenStrikes = 6f;
    public float maxTimeBetweenStrikes = 18f;
    public float flashDuration = 0.08f;

    [Header("Intensity")]
    public float maxIntensity = 3.5f;

    void Start()
    {
        lightningLight = GetComponent<Light>();
        lightningLight.enabled = false; // Keep it off initially
        StartCoroutine(LightningStormRoutine());
    }

    IEnumerator LightningStormRoutine()
    {
        while (true)
        {
            // Wait for a random interval before the next strike
            float currentWait = Random.Range(minTimeBetweenStrikes, maxTimeBetweenStrikes);
            yield return new WaitForSeconds(currentWait);

            // Execute a realistic double-strike flash pattern
            yield return StartCoroutine(TriggerLightningFlash());
        }
    }

    IEnumerator TriggerLightningFlash()
    {
        // Flash 1: Initial quick strike
        lightningLight.enabled = true;
        lightningLight.intensity = maxIntensity * 0.6f;
        yield return new WaitForSeconds(flashDuration);
        lightningLight.enabled = false;

        // Brief dark pause between the double-strike
        yield return new WaitForSeconds(Random.Range(0.05f, 0.12f));

        // Flash 2: Main brighter strike
        lightningLight.enabled = true;
        lightningLight.intensity = maxIntensity;
        yield return new WaitForSeconds(flashDuration * 1.5f);

        // Smoothly fade out the residual atmospheric glow
        float elapsed = 0f;
        float fadeDuration = 0.25f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            lightningLight.intensity = Mathf.Lerp(maxIntensity, 0f, elapsed / fadeDuration);
            yield return null;
        }

        lightningLight.enabled = false;
    }
}
