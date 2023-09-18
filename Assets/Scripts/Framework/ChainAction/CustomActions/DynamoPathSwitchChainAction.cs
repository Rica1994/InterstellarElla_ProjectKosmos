using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DynamoPathSwitchChainAction : ChainAction
{
    [SerializeField]
    private CinemachinePath _dynamoPath;
    [SerializeField]
    private bool _resetPosition = true;

    private CinemachineDollyCart _dynamoCart;

    private void Awake()
    {
        _dynamoCart = FindObjectOfType<CinemachineDollyCart>();
    }

    public override void Execute()
    {
        base.Execute();
        _dynamoCart.m_Path = _dynamoPath;
        if (_resetPosition)
        {
            _dynamoCart.m_Position = 0;
        }
    }
}
