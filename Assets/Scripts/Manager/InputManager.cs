using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Service
{
    private PI_InputElla _input;

    public PI_InputElla.PlayerActions PlayerInput => _input.Player;
    public PI_InputElla.UIActions UiInput => _input.UI;

    protected override void Awake()
    {
        _input = new PI_InputElla();
        base.Awake();
        //PlayerInput.Move.performed += x => 
    }

    //private void OnEnable()
    //{
    //    PlayerInput.Enable();
    //}
}
