using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

[DefaultExecutionOrder(-50)]
[RequireComponent(typeof(SphereCollider))]
public class PickUp : MonoBehaviour
{
    public delegate void PickUpCallback(PickUp pickUp);
    public event PickUpCallback OnPickUp;

    private bool _pickedUp;

    [SerializeField]
    private Type _PickUpType;

    public enum Type
    {
        Basic = 0,
        Special = 1,
    }

    private void Start()
    {
        var collider = GetComponent<Collider>();
        collider.isTrigger = true;

        // load in correct prefab visual
        LoadVisuals();
    }

    private void OnValidate()
    {
        var collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null && _pickedUp == false)
        {
            PickedUp();
        }
    }

    private void PickedUp()
    {
        OnPickUp?.Invoke(this);

        PlayerFeedback();

        _pickedUp = true;
    }
    protected virtual void PlayerFeedback()
    {
        // spawn particle (present in particle manager)
        ServiceLocator.Instance.GetService<ParticleManager>().CreateParticleWorldSpace(ParticleType.PS_PickupTrigger, this.transform.position);
        // sound is played through event with PickupManager

        Destroy(gameObject);
    }
    protected virtual void LoadVisuals()
    {
        // load in the correct prefab in the pickup manager (dependant on scene) (I only want to do this once for 1 pickup)
        if (PickUpManager.PickupNormalVisual != null)
        {
            // destroy my alrdy existing child
            Destroy(this.transform.GetChild(0).gameObject);

            // instantiate whatever was loaded here under my object
            Instantiate(PickUpManager.PickupNormalVisual, this.transform);
        }
        else
        {
            Debug.Log("No Pickups Found, leaving on default visual");
        }    
    }
}
