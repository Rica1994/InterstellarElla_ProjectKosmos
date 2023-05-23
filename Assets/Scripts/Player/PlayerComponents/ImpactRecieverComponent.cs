using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference: https://answers.unity.com/questions/242648/force-on-character-controller-knockback.html
public class ImpactRecieverComponent
{
    public delegate void InpactRecieverContext();
    public event InpactRecieverContext OnKnockbackEnded;

    private CharacterController _characterController;
    private Vector3 _impact = Vector3.zero;
    private readonly float _mass;
    private readonly float _lerpSpeed;

    private bool _isColliding = false;
    public bool IsColliding => _isColliding;
    public Vector3 Destination => _impact / _lerpSpeed;

    public ImpactRecieverComponent(CharacterController characterController, float mass, float lerpSpeed = 5f)
    {
        _characterController = characterController;
        _mass = mass;
        _lerpSpeed = lerpSpeed;
    }

    public void AddImpact(Vector3 direction, float force, bool canBeDownwards = false)
    {
        direction.Normalize();
        if (!canBeDownwards && direction.y < 0)
        {
            direction.y *= -1f;
        }

        _impact += direction * force * _mass;
        _isColliding = true;
    }

    public void Update()
    {
        // Only update while colliding
        if (!_isColliding) return;

        // Check impact leftover
        if (_impact.sqrMagnitude > Mathf.Pow(0.2f, 2f))
        {
            _characterController.Move(_impact * Time.deltaTime);
            _impact = Vector3.Lerp(_impact, Vector3.zero, _lerpSpeed * Time.deltaTime);
            return;
        }

        // Notify when knockback ended
        _isColliding = false;
        OnKnockbackEnded?.Invoke();
    }
}
