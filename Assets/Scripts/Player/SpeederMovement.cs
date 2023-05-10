using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class SpeederMovement : MonoBehaviour
{
    [SerializeField] private float _speedForward;
    [SerializeField] private float _speedSideways;
    [SerializeField] private CharacterController _characterController;
    private Vector2 _input;

    [SerializeField] private float _gravityValue = -9.81f;
    private bool _isJumping = false;
    private float _yVelocity = 0f;
    [SerializeField] private float _jumpHeight = 1f;

    private void Awake()
    {
        // Fixes character controller not grounded bug
        _characterController.minMoveDistance = 0f;
    }

    private void OnEnable()
    {
        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;
        playerInput.Move.performed += x => OnMoveInput(x.ReadValue<Vector2>());
        playerInput.Move.canceled += x => OnMoveInput(x.ReadValue<Vector2>());
        playerInput.Action.started += x => OnJumpInput();
    }

    private void OnDisable()
    {
        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;
        playerInput.Move.performed -= x => OnMoveInput(x.ReadValue<Vector2>());
        playerInput.Move.canceled -= x => OnMoveInput(x.ReadValue<Vector2>());
        playerInput.Action.started -= x => OnJumpInput();
    }

    private void OnMoveInput(Vector2 input)
    {
        _input = input;
    }

    private void OnJumpInput()
    {
        _isJumping = true;
    }

    private void Update()
    {
        bool isGrounded = _characterController.isGrounded;
        Move();
        Jump(isGrounded);
    }

    private void Move()
    {
        Vector3 move = new Vector3(_input.x * _speedSideways, 0, 1f * _speedForward);
        _characterController.Move(move * Time.deltaTime);
    }

    private void Jump(bool isGrounded)
    {
        if (isGrounded)
        {
            if (_yVelocity < 0) _yVelocity = 0f;
            if (_isJumping) _yVelocity += Mathf.Sqrt(_jumpHeight * -2.0f * _gravityValue);
        }
        _isJumping = false;

        _yVelocity += _gravityValue * Time.deltaTime;
        _characterController.Move(new Vector3(0f, _yVelocity * Time.deltaTime, 0f));
    }
}
