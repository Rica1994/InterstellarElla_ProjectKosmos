using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class SoundtrackChainAction : ChainAction
{
    public enum SoundAction
    {
        None = 0,
        FadeOutCurrentSoundtrack = 1,
    }

    [SerializeField]
    private AudioClip _audioClip;

    [SerializeField]
    private bool _fadeIn = false;

    [SerializeField]
    private bool _fadeOut = false;

    [SerializeField]
    private bool _crossfade = false;

    [SerializeField]
    private bool _loop = false;

    [SerializeField]
    private bool _letCurrentClipFinish = false;

    [SerializeField]
    private float _targetVolume = 1.0f;

    [SerializeField]
    private SoundAction _soundAction = SoundAction.None;

    [SerializeField]
    private float _fadeTime = 1.0f;

    protected override void Awake()
    {
        base.Awake();
        if (_soundAction == SoundAction.FadeOutCurrentSoundtrack) 
        {
            _maxTime = _fadeTime;
            return;
        } 
        else if (_audioClip == null)
        {
            Debug.LogError("Audio clip not filled for " + gameObject.name);
            return;
        }
        _maxTime = _audioClip.length;
    }

    public override void Execute()
    {
        base.Execute();
        var soundTrackManager = ServiceLocator.Instance.GetService<SoundtrackManager>();
        switch (_soundAction)
        {
            case SoundAction.FadeOutCurrentSoundtrack:
                soundTrackManager.FadeOutCurrent(_fadeTime);
                break;
            default:
                soundTrackManager.PlayClip(_audioClip, _fadeIn, _crossfade, _loop, _targetVolume, _letCurrentClipFinish);
                break;
        }
    }
}
