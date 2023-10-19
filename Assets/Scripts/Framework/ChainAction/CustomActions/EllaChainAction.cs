using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityCore.Audio;
using UnityEngine;

public class EllaChainAction : ChainAction
{
    [SerializeField]
    private AudioClip _ellaClip;

    private AudioSource _ellaVoiceAudioSource;

    protected override void Awake()
    {
        base.Awake();

        _maxTime = _ellaClip.length;
        IElla ellaObject = FindObjectsOfType<MonoBehaviour>().FirstOrDefault(obj => obj is IElla) as IElla;
        if (ellaObject == null)
        {
            Debug.LogError("No ella found!");
        }
        _ellaVoiceAudioSource = ellaObject.VoiceAudioSource;
    }

    public override void Execute()
    {
        base.Execute();

        _ellaVoiceAudioSource.clip = _ellaClip;
        _ellaVoiceAudioSource.Play();
    }
}
