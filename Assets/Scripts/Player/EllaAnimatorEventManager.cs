using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class EllaAnimatorEventManager : MonoBehaviour
{
    [Header("Foot Transforms")]
    [SerializeField]
    private Transform _particleFootstepLeftTransform;
    [SerializeField]
    private Transform _particleFootstepRightTransform;

    [Header("Audio")]
    [SerializeField]
    private List<AudioElement> _soundEffectsFootsteps = new List<AudioElement>();

    private ParticleManager _particleManager;
    private AudioController _audioController;

    private ParticleSystem _particleLeftFoot;
    private ParticleSystem _particleRightFoot;


    private void Start()
    {
        _particleManager = ServiceLocator.Instance.GetService<ParticleManager>();
        _audioController = ServiceLocator.Instance.GetService<AudioController>();

        CreateParticleSystemsAtFeet();
    }


    public void PlayFootStepLeftEvent()
    {
        PlayRandomFootStepSound();
        PlayFootStepParticleLeft();
    }
    public void PlayFootStepRightEvent()
    {
        PlayRandomFootStepSound();
        PlayFootStepParticleRight();
    }




    private void PlayRandomFootStepSound()
    {
        if (_soundEffectsFootsteps.Count > 0)
        {
            int randomInt = Random.Range(0, _soundEffectsFootsteps.Count);
            AudioElement randomSound = _soundEffectsFootsteps[randomInt];

            _audioController.PlayAudio(randomSound);
        }
    }
    private void PlayFootStepParticleLeft()
    {
        _particleLeftFoot.Play();
    }
    private void PlayFootStepParticleRight()
    {
        _particleRightFoot.Play();
    }




    private void CreateParticleSystemsAtFeet()
    {
        _particleLeftFoot = _particleManager.CreateParticleLocalSpacePermanent(ParticleType.PS_Footstep, _particleFootstepLeftTransform);
        _particleRightFoot = _particleManager.CreateParticleLocalSpacePermanent(ParticleType.PS_Footstep, _particleFootstepRightTransform);
    }

}
