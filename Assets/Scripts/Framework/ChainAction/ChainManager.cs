using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainManager : MonoBehaviourSingleton<ChainManager>
{
    private Chain _chain;

    private void Update()
    {
        if (_chain != null)
        {
            _chain.UpdateChain(Time.deltaTime);
        }
    }
    
    public void StartChain(Sequence sequence)
    {
        if (_chain != null)
        {
            Debug.Log("Chain was still playing. Aborting StartChain call.");
            return;
        }
        
        _chain = new Chain(sequence.ChainActions);
        _chain.OnChainCompleted += OnChainCompleted;
        _chain.Play();
    }

    private void OnChainCompleted(Chain chain)
    {
        _chain = null;
    }

    public Chain GetChain()
    {
        Chain CurrentChain = _chain;
        return CurrentChain;
    }
}
