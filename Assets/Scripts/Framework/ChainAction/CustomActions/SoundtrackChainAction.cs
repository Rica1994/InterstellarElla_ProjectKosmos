using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class SoundtrackChainAction : ChainAction
{
    [SerializeField]
    private AudioClip _audioClip;

    [SerializeField]
    private bool _fadeIn = false;

    [SerializeField]
    private bool _crossfade = false;

    [SerializeField]
    private bool _loop = false;

    [SerializeField]
    private float _targetVolume = 1.0f;

    protected override void Awake()
    {
        base.Awake();

        _maxTime = _audioClip.length;
    }

    public override void Execute()
    {
        base.Execute();
        ServiceLocator.Instance.GetService<SoundtrackManager>().PlayClip(_audioClip, _fadeIn, _crossfade, _loop, _targetVolume);
    }
}
