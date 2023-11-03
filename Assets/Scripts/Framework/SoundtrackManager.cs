using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundtrackManager : Service
{
    [SerializeField]
    private AudioMixer _audioMixer;

    [SerializeField]
    private AudioSource _voAudioSource;

    [SerializeField]
    private AudioSource _passByAudioSource;
    private float _passByAudioSourceStandardPitch;

    [SerializeField]
    private AudioSource[] _sfxAudioSources;

    private AudioSource currentSource;
    private AudioSource nextSource;

    public float fadeTime = 1f; // Time in seconds to fade in/out

    public AudioMixer AudioMixer => _audioMixer;

    protected override void Awake()
    {
        base.Awake();
        if (_isDestroyed) return;
        _passByAudioSourceStandardPitch = _passByAudioSource.pitch;
        // Initialize Audio Sources
        currentSource = gameObject.GetComponent<AudioSource>();
        nextSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayClip(AudioClip clip)
    {
        PlayClip(clip, false);
    }

    public IEnumerator PlayClipAfterCurrent(AudioClip clip, bool fadeIn = false, bool fadeOutCurrent = false, bool loop = false, float targetVolume = 1f)
    {
        currentSource.loop = false;
        yield return new WaitUntil(() => currentSource.time >= currentSource.clip.length - 0.03f || currentSource.isPlaying == false);

        PlayClip(clip, fadeIn, fadeOutCurrent, loop, targetVolume);
    }

    public void PlayClip(AudioClip clip, bool fadeIn = false, bool fadeOutCurrent = false, bool loop = false, float targetVolume = 1f, bool letCurrentClipFinish = false)
    {
        if (fadeOutCurrent)
        {
            StartCoroutine(FadeOutAndPlayNext(clip, fadeIn, loop, targetVolume));
        }
        else if (letCurrentClipFinish)
        {
            StartCoroutine(PlayClipAfterCurrent(clip, fadeIn, fadeOutCurrent: false, loop, targetVolume));
        }
        else
        {
            if (currentSource.isPlaying)
                currentSource.Stop();
            currentSource.clip = clip;
            currentSource.loop = loop;
            if (fadeIn)
                StartCoroutine(FadeIn(currentSource, targetVolume));
            else
            {
                currentSource.volume = targetVolume;
                currentSource.Play();
            }
        }
    }

    IEnumerator FadeOutAndPlayNext(AudioClip clip, bool fadeIn, bool loop, float targetVolume)
    {
        yield return StartCoroutine(FadeOut(currentSource));
        currentSource.clip = clip;
        currentSource.loop = loop;
        if (fadeIn)
            StartCoroutine(FadeIn(currentSource, targetVolume));
        else
        {
            currentSource.volume = targetVolume;
            currentSource.Play();
        }
    }

    public void PlayWithCrossFade(AudioClip clip, bool loop = false, float targetVolume = 1f)
    {
        StartCoroutine(CrossFade(clip, loop, targetVolume));
    }

    IEnumerator CrossFade(AudioClip newClip, bool loop, float targetVolume)
    {
        nextSource.clip = newClip;
        nextSource.loop = loop;
        nextSource.Play();
        nextSource.volume = 0f; // Start the nextSource at 0 volume.

        float startTime = Time.time;
        while (Time.time < startTime + fadeTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            currentSource.volume = 1f - t;
            nextSource.volume = t * targetVolume;
            yield return null;
        }

        // Swap the sources and stop the old one
        AudioSource temp = currentSource;
        currentSource = nextSource;
        nextSource = temp;
        nextSource.Stop();
    }

    public void FadeOutCurrent()
    {
        StartCoroutine(FadeOut(currentSource));
    }

    public void FadeOutCurrent(float fadeOutTime)
    {
        StartCoroutine(FadeOut(currentSource, fadeOutTime));
    }

    IEnumerator FadeOut(AudioSource source, float fadeOutTime = 1.0f)
    {
        float startVolume = source.volume;
        float startTime = Time.time;
        while (Time.time < startTime + fadeOutTime)
        {
            float t = (Time.time - startTime) / fadeOutTime;
            source.volume = startVolume * (1f - t);
            yield return null;
        }

        source.Stop();
    }

    public void FadeInCurrent(float targetVolume = 1f)
    {
        StartCoroutine(FadeIn(currentSource, targetVolume));
    }

    IEnumerator FadeIn(AudioSource source, float targetVolume)
    {
        source.Play();
        float startTime = Time.time;
        while (Time.time < startTime + fadeTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            source.volume = t * targetVolume;
            yield return null;
        }
    }

    public void PlayClipAt3DLocation(Vector3 pos, PassByAudioSource.AudioSourceSetting setting)
    {
        if (_passByAudioSource.isPlaying) return;

        _passByAudioSource.transform.position = pos;

        float randomPitch = _passByAudioSourceStandardPitch + Random.Range(-0.15f, 0.15f);
        _passByAudioSource.pitch = randomPitch;
        //_passByAudioSource.minDistance = setting.MinMaxDistance.x;
        //_passByAudioSource.maxDistance = setting.MinMaxDistance.y;
        //_passByAudioSource.loop = setting.IsLooping;

        _passByAudioSource.Play();
    }

    public void PlaySFX(AudioClip clip, bool forcePlay, float volume = 1.0f, float pitch = 1.0f)
    {
        AudioSource freeAudioSource = null;
        // find free sfx source
        for (int i = 0; i < _sfxAudioSources.Length; i++)
        {
            if (_sfxAudioSources[i].isPlaying == false)
                freeAudioSource = _sfxAudioSources[i];
        }

        if (freeAudioSource == null && forcePlay == false)
        {
            Debug.Log("Had no free sfx audioSource, aborting call");
            return;
        }
        else if (forcePlay)
        {
            freeAudioSource = _sfxAudioSources[0];
        }

        freeAudioSource.clip = clip;
        freeAudioSource.volume = volume;
        freeAudioSource.pitch = pitch;
        freeAudioSource.Play();
    }
}
