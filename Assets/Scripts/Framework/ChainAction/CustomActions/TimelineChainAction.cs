using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineChainAction : ChainAction
{
    [SerializeField]
    private PlayableDirector _playableDirector;

    private void Awake()
    {
        _maxTime = (float)_playableDirector.duration;
    }

    public override void Execute()
    {
        base.Execute();
        _playableDirector.Play();
    }
}