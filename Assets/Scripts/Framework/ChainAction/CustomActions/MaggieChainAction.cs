using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.Audio;

public class MaggieChainAction : ChainAction
{
    [SerializeField]
    private AudioClip _maggieAudioClip;

    private Maggie _maggie;
    private AudioSource _maggieAudioSource;
    private MouthAnimation _maggieMouthAnimation;

    private void Awake()
    {
        _maggie = FindObjectOfType<Maggie>();
        _maggieAudioSource = _maggie.GetComponent<AudioSource>();
        _maggieMouthAnimation = _maggie.GetComponent<MouthAnimation>();


        if (_maggie == null)
        {
            Debug.LogError("No Maggie found! Please add maggie in the scene");
            return;
        }
        _maxTime = _maggie.PopUpLength + _maggie.PopDownLength + _maggieAudioClip.length;
    }

    public override void Execute()
    {
        base.Execute();
        _maggie.PopUp();
        _maggieMouthAnimation.VoiceSource = _maggieAudioSource;
        StartCoroutine(Helpers.DoAfter(_maggie.PopUpLength, () =>
        {
            //_maggieMouthAnimation.PlayAudioClip(_maggieAudioClip);
            _maggieAudioSource.clip = _maggieAudioClip;
            _maggieAudioSource.Play();
            //ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_maggieVoiceClip);
            StartCoroutine(Helpers.DoAfter(_maggieAudioClip.length, () => _maggie.PopDown()));
        }));
    }
}