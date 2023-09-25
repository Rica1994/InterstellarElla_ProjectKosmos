using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class MaggieChainAction : ChainAction
{
    [SerializeField]
    private AudioElement _maggieVoiceClip;

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
        _maxTime = _maggie.PopUpLength + _maggie.PopDownLength + _maggieVoiceClip.Clip.length;
    }

    public override void Execute()
    {
        base.Execute();
        _maggie.PopUp();
        //_maggieMouthAnimation.VoiceSource = ServiceLocator.Instance.GetService<AudioController>().TracksMaggie[0].Source;
        StartCoroutine(Helpers.DoAfter(_maggie.PopUpLength, () =>
        {
            _maggieMouthAnimation.PlayAudioClip(_maggieAudioClip);
            //ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_maggieVoiceClip);
            StartCoroutine(Helpers.DoAfter(_maggieAudioClip.length, () => _maggie.PopDown()));
        }));
    }
}