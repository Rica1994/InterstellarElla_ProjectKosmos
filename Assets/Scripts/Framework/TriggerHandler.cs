using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerHandler : MonoBehaviour, IRequisite
{
    public delegate void TriggerHandlerCallBack(TriggerHandler me, Collider other, bool hasEntered);

    public event TriggerHandlerCallBack OnTriggered;

    private bool _isTriggered = false;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnValidate()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        _isTriggered = true;
        OnTriggered?.Invoke(this, other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggered?.Invoke(this, other, false);
    }

    public bool IsRequisiteMet()
    {
        return _isTriggered;
    }
}