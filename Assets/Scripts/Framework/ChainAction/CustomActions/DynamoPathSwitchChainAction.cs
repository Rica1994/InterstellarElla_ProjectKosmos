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
    private float _startPosition;

    private CinemachineDollyCart _dynamoCart;

    protected override void Awake()
    {
        base.Awake();
        _dynamoCart = FindObjectOfType<CinemachineDollyCart>();
    }

    public override void Execute()
    {
        base.Execute();
        _dynamoCart.m_Path = _dynamoPath;

        _dynamoCart.m_Position = _startPosition;
    }
}
