using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeederGroundSpeedChainAction : ChainAction
{
    [SerializeField]
    private float _forwardSpeed;
    [SerializeField]
    private SpeederGround _speederGround;

    public override void Execute()
    {
        base.Execute();
        _speederGround.speedForward = _forwardSpeed;
    }
}