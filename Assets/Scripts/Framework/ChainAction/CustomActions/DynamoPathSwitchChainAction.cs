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
    [SerializeField]
    private bool _reverseDirection = false;

    private CinemachineDollyCart _dynamoCart;
    private DynamoDistance _dynamoDisctance;

    private void Awake()
    {
        _dynamoCart = FindObjectOfType<CinemachineDollyCart>();
        _dynamoDisctance = _dynamoCart.gameObject.GetComponent<DynamoDistance>();

    }

    public override void Execute()
    {
        base.Execute();
        _dynamoCart.m_Path = _dynamoPath;

        _dynamoCart.m_Position = _startPosition;

        _dynamoDisctance.InverseDistance = _reverseDirection;
    }
}
