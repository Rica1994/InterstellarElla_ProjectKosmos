using UnityCore.Audio;
using UnityEngine;

public class MouthAnimation : MonoBehaviour
{
    [SerializeField]
    private AudioSource _voiceSource;
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

    private void Start()
    {
        samples = new float[_sampleDataLength];

        //_voiceSource = ServiceLocator.Instance.GetService<AudioController>().TracksMaggie[0].Source as AudioSource;

    }

    private void Update()
    {
        if (_voiceSource.isPlaying)
        {
            _timePassedWithCurrentMouth += Time.deltaTime;

            if (_timePassedWithCurrentMouth > _timeTillNextMouthChange)
            {
                _timePassedWithCurrentMouth = 0.0f;
                float volume = RMSValue();

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
    }

    float RMSValue()
    {
        _voiceSource.GetOutputData(samples, 0);

        float sum = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }
        return Mathf.Sqrt(sum / samples.Length);
    }
}