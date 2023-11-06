using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateChainAction : ChainAction
{
    [SerializeField] 
    private GameObject _prefabToInstantiate;

    [SerializeField]
    private Transform _instantiateTransform;

    public override void Execute()
    {
        base.Execute();
        if ( _instantiateTransform != null)
        {
            var instantiatedObject = Instantiate(_prefabToInstantiate);
            instantiatedObject.transform.position = _instantiateTransform.position;
            instantiatedObject.transform.rotation = _instantiateTransform.rotation;
        }
    }
}
