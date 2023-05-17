using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class SpeederGround : PlayerController
{
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

    private CharacterController _characterController;
    private Vector3 _previousPosition;
    private Vector3 _velocity;
    private Vector2 _input;
    private float _yVelocity = 0f;
    private bool _isJumping = false;
    private bool _isGrounded = false;
    private bool _isApplicationQuitting = false;

    private MoveComponent _moveComponent;
    private JumpComponent _jumpComponent;
    private GravityComponent _gravityComponent;
    private ImpactRecieverComponent _impactRecieverComponent;
    private BoostComponent _speedBoostComponent;
    private BoostComponent _jumpBoostComponent;

    public void BoostSpeed()
    {
        _speedBoostComponent.Boost();
    }

    public void BoostJump()
    {
        _jumpBoostComponent.Boost();
    }

    public void ForceJump()
    {
        _isJumping = true;
    }

    public void Collide()
    {
        // Knockback backwards and whatever velocity on x
        var velocity = _velocity.normalized;
        Vector3 knockbackDirection = new Vector3(-velocity.x, 0f, -1f);
        _impactRecieverComponent.AddImpact(knockbackDirection.normalized, _knockbackForce);
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        // Fixes character controller not grounded bug
        _characterController.minMoveDistance = 0f;

        // Initialize player components
        _moveComponent = new MoveComponent();
        _jumpComponent = new JumpComponent();
        _gravityComponent = new GravityComponent();
        _impactRecieverComponent = new ImpactRecieverComponent(_characterController, 2f, 4f);
        _speedBoostComponent = new BoostComponent(_boostDuration, _boostSpeedMultiplier);
        _jumpBoostComponent = new BoostComponent(_boostDuration, _boostJumpMultiplier);

        // Save previous position for velocity calculations
        _previousPosition = gameObject.transform.position;
    }

    public override void UpdateController()
    {
        base.UpdateController();

        // Remember previous location
        _previousPosition = gameObject.transform.position;

        // !!Keep this execution order!!
        _isGrounded = _characterController.isGrounded;
        Move();
        Jump();
        ApplyGravity();
        _impactRecieverComponent.Update();

        // Calculate and save player velocity
        _velocity = (transform.position - _previousPosition) / Time.deltaTime;
    }

    private void Move()
    {
        // Adjust forward and sideways speed while colliding
        float speedForward = _speedForward;
        float speedSideways = _speedSideways;
        if (_impactRecieverComponent.IsColliding)
        {
            speedForward = 0f;
            speedSideways /= 2f;
        }

        // Input only allowed for left and right (x)
        Vector3 direction = new Vector3(_input.x, 0f, 1f);
        
        Vector3 speed = new Vector3(speedSideways, 0f, speedForward) * _speedBoostComponent.BoostMultiplier;

        _moveComponent.Move(_characterController, direction, speed);
    }

    private void Jump()
    {
        if (_isJumping)
        {
            float boostMultiplier = _jumpBoostComponent.BoostMultiplier;
            _jumpComponent.Jump(ref _yVelocity, _gravityValue, _jumpHeight * boostMultiplier);
        }
        _isJumping = false;
    }

    private void ApplyGravity()
    {
        _gravityComponent.ApplyGravity(_characterController, ref _yVelocity, _gravityValue, _isGrounded);
    }

    private void OnEnable()
    {
        // Subscribe to events
        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;
        playerInput.Move.performed += x => OnMoveInput(x.ReadValue<Vector2>());
        playerInput.Move.canceled += x => OnMoveInput(x.ReadValue<Vector2>());
        playerInput.Action.started += x => OnJumpInput();
    }

    private void OnDisable()
    {
        if (_isApplicationQuitting)
        {
            return;
        }

        // If service locator does not exist anymore, there is no need to unsubscribe:
        // References of this script will not be saved by the input manager anyway since this is also destroyed
        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;

        // Unsubscribe to events
        playerInput.Move.performed -= x => OnMoveInput(x.ReadValue<Vector2>());
        playerInput.Move.canceled -= x => OnMoveInput(x.ReadValue<Vector2>());
        playerInput.Action.started -= x => OnJumpInput();
    }

    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }

    private void OnMoveInput(Vector2 input)
    {
        _input = input;
    }

    private void OnJumpInput()
    {
        if (_isGrounded)
        {
            _isJumping = true;
        }
    }
}
