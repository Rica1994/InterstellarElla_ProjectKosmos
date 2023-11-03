using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using System.Runtime.InteropServices;
public class MouthAnimation : MonoBehaviour
{
    const int BAND_8 = 8;
    public AudioSource VoiceSource;

    private AudioClip _audioClip;
    private int _audioClipIndex;

    // Mouth references
    [SerializeField]
    private SkinnedMeshRenderer _halfOpenMouth;
    [SerializeField]
    private SkinnedMeshRenderer _wavyMouth;
    [SerializeField]
    private SkinnedMeshRenderer _lineMouth;

    [Header("Happy")]
    [SerializeField]
    private SkinnedMeshRenderer _closedMouthHappy;
    [SerializeField]
    private SkinnedMeshRenderer _openMouthHappy;
    [Header("Sad")]
    [SerializeField]
    private SkinnedMeshRenderer _closedMouthSad;
    [SerializeField]
    private SkinnedMeshRenderer _openMouthSad;

    [Header("Joint")]
    [SerializeField]
    private Transform _mouthJoint;
    [SerializeField]
    private float _mouthScaleMultiplier = 4.0f;

    [SerializeField]
    private PlayableDirector _playableDirector;

    public enum Mood
    {
        Happy,  
        Sad,
        Confused,
        Thinking
    }

    [SerializeField] private Mood MaggieMood;


    [SerializeField, Range(0f, 1f)]
    private float _timeTillNextMouthChange = 0.08f;

    private float _timePassedWithCurrentMouth = 0.0f;

    [Header("Thresholds")]
    [SerializeField]
    [Range(0f, 1f)]
    private float _thresholdHalfOpen = 0.06f;  // Default value
    [SerializeField]
    [Range(0f, 1f)]
    private float _thresholdOpen = 0.15f;      // Default value

    private int _sampleDataLength = 1024;
    private float[] samples;

    public static AudioBand AudioFrequencyBand8 { get; private set; }

    [DllImport("__Internal")]
    private static extern bool StartSampling(string name, float duration, int bufferSize);

    [DllImport("__Internal")]
    private static extern bool CloseSampling(string name);

    [DllImport("__Internal")]
    private static extern bool GetSamples(string name, float[] freqData, int size);

    // Import the JavaScript function
    [DllImport("__Internal")]
    private static extern void RegisterVisibilityChangeCallback();

    private void Awake()
    {
        if (_playableDirector != null)
        {
            SetTimelineAudioTrack();
        }
    }

