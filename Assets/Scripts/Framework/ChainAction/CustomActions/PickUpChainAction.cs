using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpChainAction : ChainAction
{
    [SerializeField]
    private int _amountPickUpsBeforeCompleted = 1;
    // Start is called before the first frame update
    void Start()
    {
        _useUserBasedAction = true;
        ServiceLocator.Instance.GetService<PickUpManager>().PickUpPickedUpEvent += OnPickUpPickedUp;
    }

    private void OnValidate()
    {
        _useUserBasedAction = true;
    }

    private void OnPickUpPickedUp(int pickupspickedup)
    {
        if (pickupspickedup >= _amountPickUpsBeforeCompleted)
        {
            _userBasedActionCompleted = true;
        }
    }
}
