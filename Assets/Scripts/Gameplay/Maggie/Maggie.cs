using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maggie : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private AnimationClip _popUpClip;
    
    [SerializeField]
    private AnimationClip _popDownClip;
    
    private const string popDownTrigger = "PopDown";
    private float _popUpLength = 2.0f;
    private float _popDownLength = 2.0f;

    public Animator Animator => _animator;
    public float PopUpLength => _popUpLength;
    public float PopDownLength => _popDownLength;


    private void Awake()
    {
        _popUpLength = _popUpClip.length;
        _popDownLength = _popDownClip.length;
    }

    public void PopDown()
    {
     //   _animator.GetCurrentAnimatorClipInfo()
        _animator.SetTrigger(popDownTrigger);
    }
}
