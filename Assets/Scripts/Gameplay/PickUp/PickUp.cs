using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PickUp : MonoBehaviour
{
    public delegate void PickUpCallback(PickUp pickUp);
    public event PickUpCallback OnPickUp;
    
    private void Awake()
    {
        var collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    private void OnValidate()
    {
        var collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ToDeleteMoveScript>() != null)
        {
            PickedUp();
        }
    }

    private void PickedUp()
    {
        // particle effects
        Debug.Log("PickedUp");
        OnPickUp?.Invoke(this);
        Destroy(gameObject);
    }
}
