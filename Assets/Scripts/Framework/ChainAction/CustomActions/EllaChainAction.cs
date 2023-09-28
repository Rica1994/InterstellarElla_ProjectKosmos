using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class EllaChainAction : ChainAction
{
    [SerializeField]
    private AudioElement _ellaVoiceClip;


    private void Awake()
    {
        _maxTime = _ellaVoiceClip.Clip.length;
    }

    public override void Execute()
    {
        base.Execute();
       
        ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_ellaVoiceClip);
    }
}
