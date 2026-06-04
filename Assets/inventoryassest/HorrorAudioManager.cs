using UnityEngine;
using System.Collections;

public class HorrorAudioManager : MonoBehaviour
{
    public static HorrorAudioManager Instance;

    public AudioSource ambienceSource;
    public AudioSource chaseSource;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        Instance = this;
    }

    public void StartChaseMusic()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        if (!chaseSource.isPlaying)
            chaseSource.Play();

        fadeRoutine =
            StartCoroutine(
                FadeToChase()
            );
    }

    public void StopChaseMusic()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine =
            StartCoroutine(
                FadeToAmbience()
            );
    }

    IEnumerator FadeToChase()
    {
        float t = 0;

        while (t < 2f)
        {
            t += Time.deltaTime;

            ambienceSource.volume =
                Mathf.Lerp(0.5f, 0f, t / 2f);

            chaseSource.volume =
                Mathf.Lerp(0f, 1f, t / 2f);

            yield return null;
        }
    }

    IEnumerator FadeToAmbience()
    {
        float t = 0;

        while (t < 2f)
        {
            t += Time.deltaTime;

            ambienceSource.volume =
                Mathf.Lerp(0f, 0.5f, t / 2f);

            chaseSource.volume =
                Mathf.Lerp(1f, 0f, t / 2f);

            yield return null;
        }

        chaseSource.Stop();
    }
}