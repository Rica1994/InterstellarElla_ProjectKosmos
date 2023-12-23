using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System;

[DefaultExecutionOrder(-500)]
[RequireComponent(typeof(AudioSource))]
public class SoundManager : Service
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

    [SerializeField]
    private AudioSource[] _3DSfxAudioSources;

    [SerializeField]
    private AudioSource _chordAudioSource;

    private AudioSource _currentSoundtrackAudioSource;
    private AudioSource _nextSoundtrackAudioSource;

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
        _currentSoundtrackAudioSource = gameObject.GetComponent<AudioSource>();
        _nextSoundtrackAudioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        Debug.Log("Start called from " + gameObject.name);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        StartCoroutine(InitializeNextFrame());
    }

    /// <summary>
    /// Stops all soundtrack audio sources
    /// </summary>
    /// <param name="arg0"></param>
    private void OnSceneUnloaded(Scene arg0)
    {
        _currentSoundtrackAudioSource.Stop();
        _nextSoundtrackAudioSource.Stop();
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
        allSFXAudioSources.Remove(_currentSoundtrackAudioSource);
        allSFXAudioSources.Remove(_nextSoundtrackAudioSource);

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
        FadeOutCurrent();
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
        _currentSoundtrackAudioSource.loop = false;
        yield return new WaitUntil(() => _currentSoundtrackAudioSource.time >= _currentSoundtrackAudioSource.clip.length - 0.03f || _currentSoundtrackAudioSource.isPlaying == false);

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
            if (_currentSoundtrackAudioSource.isPlaying)
                _currentSoundtrackAudioSource.Stop();
            _currentSoundtrackAudioSource.clip = clip;
            _currentSoundtrackAudioSource.loop = loop;
            if (fadeIn)
                StartCoroutine(FadeIn(_currentSoundtrackAudioSource, targetVolume));
            else
            {
                _currentSoundtrackAudioSource.volume = targetVolume * _soundtrackVolume;
                _currentSoundtrackAudioSource.Play();
            }
        }
    }

    IEnumerator FadeOutAndPlayNext(AudioClip clip, bool fadeIn, bool loop, float targetVolume)
    {
        yield return StartCoroutine(FadeOut(_currentSoundtrackAudioSource));
        _currentSoundtrackAudioSource.clip = clip;
        _currentSoundtrackAudioSource.loop = loop;
        if (fadeIn)
            StartCoroutine(FadeIn(_currentSoundtrackAudioSource, targetVolume));
        else
        {
            _currentSoundtrackAudioSource.volume = targetVolume * _soundtrackVolume;
            _currentSoundtrackAudioSource.Play();
        }
    }

    public void PlayWithCrossFade(AudioClip clip, bool loop = false, float targetVolume = 1f)
    {
        StartCoroutine(CrossFade(clip, loop, targetVolume));
    }

    IEnumerator CrossFade(AudioClip newClip, bool loop, float targetVolume)
    {
        _nextSoundtrackAudioSource.clip = newClip;
        _nextSoundtrackAudioSource.loop = loop;
        _nextSoundtrackAudioSource.Play();
        _nextSoundtrackAudioSource.volume = 0f; // Start the nextSoundtrackAudioSource at 0 volume.

        float startTime = Time.time;
        while (Time.time < startTime + fadeTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            _currentSoundtrackAudioSource.volume = 1f - t;
            _nextSoundtrackAudioSource.volume = t * (targetVolume * _soundtrackVolume);
            yield return null;
        }

        // Swap the sources and stop the old one
        AudioSource temp = _currentSoundtrackAudioSource;
        _currentSoundtrackAudioSource = _nextSoundtrackAudioSource;
        _nextSoundtrackAudioSource = temp;
        _nextSoundtrackAudioSource.Stop();
    }

    public void FadeOutCurrent()
    {
        StartCoroutine(FadeOut(_currentSoundtrackAudioSource));
    }

    public void FadeOutCurrent(float fadeOutTime)
    {
        StartCoroutine(FadeOut(_currentSoundtrackAudioSource, fadeOutTime));
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
        StartCoroutine(FadeIn(_currentSoundtrackAudioSource, targetVolume));
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

    public void PlayClipAt3DLocation(Vector3 pos, AudioSourceSetting setting)
    {
        if (_passByAudioSource.isPlaying) return;

        _passByAudioSource.transform.position = pos;

        float randomPitch = _passByAudioSourceStandardPitch + UnityEngine.Random.Range(-0.15f, 0.15f);
        _passByAudioSource.pitch = randomPitch;
      //  _passByAudioSource.minDistance = setting.MinMaxDistance.x;
      //  _passByAudioSource.maxDistance = setting.MinMaxDistance.y;
      //  _passByAudioSource.loop = setting.IsLooping;

        _passByAudioSource.Play();
    }

    public AudioSource PlaySFXAt3DLocation(AudioClip clip, Vector3 pos, AudioSourceSetting setting)
    {
        var freeAudioSource = GetFreeAudioSource(_3DSfxAudioSources);
        if (freeAudioSource == null) return null;

        float randomPitch = 1.0f + UnityEngine.Random.Range(setting.MinMaxRandomPitchValues.x, setting.MinMaxRandomPitchValues.y);

        freeAudioSource.clip = clip;
        freeAudioSource.transform.position = pos;
        freeAudioSource.pitch = randomPitch;
        freeAudioSource.volume = setting.Volume;
        freeAudioSource.minDistance = setting.MinMaxDistance.x;
        freeAudioSource.maxDistance = setting.MinMaxDistance.y;
        freeAudioSource.loop = setting.IsLooping;
        freeAudioSource.Play();

        return freeAudioSource;
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
                if (_inSceneAudioSources[i] == null) continue;
                if (_inSceneAudioSources[i].isPlaying) _inSceneAudioSources[i].Pause();
            }
        }
        else
        {
            for (int i = 0; i < _inSceneAudioSources.Length; i++)
            {
                if (_inSceneAudioSources[i] == null) continue;
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
        _nextSoundtrackAudioSource.volume = volume;
        _currentSoundtrackAudioSource.volume = volume;
    }

    // Call this method with an index to play a specific note in the major chord at a potentially higher octave
    public void PlayNoteInMajorChord(AudioClip clip, int noteIndex, float basePitch = 1.0f)
    {
        int[] intervals = new int[] { 0, 4, 7 }; // Major chord intervals: root, major third, perfect fifth
        int octave = noteIndex / intervals.Length; // Calculate how many octaves to jump
        int actualIndex = noteIndex % intervals.Length; // Calculate the note within the chord

        float targetPitch = basePitch * Mathf.Pow(2, intervals[actualIndex] / 12f + octave);

        _chordAudioSource.clip = clip;
        _chordAudioSource.pitch = targetPitch;
        _chordAudioSource.Play();
    }

    public AudioSource GetFreeAudioSource(AudioSource[] sources)
    {
        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i].isPlaying == false) return sources[i];
        }

        Debug.LogWarning("GetFreeAudioSource()  no free audio sources found!");
        return null;
    }
}

[Serializable]
public struct AudioSourceSetting
{
    public bool IsThreeDimensional;
    public bool IsLooping;
    public float Volume;
    public Vector2 MinMaxDistance;
    public Vector2 MinMaxRandomPitchValues;

    public AudioSourceSetting(float volume, bool isThreeDimensional, bool isLooping, Vector2 minMaxDistance, Vector2 minMaxRandomPitchValues)
    {
        Volume = volume;
        IsThreeDimensional = isThreeDimensional;
        IsLooping = isLooping;
        MinMaxDistance = minMaxDistance;
        MinMaxRandomPitchValues = minMaxRandomPitchValues;
    }

    // Overloaded constructor with defaults
    // Static factory method to create a default AudioSourceSetting
    public static AudioSourceSetting CreateDefault()
    {
        return new AudioSourceSetting(1.0f, false, false, Vector2.zero, Vector2.zero);
    }
}