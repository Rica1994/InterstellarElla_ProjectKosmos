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

    protected AudioController _audioController;

    private bool _isActive = false;

    private void Start()
    {
        _audioController = ServiceLocator.Instance.GetService<AudioController>();
    }


    public virtual void ActivateCameraCutscene(SimpleCarController glitch)
    {
        float animationLength = _animationCamera.clip.length;

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
