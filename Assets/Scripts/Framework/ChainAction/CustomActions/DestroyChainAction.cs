using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyChainAction : ChainAction
{
    [SerializeField]
    private GameObject[] _objectsToDestroy;

    public override void Execute()
    {
        base.Execute();
        for (int i = 0; i < _objectsToDestroy.Length; i++)
        {
            Destroy(_objectsToDestroy[i]);
        }
    }
}
