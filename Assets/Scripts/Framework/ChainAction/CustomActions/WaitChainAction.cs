using System.Collections;
using System.Collections.Generic;
using UnityCore.Scene;
using UnityEngine;

public class WaitChainAction : ChainAction
{
    [SerializeField]
    private float _waitSeconds;


    public override void Execute()
    {
        base.Execute();
        _maxTime = _waitSeconds;
    }
}
