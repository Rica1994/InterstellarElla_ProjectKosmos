using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchAudioHandler : MonoBehaviour
{
    [SerializeField]
    private AudioSource _voiceAudioSource;

    [SerializeField]
    private AudioSource _engineAudioSource;

    [SerializeField]
    private AudioSource _engineIgnitionAudioSource;

    [SerializeField]
    private AudioSource _slippingAudioSource;

    [SerializeField]
    private AudioSource _wheelsAudioSource;

    [SerializeField]
    private AudioSource _bumpAudioSource;

    [SerializeField]
    private AudioClip[] _voiceClips;

    [SerializeField]
    private AudioClip[] _genericVoiceClips;

    /// <summary>
    /// First clip is ignition, second loop, third is extinguish
    /// </summary>
    [SerializeField]
    private AudioClip[] _jetEngineClips;

    [SerializeField]
    private AudioClip _landingClip;

    [SerializeField]
    private AudioSource _squeakyAudioSource;

    [SerializeField]
    private AudioClip[] _squeakyClips;

    [SerializeField]
    private AudioClip[] _bumpClips;

    [SerializeField]
    private float _thresholdSlippingSpeed = 6.0f;

    [SerializeField]
    private float _fadeTimeSlippingSound = 0.5f;

    private SimpleCarController _carController;

    private float _previousSteeringAngle;
    private int _lastSqueakyClipIndex = -1;
    private int _lastBumpClipIndex = -1;

    private float _slippySourceStartVolume = 1.0f;
    private Coroutine _fadeCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("PlayRandomSqueakySound", 5.0f, 5.0f);  // Call the function every 5 seconds
        _carController = ServiceLocator.Instance.GetService<GameManager>().PlayerController as SimpleCarController;
        if (_carController == null) return;
        _carController.BoostEvent += OnGlitchBoosted;
        _carController.BoostEndedEvent += OnBoostEnded;
        _carController.IdleEvent += OnGlitchIdle;
        _carController.LandedEvent += OnGlitchLanded;
        _carController.BumpEvent += OnGlitchBumped;

        _slippySourceStartVolume = _slippingAudioSource.volume;
    }

    private void OnGlitchBumped()
    {
        int rand;
        do
        {
            rand = Random.Range(0, _bumpClips.Length);
        } while (rand == _lastBumpClipIndex);

        _lastBumpClipIndex = rand;

        _bumpAudioSource.clip = _bumpClips[rand];
        _bumpAudioSource.pitch = Random.Range(0.7f, 1.3f);  // Adjust pitch slightly for variance

        _bumpAudioSource.Play();
    }

    private void OnGlitchLanded()
    {
        if (_carController == null) return;
        if (_wheelsAudioSource.isPlaying || _carController.GetVerticalSpeed() > -5.0f) return;
        _wheelsAudioSource.clip = _landingClip;
        _wheelsAudioSource.Play();
    }

    private void OnGlitchIdle()
    {
        Speak(_genericVoiceClips);
    }

    private void OnGlitchBoosted()
    {
        Speak(_genericVoiceClips);
        PlayJetEngine();
    }
    private void OnBoostEnded()
    {
        _engineAudioSource.Stop();
        _engineIgnitionAudioSource.Stop();
        _engineIgnitionAudioSource.clip = _jetEngineClips[2];
        _engineIgnitionAudioSource.loop = false;
        _engineIgnitionAudioSource.Play();
    }

    private void PlayJetEngine()
    {
        _engineIgnitionAudioSource.clip = _jetEngineClips[0];
        _engineIgnitionAudioSource.Play();

        _engineAudioSource.clip = _jetEngineClips[1];
        _engineAudioSource.loop = true;
        _engineAudioSource.Play();
    }

    private void Speak(AudioClip[] voiceClips)
    {
        int rand = Random.Range(0, voiceClips.Length);
        PlayAudio(voiceClips[rand]);
    }


    private void PlayRandomSqueakySound()
    {
        if (_carController == null) return;

        // Only play squeaky sound if the car is moving
        if (_carController.GetSpeed() > 1)
        {
            int rand = Random.Range(0, _squeakyClips.Length);
            _squeakyAudioSource.clip = _squeakyClips[rand];
            _squeakyAudioSource.Play();
        }
    }

    public void PlayAudio(AudioClip audioClip, bool overwrite = true)
    {
        if (audioClip == null)
        {
            return;
        }


        if (overwrite == false && _voiceAudioSource.isPlaying) return;

        _voiceAudioSource.Stop();
        _voiceAudioSource.clip = audioClip;
        _voiceAudioSource.Play();
    }

    private void Update()
    {
        if (_carController == null) return;

        // Only play squeaky sound if the car is turning significantly
        float currentSteeringAngle = _carController.SteeringAngle;
        if (Mathf.Abs(currentSteeringAngle - _previousSteeringAngle) > 10f && _squeakyAudioSource.isPlaying == false)
        {
            int rand;
            do
            {
                rand = Random.Range(0, _squeakyClips.Length);
            } while (rand == _lastSqueakyClipIndex);

            _lastSqueakyClipIndex = rand;

            _squeakyAudioSource.clip = _squeakyClips[rand];
            _squeakyAudioSource.pitch = Random.Range(0.9f, 1.1f);  // Adjust pitch slightly for variance

            _squeakyAudioSource.Play();
            _previousSteeringAngle = currentSteeringAngle;
        }

        if (_carController.IsSlipping && _slippingAudioSource.isPlaying == false && _carController.GetSpeed() > _thresholdSlippingSpeed)
        {
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            // _slippingAudioSource.pitch = Random.Range(0.9f, 1.1f);
            _slippingAudioSource.volume = _slippySourceStartVolume;
            _slippingAudioSource.Play();
            _slippingAudioSource.loop = true;
        }
        else if ((_carController.IsSlipping == false || _carController.GetSpeed() < 1.0f) && _slippingAudioSource.isPlaying )
        {
            _slippingAudioSource.loop = false;
            _fadeCoroutine = StartCoroutine(FadeAudio(_slippingAudioSource, _slippingAudioSource.clip.length / 2.0f));
        }

      // var speed = _carController.GetSpeed();
      // if (speed > 0)
      // {
      //     if (_wheelsAudioSource.isPlaying == false) _wheelsAudioSource.Play();
      //     _wheelsAudioSource.pitch 
      // }
      // else
      // {
      //     _wheelsAudioSource.Stop();
      // }
    }

    private IEnumerator FadeAudio(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;

        float startTime = Time.time;
        while (Time.time < startTime + fadeTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            audioSource.volume = startVolume * (1f - t);
            yield return null;
        }

        audioSource.Stop();
    }
}
