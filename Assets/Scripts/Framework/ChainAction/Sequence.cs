using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : MonoBehaviour
{
    [SerializeField]
    private List<ChainAction> _chainActions = new List<ChainAction>();

    public List<ChainAction> ChainActions => _chainActions;
    
    private float elapsedTime = 0.0f;
    
    private void Start()
    {
      //  ChainManager.Instance.StartChain(this);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 10)
        {
            ChainManager.Instance.StartChain(this);
            elapsedTime = -Mathf.Infinity;
        }

    }
}