    private void Start()
    {
        InitializeVariables();
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            RegisterVisibilityChangeCallback();
        }
    }

    private void InitializeVariables()
    {
        //samples = new float[_sampleDataLength];
        AudioFrequencyBand8 = new AudioBand(BandCount.Eight);
        //StartSampling(name, VoiceSource.clip.length, 512);
    }

    private void Update()
    {
        //UpdateMouth();
        if (VoiceSource.isPlaying)
        {
            AudioFrequencyBand8.Update((sample) =>
            {
#if UNITY_EDITOR
                VoiceSource.GetSpectrumData(sample, 0, FFTWindow.Blackman);
#elif UNITY_WEBGL && !UNITY_EDITOR
                Debug.Log("isPlaying");
                if (_playableDirector != null)
                {
                    StartSampling(name, _audioClip.length, 512);
                }
                else
                {
                    StartSampling(name, VoiceSource.clip.length, 512);
                }
                GetSamples(name, sample, sample.Length);
#endif
                UpdateMouth();
            });
        }
    }

    private void UpdateMouth()
    {
        _timePassedWithCurrentMouth += Time.deltaTime;

        if (_timePassedWithCurrentMouth > _timeTillNextMouthChange)
        {
            _timePassedWithCurrentMouth = 0.0f;
            float volume = AudioFrequencyBand8.GetAmplitude();

            if (volume < _thresholdHalfOpen)
            {
                _mouthJoint.localScale = new Vector3(1, 1, 1);
                //Debug.Log("Closed Mouth Activated");
                switch (MaggieMood)
                {
                    case Mood.Happy:
                        _halfOpenMouth.enabled = false;
                        _wavyMouth.enabled = false;
                        _lineMouth.enabled = false;
                        _closedMouthHappy.enabled = true;
                        _openMouthHappy.enabled = false;
                        _closedMouthSad.enabled = false;
                        _openMouthSad.enabled = false;
                        break;
                    case Mood.Sad:
                        _halfOpenMouth.enabled = false;
                        _wavyMouth.enabled = false;
                        _lineMouth.enabled = false;
                        _closedMouthHappy.enabled = false;
                        _openMouthHappy.enabled = false;
                        _closedMouthSad.enabled = true;
                        _openMouthSad.enabled = false;
                        break;
                    case Mood.Confused:
                        _halfOpenMouth.enabled = false;
                        _wavyMouth.enabled = true;
                        _lineMouth.enabled = false;
                        _closedMouthHappy.enabled = false;
                        _openMouthHappy.enabled = false;
                        _closedMouthSad.enabled = false;
                        _openMouthSad.enabled = false;
                        break;
                    case Mood.Thinking:
                        _halfOpenMouth.enabled = false;
                        _wavyMouth.enabled = false;
                        _lineMouth.enabled = true;
                        _closedMouthHappy.enabled = false;
                        _openMouthHappy.enabled = false;
                        _closedMouthSad.enabled = false;
                        _openMouthSad.enabled = false;
                        break;
                }
            }
            else if (volume >= _thresholdHalfOpen && volume < _thresholdOpen)
            {
                _mouthJoint.localScale = new Vector3(1, 1, 1);
                //Debug.Log("Half Open Mouth Activated");
                switch (MaggieMood)
                {
                    case Mood.Happy:
                        _halfOpenMouth.enabled = true;
                        _wavyMouth.enabled = false;
                        _lineMouth.enabled = false;
                        _closedMouthHappy.enabled = false;
                        _openMouthHappy.enabled = false;
                        _closedMouthSad.enabled = false;
                        _openMouthSad.enabled = false;
                        break;
                    case Mood.Sad:
                        _halfOpenMouth.enabled = true;
                        _wavyMouth.enabled = false;
                        _lineMouth.enabled = false;
                        _closedMouthHappy.enabled = false;
                        _openMouthHappy.enabled = false;
                        _closedMouthSad.enabled = false;
                        _openMouthSad.enabled = false;
                        break;
                    case Mood.Confused:
                        _halfOpenMouth.enabled = false;
                        _wavyMouth.enabled = true;
                        _lineMouth.enabled = false;
                        _closedMouthHappy.enabled = false;
                        _openMouthHappy.enabled = false;
                        _closedMouthSad.enabled = false;
                        _openMouthSad.enabled = false;
                        break;
                    case Mood.Thinking:
                        _halfOpenMouth.enabled = false;
                        _wavyMouth.enabled = false;
                        _lineMouth.enabled = true;
                        _closedMouthHappy.enabled = false;
                        _openMouthHappy.enabled = false;
                        _closedMouthSad.enabled = false;
                        _openMouthSad.enabled = false;
                        break;
                }
            }
            else
            {
                _mouthJoint.localScale = new Vector3(1, volume * _mouthScaleMultiplier, 1);
                //Debug.Log("Open Mouth Activated");
                switch (MaggieMood)
                {
                    case Mood.Happy:
                        _halfOpenMouth.enabled = false;
                        _wavyMouth.enabled = false;
                        _lineMouth.enabled = false;
                        _closedMouthHappy.enabled = false;
                        _openMouthHappy.enabled = true;
                        _closedMouthSad.enabled = false;
                        _openMouthSad.enabled = false;
                        break;
                    case Mood.Sad:
                        _halfOpenMouth.enabled = false;
                        _wavyMouth.enabled = false;
                        _lineMouth.enabled = false;
                        _closedMouthHappy.enabled = false;
                        _openMouthHappy.enabled = false;
                        _closedMouthSad.enabled = false;
                        _openMouthSad.enabled = true;
                        break;
                    case Mood.Confused:
                        _halfOpenMouth.enabled = false;
                        _wavyMouth.enabled = true;
                        _lineMouth.enabled = false;
                        _closedMouthHappy.enabled = false;
                        _openMouthHappy.enabled = false;
                        _closedMouthSad.enabled = false;
                        _openMouthSad.enabled = false;
                        break;
                    case Mood.Thinking:
                        _halfOpenMouth.enabled = false;
                        _wavyMouth.enabled = false;
                        _lineMouth.enabled = true;
                        _closedMouthHappy.enabled = false;
                        _openMouthHappy.enabled = false;
                        _closedMouthSad.enabled = false;
                        _openMouthSad.enabled = false;
                        break;
                }
            }
        }
    }

    void RefreshAudioSource()
    {
        // Store the AudioClip for later
        AudioClip clip = VoiceSource.clip;

        // Remove the AudioSource component
        Destroy(VoiceSource);

        // Add a new AudioSource component
        VoiceSource = gameObject.AddComponent<AudioSource>();

        // Reassign the AudioClip
        VoiceSource.clip = clip;
    }


    public void PlayAudioClip(AudioClip clip)
    {
        VoiceSource.clip = clip;
        RefreshAudioSource();
        InitializeVariables();
        VoiceSource.Play();
    }

    public void Restart()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        CloseSampling(name);
        #endif
    }

    private void SetTimelineAudioTrack()
    {
        TimelineAsset timeline = _playableDirector.playableAsset as TimelineAsset;
        //TrackAsset maggieTrack = timeline.GetOutputTracks().FirstOrDefault(t => t.name == "MaggieAudioTrack");

        AudioTrack audioTrack = null;
        foreach (var track in _playableDirector.playableAsset.outputs)
        {
            if (track.streamName == "MaggieAudioTrack")
            {
                audioTrack = track.sourceObject as AudioTrack;
                break;
            }
        }

        //_audioClip = ((AudioPlayableAsset)audioTrack.GetClips().FirstOrDefault().asset).clip;
        //if (_audioClip != null)
        //{
        //    Debug.Log("Name of audioclip: " + _audioClip.name);
        //}

        List<AudioClip> audioClips = new List<AudioClip>();

        foreach (var clip in audioTrack.GetClips())
        {
            // Assuming that the TimelineClip's asset is an AudioPlayableAsset
            AudioPlayableAsset audioPlayableAsset = clip.asset as AudioPlayableAsset;
            if (audioPlayableAsset != null)
            {
                // Retrieve the AudioClip and add it to the list
                AudioClip audioClip = audioPlayableAsset.clip;
                if (audioClip != null)
                {
                    audioClips.Add(audioClip);
                    Debug.Log("Found AudioClip: " + audioClip.name);
                }
            }
        }

        _audioClip = audioClips[_audioClipIndex];
        _audioClipIndex++;
    }

    // This will be called from JavaScript
    public void OnVisibilityChanged(int visible)
    {
        if (visible == 1)
        {
            Debug.Log("User is viewing the WebGL tab");

        }
        else
        {
            Debug.Log("User has left the WebGL tab");
            Restart();
        }
    }
}