using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference: https://answers.unity.com/questions/242648/force-on-character-controller-knockback.html
public class ImpactRecieverComponent
{
    private CharacterController _characterController;
    private Vector3 _impact = Vector3.zero;
    private float _mass;
    private float _lerpSpeed;

    public bool _isColliding = false;

    public ImpactRecieverComponent(CharacterController characterController, float mass, float lerpSpeed = 5f)
    {
        _characterController = characterController;
        _mass = mass;
        _lerpSpeed = lerpSpeed;
    }

    public void AddImpact(Vector3 direction, float force)
    {
        direction.Normalize();
        if (direction.y < 0)
        {
            direction.y *= -1f;
        }

        _impact += direction * force * _mass;
        _isColliding = true;
    }

    public void Update()
    {
        if (_impact.sqrMagnitude > Mathf.Pow(0.2f, 2f))
        {
            _characterController.Move(_impact * Time.deltaTime);
            _impact = Vector3.Lerp(_impact, Vector3.zero, _lerpSpeed * Time.deltaTime);
            return;
        }
        _isColliding = false;
    }
}
