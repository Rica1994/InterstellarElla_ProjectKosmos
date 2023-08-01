using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BoulderTrigger : MonoBehaviour
{
    [FormerlySerializedAs("_boulderAnimator")] [SerializeField]
    private Animator _animator;

    [SerializeField]
    private Collider _rockCollider;
    
    [SerializeField]
    private bool _requiresSpeedBoost = false;
    
    private bool _isActivated = false;
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if it is the car that collides with us
        SimpleCarController car = other.GetComponent<SimpleCarController>();
        if (_isActivated || car == null) return;
        if (_requiresSpeedBoost && car.IsBoosting == false) return;
        
        _isActivated = true;
        
        // Activates the boulder animation
        _animator.SetTrigger("Activate");
        
        _rockCollider.enabled = false;
    }

    private void FlickerAndDestroy()
    {
        StartCoroutine(Helpers.Flicker(gameObject, 3.0f, 10));
        StartCoroutine(Helpers.DoAfter(3.0f, () => Destroy(gameObject)));
    }
}
