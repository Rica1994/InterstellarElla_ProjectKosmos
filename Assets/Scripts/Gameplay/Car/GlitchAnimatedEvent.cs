using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class GlitchAnimatedEvent : MonoBehaviour
{
    [Header("Cutscene")]
    [SerializeField] protected Animation _animationCamera;
    [SerializeField] protected CinemachineVirtualCamera _virtualCamera;

    [Header("Components &|| Children")]
    [SerializeField] protected Animation _animationObject;

    [Header("Sounds")]
    [SerializeField] protected AudioElement _soundToPlay;

    [Header("Particles")]
    [SerializeField]
    private GameObject _particleIndication;

    protected AudioController _audioController;

    private bool _isActive = false;

    protected virtual void Start()
    {
        _audioController = ServiceLocator.Instance.GetService<AudioController>();
    }


    public virtual void ActivateCameraCutscene(SimpleCarController glitch)
    {
        float animationLength = _animationCamera.clip.length;

        // stop particle
        if (_particleIndication != null)
        {
            _particleIndication.SetActive(false);
        }

        // freeze gameplay
        glitch.ToggleMoveInput(animationLength);

        // logic to swap current virtual camera, and return to normal afterwards
        ServiceLocator.Instance.GetService<VirtualCameraManagerExploring>().SwapCutsceneCamera(_virtualCamera, animationLength);

        // play cutscene animation
        _animationCamera.Play();

        // activate animation of pushed object
        _animationObject.Play();

        // play sound lever
        _audioController.PlayAudio(_soundToPlay);

        _isActive = true;
    }
}
