using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineChainAction : ChainAction
{
    [SerializeField]
    private PlayableDirector _playableDirector;

    [SerializeField]
    private SkipCutscene _skipButton;

    protected override void Awake()
    {
        base.Awake();

        _maxTime = (float)_playableDirector.duration;
        
    }

    public override void Execute()
    {
        base.Execute();
        _playableDirector.Play();

        if (_skipButton != null)
        {
            _skipButton.playableDirector = _playableDirector;
            _skipButton.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_skipButton != null)
        {
            _skipButton.transform.GetChild(0).gameObject.SetActive(false);
        }

    }
}