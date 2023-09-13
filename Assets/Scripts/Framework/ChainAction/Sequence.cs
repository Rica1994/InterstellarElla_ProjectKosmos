using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : MonoBehaviour   
{
    [SerializeField]
    private List<ChainAction> _chainActions = new List<ChainAction>();

    [SerializeField]
    private bool _startOnAwake = false;
    
    public List<ChainAction> ChainActions => _chainActions;

    private void Start()
    {
      if (_startOnAwake) ChainManager.Instance.StartChain(this);
    }
}
