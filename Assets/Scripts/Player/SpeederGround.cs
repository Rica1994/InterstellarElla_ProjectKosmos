using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeederGround : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;

    [Header ("Speed")]
    [SerializeField] private float _speedForward = 50f;
    [SerializeField] private float _speedSideways = 15f;

    [Header ("Boost")]
    [SerializeField] private float _boostSpeedMultiplier = 2f;
    [SerializeField] private float _boostJumpMultiplier = 2f;
    [SerializeField] private float _boostDuration = 3f;

    [Header ("Jump & Gravity & Knockback")]
    [SerializeField] private float _jumpHeight = 8f;
    [SerializeField] private float _gravityValue = -9.81f;
    [SerializeField] private float _knockbackForce = 20f;

    private Vector3 _previousPosition;
    private Vector3 _velocity;
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

    public void Collide()
    {
        var velocity = _velocity.normalized;
        var knockbackDirection = new Vector3(-velocity.x, 0f, -1f);
        knockbackDirection.Normalize();
        _impactRecieverComponent.AddImpact(knockbackDirection, _knockbackForce);
    }

    private void Awake()
    {
        // Fixes character controller not grounded bug
        _characterController.minMoveDistance = 0f;

        // Initialize player components
        _moveComponent = new MoveComponent();
        _jumpComponent = new JumpComponent();
        _gravityComponent = new GravityComponent();
        _impactRecieverComponent = new ImpactRecieverComponent(_characterController, 2f, 4f);
        _previousPosition = gameObject.transform.position;
    }

    private void Update()
    {
        // Remember previous location
        _previousPosition = gameObject.transform.position;

        // !!Keep this execution order!!
        bool isGrounded = _characterController.isGrounded;
        if (!_impactRecieverComponent._isColliding)
        {
            Move();
            if (isGrounded) Jump();
        }
        ApplyGravity(isGrounded);
        _impactRecieverComponent.Update();

        // Calculate player velocity
        _velocity = (transform.position - _previousPosition) / Time.deltaTime;
    }

    private void Move()
    {
        // Add boost multiplier to move when nessescary
        float boostMultiplier = _isBoosting ? _boostSpeedMultiplier : 1f;
        _moveComponent.Move(_characterController, new Vector3(_input.x, 0f, 1f), new Vector3(_speedSideways, 0f, _speedForward) * boostMultiplier);
    }

    private void Jump()
    {
        // Add boost multiplier to jump when nessescary
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
