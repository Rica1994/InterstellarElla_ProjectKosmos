using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimatorParameterChainAction : ChainAction
{
    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private string _parameterName;
    [SerializeField]
    private float _parameterValue;
    public override void Execute()
    {
        base.Execute();

        _animator.SetFloat(_parameterName, _parameterValue);
    }
}
