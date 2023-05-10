using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Service
{
    private PI_InputElla _input;

    public PI_InputElla.PlayerActions PlayerInput => _input.Player;

    protected override void Awake()
    {
        _input = new PI_InputElla();
        base.Awake();
    }

    public void EnablePlayerInput(bool shouldEnable)
    {
        if (shouldEnable)
        {
            PlayerInput.Enable();
        }
        else
        {
            PlayerInput.Disable();
        }
    }

    public void EnableUiInput(bool shouldEnable)
    {
        if (shouldEnable)
        {
            _input.UI.Enable();
        }
        else
        {
            _input.UI.Disable();
        }
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }
}
