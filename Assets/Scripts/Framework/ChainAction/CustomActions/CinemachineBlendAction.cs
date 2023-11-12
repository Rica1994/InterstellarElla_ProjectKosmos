using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CinemachineBlendAction : ChainAction
{
    [SerializeField]
    private CinemachineBrain _cinemachineBrain;
    [SerializeField]
    private CinemachineBlendDefinition _cinemachineBlendDefinition;

    public override void Execute()
    {
        base.Execute();
        if (_cinemachineBrain != null)
        {
            _cinemachineBrain.m_DefaultBlend = _cinemachineBlendDefinition;
        }
    }
}