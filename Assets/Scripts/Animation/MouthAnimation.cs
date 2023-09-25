using Assets.Scripts;
using System.Runtime.InteropServices;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.UI;

public class MouthAnimation : MonoBehaviour
{
    public AudioSource VoiceSource;
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

    //[SerializeField]
    //private Text _text;

    private bool _clipStopped = false;

    private void Start()
    {
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        samples = new float[_sampleDataLength];
        AudioFrequencyBand8 = new AudioBand(BandCount.Eight);
    }

    private void Update()
    {
        if (VoiceSource.isPlaying)
        {
            AudioFrequencyBand8.Update((sample) =>
            {
            #if UNITY_EDITOR
                VoiceSource.GetSpectrumData(sample, 0, FFTWindow.Blackman);
            #endif
            #if UNITY_WEBGL && !UNITY_EDITOR
                StartSampling(name, VoiceSource.clip.length, 512);
                bool gotSamples = GetSamples(name, sample, sample.Length);
                //_text.text = "Got Samples: " + gotSamples + "\n" + "Sample Data: " + string.Join(", ", sample);
            #endif
                UpdateMouth();
            });
        }
        else
        {
            //_text.text = VoiceSource.name + ":no values";
            _clipStopped = true;
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
        AudioFrequencyBand8.Reset();
        VoiceSource.Play();
    }
}