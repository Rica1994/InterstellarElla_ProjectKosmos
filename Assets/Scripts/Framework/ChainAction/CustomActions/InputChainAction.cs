using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputChainAction : ChainAction, IRequisite
{
    [SerializeField]
    private KeyCode _keyToPress;

    public override void Execute()
    {
        base.Execute();
    }

    private void OnValidate()
    {
        _useUserBasedAction = true;
    }

    public override void UpdateAction(float elapsedTime)
    {
        base.UpdateAction(elapsedTime);
        if (_userBasedActionCompleted == false)
        {
            if (Input.GetKeyDown(_keyToPress))
            {
                _userBasedActionCompleted = true;
            }
        }
    }

    public bool IsRequisiteMet()
    {
        return _userBasedActionCompleted;
    }
}
