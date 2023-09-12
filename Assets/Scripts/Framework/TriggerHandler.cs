using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerHandler : MonoBehaviour
{
    public delegate void TriggerHandlerCallBack(TriggerHandler me, Collider other, bool hasEntered);

    public event TriggerHandlerCallBack OnTriggered;

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
        OnTriggered?.Invoke(this, other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggered?.Invoke(this, other, false);
    }
}