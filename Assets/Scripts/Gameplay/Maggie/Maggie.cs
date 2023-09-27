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
    
    private const string POP_UP_TRIGGER = "PopUp";
    private const string POP_DOWN_TRIGGER = "PopDown";
    private const string HAPPY_TRIGGER = "Happy";
    private const string SAD_TRIGGER = "Sad";
    
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

    public void PopUp()
    {
        Debug.Log(" Gone into pop-up ");
        Debug.Log(_animator.GetBool(POP_UP_TRIGGER) + " <---- should be false , found it ?");
        _animator.SetTrigger(POP_UP_TRIGGER);
        Debug.Log(_animator.GetBool(POP_UP_TRIGGER) + " <---- should be TRUE , found it ?");
    }

    public void PopDown()
    {
        _animator.SetTrigger(POP_DOWN_TRIGGER);
    }

    public void MakeHappy()
    {
        _animator.SetTrigger(HAPPY_TRIGGER);
    }

    public void MakeSad()
    {
        _animator.SetTrigger(SAD_TRIGGER);
    }
}