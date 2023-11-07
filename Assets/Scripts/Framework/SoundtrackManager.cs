using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

[DefaultExecutionOrder(-500)]
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

    private AudioSource currentSoundtrackAudioSource;
    private AudioSource nextSoundtrackAudioSource;

    private AudioSource[] _inSceneAudioSources = new AudioSource[] { };

    // key pair, with key the audiosource and its value its original volume
    private KeyValuePair<AudioSource, float>[] _allInSceneSFXAudioSources;

    private float _sfxVolume = 1.0f;
    private float _soundtrackVolume = 1.0f;

    public float fadeTime = 1f; // Time in seconds to fade in/out

    public AudioMixer AudioMixer => _audioMixer;

    protected override void Awake()
    {
        base.Awake();
        if (_isDestroyed) return;
        _passByAudioSourceStandardPitch = _passByAudioSource.pitch;
        // Initialize Audio Sources
        currentSoundtrackAudioSource = gameObject.GetComponent<AudioSource>();
        nextSoundtrackAudioSource = gameObject.AddComponent<AudioSource>();

    }

    private void Start()
    {
        Debug.Log("Start called from " + gameObject.name);
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(InitializeNextFrame());
    }

    private IEnumerator InitializeNextFrame()
    {
        // Wait until the next frame
        yield return null;

        // Now that we're in the next frame, run the initialization
        Initialize();
    }

    private void Initialize()
    {
        _inSceneAudioSources = new AudioSource[] { };
        _inSceneAudioSources = FindAllAudioSourcesIncludingDontDestroyOnLoad();
        List<AudioSource> allSFXAudioSources = new List<AudioSource>(_inSceneAudioSources);
        allSFXAudioSources.Remove(currentSoundtrackAudioSource);
        allSFXAudioSources.Remove(nextSoundtrackAudioSource);

        List<KeyValuePair<AudioSource, float>> audioSourceVolumePairs = new List<KeyValuePair<AudioSource, float>>();
        for (int i = 0; i < allSFXAudioSources.Count; i++)
        {
            var audioSource = allSFXAudioSources[i];
            KeyValuePair<AudioSource, float> keyValuePair = new KeyValuePair<AudioSource, float>(audioSource, audioSource.volume);
            audioSourceVolumePairs.Add(keyValuePair);
        }

        _allInSceneSFXAudioSources = new KeyValuePair<AudioSource, float>[] { };
        _allInSceneSFXAudioSources = audioSourceVolumePairs.ToArray();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine(InitializeNextFrame());
    }

    private AudioSource[] FindAllAudioSourcesIncludingDontDestroyOnLoad()
    {
        List<AudioSource> audioSources = new List<AudioSource>();

        // Find all audio sources in active scene
        var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var rootGameObject in rootGameObjects)
        {
            audioSources.AddRange(rootGameObject.GetComponentsInChildren<AudioSource>(true));
        }

        // Temporary object to access the DontDestroyOnLoad scene
        GameObject temp = new GameObject();
        DontDestroyOnLoad(temp);
        Scene dontDestroyOnLoadScene = temp.scene;
        Destroy(temp);

        // Now find all root GameObjects in the DontDestroyOnLoad scene
        var dontDestroyRootGameObjects = dontDestroyOnLoadScene.GetRootGameObjects();
        foreach (var rootGameObject in dontDestroyRootGameObjects)
        {
            audioSources.AddRange(rootGameObject.GetComponentsInChildren<AudioSource>(true));
        }

        audioSources = audioSources.Where(source => source != null && source.gameObject != null).ToList();
        return audioSources.ToArray();
    }

    public void PlayClip(AudioClip clip)
    {
        PlayClip(clip, false);
    }

    public IEnumerator PlayClipAfterCurrent(AudioClip clip, bool fadeIn = false, bool fadeOutCurrent = false, bool loop = false, float targetVolume = 1f)
    {
        currentSoundtrackAudioSource.loop = false;
        yield return new WaitUntil(() => currentSoundtrackAudioSource.time >= currentSoundtrackAudioSource.clip.length - 0.03f || currentSoundtrackAudioSource.isPlaying == false);

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
            if (currentSoundtrackAudioSource.isPlaying)
                currentSoundtrackAudioSource.Stop();
            currentSoundtrackAudioSource.clip = clip;
            currentSoundtrackAudioSource.loop = loop;
            if (fadeIn)
                StartCoroutine(FadeIn(currentSoundtrackAudioSource, targetVolume));
            else
            {
                currentSoundtrackAudioSource.volume = targetVolume * _soundtrackVolume;
                currentSoundtrackAudioSource.Play();
            }
        }
    }

    IEnumerator FadeOutAndPlayNext(AudioClip clip, bool fadeIn, bool loop, float targetVolume)
    {
        yield return StartCoroutine(FadeOut(currentSoundtrackAudioSource));
        currentSoundtrackAudioSource.clip = clip;
        currentSoundtrackAudioSource.loop = loop;
        if (fadeIn)
            StartCoroutine(FadeIn(currentSoundtrackAudioSource, targetVolume));
        else
        {
            currentSoundtrackAudioSource.volume = targetVolume * _soundtrackVolume;
            currentSoundtrackAudioSource.Play();
        }
    }

    public void PlayWithCrossFade(AudioClip clip, bool loop = false, float targetVolume = 1f)
    {
        StartCoroutine(CrossFade(clip, loop, targetVolume));
    }

    IEnumerator CrossFade(AudioClip newClip, bool loop, float targetVolume)
    {
        nextSoundtrackAudioSource.clip = newClip;
        nextSoundtrackAudioSource.loop = loop;
        nextSoundtrackAudioSource.Play();
        nextSoundtrackAudioSource.volume = 0f; // Start the nextSoundtrackAudioSource at 0 volume.

        float startTime = Time.time;
        while (Time.time < startTime + fadeTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            currentSoundtrackAudioSource.volume = 1f - t;
            nextSoundtrackAudioSource.volume = t * (targetVolume * _soundtrackVolume);
            yield return null;
        }

        // Swap the sources and stop the old one
        AudioSource temp = currentSoundtrackAudioSource;
        currentSoundtrackAudioSource = nextSoundtrackAudioSource;
        nextSoundtrackAudioSource = temp;
        nextSoundtrackAudioSource.Stop();
    }

    public void FadeOutCurrent()
    {
        StartCoroutine(FadeOut(currentSoundtrackAudioSource));
    }

    public void FadeOutCurrent(float fadeOutTime)
    {
        StartCoroutine(FadeOut(currentSoundtrackAudioSource, fadeOutTime));
    }

    IEnumerator FadeOut(AudioSource source, float fadeOutTime = 1.0f)
    {
        float startVolume = source.volume * _soundtrackVolume;
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
        StartCoroutine(FadeIn(currentSoundtrackAudioSource, targetVolume));
    }

    IEnumerator FadeIn(AudioSource source, float targetVolume)
    {
        source.Play();
        float startTime = Time.time;
        while (Time.time < startTime + fadeTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            source.volume = t * targetVolume * _soundtrackVolume;
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
        freeAudioSource.volume = volume * _sfxVolume;
        freeAudioSource.pitch = pitch;
        freeAudioSource.Play();
    }

    public void PauseInSceneAudioSources(bool pause)
    {
        if (_inSceneAudioSources.Length <= 0) return;

        if (pause)
        {
            for (int i = 0; i < _inSceneAudioSources.Length; i++)
            {
                if (_inSceneAudioSources[i].isPlaying) _inSceneAudioSources[i].Pause();
            }
        }
        else
        {
            for (int i = 0; i < _inSceneAudioSources.Length; i++)
            {
                _inSceneAudioSources[i].UnPause();
            }
        }
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = volume;
        for (int i = 0; i < _allInSceneSFXAudioSources.Length; i++)
        {
            var audioSourceVolumePair = _allInSceneSFXAudioSources[i];
            audioSourceVolumePair.Key.volume = audioSourceVolumePair.Value * volume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        _soundtrackVolume = volume;
        nextSoundtrackAudioSource.volume = volume;
        currentSoundtrackAudioSource.volume = volume;
    }
}
