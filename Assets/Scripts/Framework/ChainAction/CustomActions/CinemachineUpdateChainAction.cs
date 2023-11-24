using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CinemachineUpdateChainAction : ChainAction
{
    [SerializeField]
    private CinemachineBrain _cinemachineBrain;

    [SerializeField]
    private CinemachineBrain.UpdateMethod _updateMethod;

    public override void Execute()
    {
        base.Execute();
        if (_cinemachineBrain != null)
        {
            _cinemachineBrain.m_UpdateMethod = _updateMethod;
        }
    }
}