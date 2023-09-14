using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class MaggieChainAction : ChainAction
{
    [SerializeField]
    private AudioElement _maggieVoiceClip;

    private Maggie _maggie;

    private void Awake()
    {
        foreach (var maggie in Resources.FindObjectsOfTypeAll<Maggie>())
        {

            _maggie = maggie;

        }

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
        _maggie.gameObject.SetActive(true);
        //  StartCoroutine(PlayAudioAfter(_maggie.PopUpLength));
        StartCoroutine(Helpers.DoAfter(_maggie.PopUpLength, () =>
        {
            ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_maggieVoiceClip);
            StartCoroutine(Helpers.DoAfter(_maggieVoiceClip.Clip.length, () => _maggie.PopDown()));
        }));
    }

    public override void OnExit()
    {
        base.OnExit();
        _maggie.gameObject.SetActive(false);
    }
}