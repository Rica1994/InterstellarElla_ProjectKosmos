using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// https://answers.unity.com/questions/242648/force-on-character-controller-knockback.html
public class ImpactRecieverComponent
{
    private CharacterController _characterController;
    private Vector3 _impact = Vector3.zero;
    private float _mass;

    public ImpactRecieverComponent(CharacterController characterController, float mass)
    {
        _characterController = characterController;
        _mass = mass;
    }

    public void AddImpact(Vector3 direction, float force)
    {
        direction.Normalize();
        if (direction.y < 0)
        {
            direction.y *= -1f;
        }

        _impact += direction * force * _mass;
    }

    public void Update()
    {
        if (_impact.sqrMagnitude > Mathf.Pow(0.2f, 2f))
        {
            _characterController.Move(_impact * Time.deltaTime);
            _impact = Vector3.Lerp(_impact, Vector3.zero, 5f * Time.deltaTime);
        }
    }
}
