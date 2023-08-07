using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaggieTriggerChainAction : ChainAction
{
    void Start()
    {
        _useUserBasedAction = true;
    }

    private void OnValidate()
    {
        _useUserBasedAction = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player) == true)
        {
            //Debug.Log("Completing action");
            _userBasedActionCompleted = true;
        }
    }
}
