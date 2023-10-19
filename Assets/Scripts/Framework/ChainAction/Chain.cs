using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Chain
{
    #region Events

    public delegate void ChainDelegate(Chain chain);
    public event ChainDelegate OnChainCompleted;

    #endregion
    private Queue<ChainAction> _chainActions = new Queue<ChainAction>();

    private ChainAction _currentChainAction;


    public Chain(List<ChainAction> chainActions)
    {
        foreach (ChainAction chainAction in chainActions)
        {
            _chainActions.Enqueue(chainAction);
        }
    }

    private void OnCurrentChainActionDone()
    {
        EndCurrentChainAction();
    }

    public void AddAction(ChainAction chainAction)
    {
        _chainActions.Enqueue(chainAction);
    }
    
    public void Play()
    {
        Debug.Log("Chain Started");
        if (_chainActions.Count > 0)
        {
            PlayNextChainAction();
        }
    }
    
    public void EndCurrentChainAction(bool force = false)
    {
        if (_currentChainAction != null)
        {
            _currentChainAction.OnExit();
            _currentChainAction.ChainActionDone -= OnCurrentChainActionDone;
        }

        Debug.Log($"ChainAction {_currentChainAction.name} completed");

        if (_chainActions.Count > 0)
        {
            PlayNextChainAction();
        }
        else
        {
            OnChainCompleted?.Invoke(this);
        }
    }

    public void EndCurrentChainAction(PlayableDirector playableDirector)
    {
        if (_currentChainAction != null)
        {
            _currentChainAction.OnExit();
            _currentChainAction.ChainActionDone -= OnCurrentChainActionDone;
        }

        Debug.Log($"ChainAction {_currentChainAction.name} completed");
        playableDirector.Stop();

        if (_chainActions.Count > 0)
        {
            PlayNextChainAction();
        }
        else
        {
            OnChainCompleted?.Invoke(this);
        }
    }

    private void PlayNextChainAction()
    {
        try
        {
            _currentChainAction = _chainActions.Dequeue();
            _currentChainAction.ChainActionDone += OnCurrentChainActionDone;
            _currentChainAction.Execute();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void StopChain()
    {
        _chainActions.Clear();
        EndCurrentChainAction();
    }
}


