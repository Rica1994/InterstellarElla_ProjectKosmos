using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class SpeederMovement : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;

    [SerializeField] private float _speedForward = 50f;
    [SerializeField] private float _speedSideways = 15f;

    [SerializeField] private float _boostSpeedMultiplier = 2f;
    [SerializeField] private float _boostJumpMultiplier = 2f;
    [SerializeField] private float _boostDuration = 3f;

    [SerializeField] private float _jumpHeight = 8f;
    [SerializeField] private float _gravityValue = -9.81f;

    private Vector2 _input;
    private float _yVelocity = 0f;
    private bool _isJumping = false;
    private bool _isBoosting = false;

    private void Awake()
    {
        // Fixes character controller not grounded bug
        _characterController.minMoveDistance = 0f;
    }

    private void Update()
    {
        // !!Keep this execution order!!
        bool isGrounded = _characterController.isGrounded;
        Move();
        Jump(isGrounded);
    }

    private void Move()
    {
        float boostMultiplier = _isBoosting ? _boostSpeedMultiplier : 1f;

        Vector3 move = new Vector3(_input.x * _speedSideways, 0, 1f * _speedForward);
        _characterController.Move(move * boostMultiplier * Time.deltaTime);
    }

    private void Jump(bool isGrounded)
    {
        if (isGrounded)
        {
            if (_yVelocity < 0) _yVelocity = 0f;
            if (_isJumping)
            {
                ForceJump();
            }
        }
        _isJumping = false;

        _yVelocity += _gravityValue * Time.deltaTime;
        _characterController.Move(new Vector3(0f, _yVelocity * Time.deltaTime, 0f));
    }

    /// <summary>
    /// Forces the player to jump even if they are not grounded or the jumpinput has been pressed.
    /// Can cause the player to jump even if they are already high up in the air!!
    /// </summary>
    public void ForceJump()
    {
        float boostMultiplier = _isBoosting ? _boostJumpMultiplier : 1f;
        _yVelocity += Mathf.Sqrt(-2.0f * _jumpHeight * _gravityValue * boostMultiplier);
    }

    public void Boost()
    {
        _isBoosting = true;
        StopCoroutine(BoostTimer().ToString());
        StartCoroutine(BoostTimer());
    }

    private IEnumerator BoostTimer()
    {
        yield return new WaitForSeconds(_boostDuration);
        _isBoosting = false;
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
}
