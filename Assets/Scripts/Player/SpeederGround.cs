using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeederGround : MonoBehaviour
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
        Jump();
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

        // Calculate and save player velocity
        _velocity = (transform.position - _previousPosition) / Time.deltaTime;
    }

    private void Move()
    {
        // Input only allowed for left and right (x)
        Vector3 direction = new Vector3(_input.x, 0f, 1f);
        
        Vector3 speed = new Vector3(_speedSideways, 0f, _speedForward) * _speedBoostComponent.BoostMultiplier;

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

    private void ApplyGravity(bool isGrounded)
    {
        _gravityComponent.ApplyGravity(_characterController, ref _yVelocity, _gravityValue, isGrounded);
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
        // Unsubscribe to events
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
