using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance=>instance;

    [SerializeField] private AudioSource sourceMusica;
    [SerializeField] private AudioSource sourceSFX;

    [SerializeField] private float fadeDuration = 1f;
    private Coroutine currentFade;

    private Coroutine loopCoroutine;
    private float loopStartTime = 0f;
    private bool customLoop = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip, float volumen=1f)
    {
        sourceSFX.PlayOneShot(clip,volumen);
    }

    public void SetClip(AudioClip clip)
    {
        if (loopCoroutine != null)
            StopCoroutine(loopCoroutine);

        customLoop = false;

        sourceMusica.clip = clip;
        sourceMusica.loop = true;
        sourceMusica.volume = 0.75f;
        sourceMusica.Stop();
    }

    private IEnumerator LoopDesdeTiempo()
    {
        while (customLoop && sourceMusica.clip != null)
        {
            if (sourceMusica.time >= sourceMusica.clip.length - 0.1f)
            {
                sourceMusica.time = loopStartTime;
            }
            yield return null;
        }
    }

    public void PlayMusicaConLoop(AudioClip clip, float loopStart, float volumen = 0.75f)
    {
        // (sourceMusica.clip == clip) return;

        if (loopCoroutine != null)
            StopCoroutine(loopCoroutine);

        sourceMusica.clip = clip;
        sourceMusica.time = 0f;
        sourceMusica.loop = false;

        sourceMusica.volume = 1f;
        sourceMusica.Play();

        loopStartTime = loopStart;
        customLoop = true;

        loopCoroutine = StartCoroutine(LoopDesdeTiempo());
    }

    public void PlayMusica(AudioClip clip)
    {
        if (sourceMusica.clip == clip) return;

        sourceMusica.clip = clip;
        sourceMusica.loop = true;  
        sourceMusica.volume = 0.75f;
        sourceMusica.Play();
    }

    public void PausarMusica()
    {
        StartFade(0f, true);
    }

    public void DespausarMusica()
    {
        sourceMusica.Play(); // importante: arrancar antes del fade
        StartFade(0.75f, false);
    }

    public void StopMusica()
    {
        StartFade(0f, false, true);
    }


    private void StartFade(float targetVolume, bool pauseAfter, bool stopAfter = false)
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeVolume(targetVolume, pauseAfter, stopAfter));
    }

    private IEnumerator FadeVolume(float targetVolume, bool pauseAfter, bool stopAfter)
    {
        float startVolume = sourceMusica.volume;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            sourceMusica.volume = Mathf.Lerp(startVolume, targetVolume, time / fadeDuration);
            yield return null;
        }

        sourceMusica.volume = targetVolume;

        if (pauseAfter)
            sourceMusica.Pause();

        if (stopAfter)
            sourceMusica.Stop();
    }
}
