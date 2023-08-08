using System.Collections;
using System.Collections.Generic;
using UnityCore.Scene;
using UnityEngine;

public class StartGameChainAction : ChainAction
{
    [SerializeField]
    private GameObject _cutsceneObject;

    [SerializeField]
    private SpeederGround _speederGround;

    public override void Execute()
    {
        base.Execute();
        Destroy(_cutsceneObject);
        _speederGround.enabled = true;
    }
}
