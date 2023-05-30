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

        // load in the correct model ?!
        //Resources.Load();
    }

    private void OnValidate()
    {
        var collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            PickedUp();
        }
    }

    private void PickedUp()
    {
        OnPickUp?.Invoke(this);

        // spawn particle
        ServiceLocator.Instance.GetService<ParticleManager>().
            CreateParticleWorldSpace(ParticleType.PS_PickupTrigger, this.transform.position);

        Destroy(gameObject);
    }
}
