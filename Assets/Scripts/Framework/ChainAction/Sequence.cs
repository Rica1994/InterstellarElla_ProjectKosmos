using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var box = GetComponent<BoxCollider>();
        if (box == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(box.center, box.size);
    }
#endif
}
