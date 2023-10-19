using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class AudioChainAction : ChainAction
{
    [SerializeField]
    private AudioElement _audioElement;

    protected override void Awake()
    {
        base.Awake();
        _maxTime = _audioElement.Clip.length;
    }

    public override void Execute()
    {
        base.Execute();
        ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_audioElement);
    }
}
