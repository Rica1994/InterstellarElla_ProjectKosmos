using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeederGroundHoveringComponent
{
    public SpeederGroundHoveringComponent(Transform t, CharacterController characterController)
    {
        _hoveringTransform = t;
        _originalLocalPosition = _hoveringTransform.localPosition;
        _characterController = characterController;
    }
    
    [SerializeField]
    private CharacterController _characterController;

    private bool _isGrounded = false;
    private float _currentDisplacement = 0.0f;
    private Vector3 _originalLocalPosition;
    private bool _shouldHover = false;
    private Transform _hoveringTransform;
    private float _angle = 0.0f;
    
    public void UpdateHovering(float upDownSpeed, float displacement)
    {
        _isGrounded = _characterController.isGrounded;
        
        if (_isGrounded && _shouldHover)
        {
            Hover(_hoveringTransform, upDownSpeed, displacement);
        }
        else if (_shouldHover == false)
        {
            var localPos = _hoveringTransform.localPosition;
            _hoveringTransform.localPosition = Vector3.Lerp(localPos, _originalLocalPosition, 1 * Time.deltaTime);
            _shouldHover = Mathf.Approximately(_hoveringTransform.localPosition.sqrMagnitude,
                _originalLocalPosition.sqrMagnitude);
        }
    }

    private void Hover(Transform t, float upDownSpeed, float hoverDisplacement)
    {
        _angle += Time.deltaTime * upDownSpeed;
        var result = Mathf.Cos(_angle);

        /*if (result > 0.0f) result = 1.0f;
        else result = -1.0f;*/
        
        _currentDisplacement = result * hoverDisplacement * Time.deltaTime;
        t.localPosition += new Vector3(0, _currentDisplacement, 0);
    }
}