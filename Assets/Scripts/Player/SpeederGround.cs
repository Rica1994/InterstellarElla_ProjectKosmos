using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class SpeederGround : MonoBehaviour
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

    private MoveComponent _moveComponent;
    private JumpComponent _jumpComponent;
    private GravityComponent _gravityComponent;
    private ImpactRecieverComponent _impactRecieverComponent;

    public void Boost()
    {
        _isBoosting = true;
        StopCoroutine(BoostTimer().ToString());
        StartCoroutine(BoostTimer());
    }

    public void ForceJump()
    {
        _isJumping = true;
        Jump();
    }

    private void Awake()
    {
        // Fixes character controller not grounded bug
        _characterController.minMoveDistance = 0f;

        _moveComponent = new MoveComponent();
        _jumpComponent = new JumpComponent();
        _gravityComponent = new GravityComponent();
        _impactRecieverComponent = new ImpactRecieverComponent(_characterController, 3f);
    }

    private void Update()
    {
        // !!Keep this execution order!!
        bool isGrounded = _characterController.isGrounded;
        Move();
        if (isGrounded) Jump();
        ApplyGravity(isGrounded);
        _impactRecieverComponent.Update();
    }

    private void Move()
    {
        float boostMultiplier = _isBoosting ? _boostSpeedMultiplier : 1f;
        _moveComponent.Move(_characterController, new Vector3(_input.x, 0f, 1f), new Vector3(_speedSideways, 0f, _speedForward) * boostMultiplier);
    }

    private void Jump()
    {
        float boostMultiplier = _isBoosting ? _boostJumpMultiplier : 1f;
        if (_isJumping) _jumpComponent.Jump(ref _yVelocity, _gravityValue, _jumpHeight * boostMultiplier);
        _isJumping = false;
    }

    private void ApplyGravity(bool isGrounded)
    {
        _gravityComponent.ApplyGravity(_characterController, ref _yVelocity, _gravityValue, isGrounded);
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
