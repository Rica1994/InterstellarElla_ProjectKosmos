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

    [SerializeField, Tooltip("If this is filled in, a check will be made whether we are on pc or not, if so this clip will play instead")]
    private AudioClip _maggiePcAudioClip;

    private Maggie _maggie;
    private AudioSource _maggieAudioSource;
    private MouthAnimation _maggieMouthAnimation;

    [SerializeField]
    private MouthAnimation.Mood _maggieMood;

    protected override void Awake()
    {
        base.Awake();

        _maggie = FindObjectOfType<Maggie>();
        _maggieAudioSource = _maggie.GetComponent<AudioSource>();
        _maggieMouthAnimation = _maggie.GetComponent<MouthAnimation>();

        // check whether we are on pc
        if (_maggiePcAudioClip != null && ServiceLocator.Instance.GetService<GameManager>().IsMobileWebGl == false)
        {
            _maggieAudioClip = _maggiePcAudioClip;
        }

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

        if (_maggieAudioClip == null)
        {
            Debug.Log("MaggieAudioClip was null");
            return;
        }

        _maggieMouthAnimation.Restart();
        _maggieMouthAnimation.MaggieMood = _maggieMood;
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