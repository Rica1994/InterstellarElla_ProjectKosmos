using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputChainAction : ChainAction
{
    [SerializeField]
    private KeyCode _keyToPress;

    public override void Execute()
    {
        base.Execute();
    }

    public override void UpdateAction(float elapsedTime)
    {
        if (_userBasedActionCompleted == false)
        {
            base.UpdateAction(elapsedTime);
            if (Input.GetKeyDown(_keyToPress))
            {
                _userBasedActionCompleted = true;
            }    
        }
    }
}