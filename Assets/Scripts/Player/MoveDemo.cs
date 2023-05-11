using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveDemo : MonoBehaviour
{
    [SerializeField] private Canvas Player;
    [SerializeField] private Canvas Ui;

    private bool _isBoosting = false;
    private bool _isPaused = false;
    private Vector2 _velocity;

    void OnEnable()
    {
        // Subscribe to player input events
        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;

        playerInput.Move.performed += x => Rotate(x.ReadValue<Vector2>());
        playerInput.Move.canceled += x => Rotate(x.ReadValue<Vector2>());
        playerInput.Action.started += x => Click();
        playerInput.Action.canceled += x => Click();
    }

    // TODO: subscribe somwhere else or cache value of servicelocator?
    private void OnDisable()
    {
        // Unsubscribe to player input events
        var playerInput = ServiceLocator.Instance.InputManager.PlayerInput;

        playerInput.Move.performed -= x => Rotate(x.ReadValue<Vector2>());
        playerInput.Move.canceled -= x => Rotate(x.ReadValue<Vector2>());
        playerInput.Action.performed -= x => Click();
        playerInput.Action.canceled -= x => Click();
    }

    private void Rotate(Vector2 input)
    {
        _velocity = input;
        Debug.Log("Move " + _velocity);
    }

    private void Click()
    {
        _isBoosting = !_isBoosting;
    }

    public void Pause()
    {
        _isPaused = !_isPaused;
        Ui.gameObject.SetActive(_isPaused);
        ServiceLocator.Instance.InputManager.EnableUiInput(_isPaused);
        Player.gameObject.SetActive(!_isPaused);
        ServiceLocator.Instance.InputManager.EnablePlayerInput(_isPaused);

        Debug.Log("Pause");
    }

    private void Update()
    {
        if(_isBoosting) Debug.Log("Click");        
    }
}
