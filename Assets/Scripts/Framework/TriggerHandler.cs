using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
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
        if (other.GetComponent<ToDeleteMoveScript>() != null)
        {
            OnTriggered?.Invoke(this, other, true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ToDeleteMoveScript>() != null)
        {
            OnTriggered?.Invoke(this, other, false);
        }
    }
}
