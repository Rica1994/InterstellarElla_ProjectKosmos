using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventChainAction : ChainAction
{
    [SerializeField]
    private UnityEvent _unityEvent;

    public override void Execute()
    {
        base.Execute();
        _unityEvent?.Invoke();
    }
}
