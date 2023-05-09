using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    private bool _isBoosting = false;
    private Vector2 _velocity;

    void Start()
    {
        // Subscribe to player input events
        var playerInput = ServiceLocator.Instance.InputManager.PlayerInput;

        playerInput.Move.performed += x => Rotate(x.ReadValue<Vector2>());
        playerInput.Move.canceled += x => Rotate(x.ReadValue<Vector2>());
        playerInput.Action.performed += x => Click();
        playerInput.Action.canceled += x => Click();
    }

    private void Rotate(Vector2 input)
    {
        _velocity = input;
        Debug.Log("Move " + input);
    }

    private void Click()
    {
        _isBoosting = !_isBoosting;
    }

    private void Update()
    {
        if(_isBoosting) Debug.Log("Click");        
    }
}
