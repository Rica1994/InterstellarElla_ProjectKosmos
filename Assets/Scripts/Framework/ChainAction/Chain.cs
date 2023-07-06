using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    public void AddAction(ChainAction chainAction)
    {
        _chainActions.Enqueue(chainAction);
    }
    
    public void UpdateChain(float elapsedTime)
    {
        if (_chainActions.Count == 0) return;
        
        _currentChainAction = _chainActions.Peek();
        _currentChainAction.UpdateAction(elapsedTime);
        if (_currentChainAction.IsCompleted())
        {
            EndCurrentChainAction();
        }
    }

    public void Play()
    {
        Debug.Log("Chain Started");
        if (_chainActions.Count > 0)
        {
            ChainAction nextChainAction = _chainActions.Peek();
            nextChainAction.Execute();
        }
    }

    public void EndCurrentChainAction()
    {
        _currentChainAction.OnExit();
        Debug.Log($"ChainAction {_currentChainAction.name} completed");
        _chainActions.Dequeue();
        if (_chainActions.Count > 0)
        {
            ChainAction nextChainAction = _chainActions.Peek();
            nextChainAction.OnEnter();
            nextChainAction.Execute();
        }
        else
        {
            OnChainCompleted?.Invoke(this);
        }
    }
}


