using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class SpeederGround : PlayerController
{
    [Header("Speed")]
    [SerializeField] private Vector3 _moveDirection = new Vector3(0f, 0f, 1f); 
    [SerializeField] private float _speedForward = 50f;
    [SerializeField] private float _speedSideways = 15f;

    //public static float SpeedForward;
    
    [Header ("Boost")]
    [SerializeField, Range(1.0f, 3.0f)] private float _boostSpeedMultiplier = 2f;
    [SerializeField, Range(1.0f, 3.0f)] private float _boostJumpMultiplier = 2f;
    [SerializeField] private float _boostDuration = 2f;

    [Header ("Jump & Gravity")]
    [SerializeField] private float _jumpHeight = 8f;
    [SerializeField] private float _gravityValue = -9.81f;

    [Header("Knockback")]
    [SerializeField] private float _knockbackDuration = 3f;
    [SerializeField, Range(0.0f, 1.0f)] private float _knockbackMultiplier = .3f;

    private CharacterController _characterController;
    private Vector3 _rightVector;
    private Vector2 _input;
    private float _yVelocity = 0f;
    private float _fakeGroundedTimer;
    [SerializeField] private float _fakeGroundedTimeLimit = 0.25f;
    private bool _isJumping = false;
    private bool _hasJumped = false;
    private bool _isGrounded = false;
    private bool _isGroundedFake = false;
    private bool _isApplicationQuitting = false;

    private MoveComponent _moveComponent;
    private JumpComponent _jumpComponent;
    private GravityComponent _gravityComponent;
    private MultiplierTimerComponent _speedBoostComponent;
    private MultiplierTimerComponent _jumpBoostComponent;
    private MultiplierTimerComponent _knockbackComponent;

    private Vector3 _lastPosition;
    private Vector3 _velocity;
    [SerializeField]
    private Vector3 _velocityNormalized;
    private float _xRotation = 0;
    [SerializeField]
    private Transform _visual;
    [SerializeField]
    private Transform _target;

    //private void OnValidate()
    //{
    //    SpeedForward = _speedForward;
    //}

    private void Start()
    {
        _lastPosition = transform.position;
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
        
        _speedBoostComponent = new MultiplierTimerComponent(_boostDuration, _boostSpeedMultiplier, true, 2f, true, 1f);
        _jumpBoostComponent = new MultiplierTimerComponent(_boostDuration, _boostJumpMultiplier, true, 2f, true, 1f);
        _knockbackComponent = new MultiplierTimerComponent(_knockbackDuration, _knockbackMultiplier, true, false);

        _moveDirection.Normalize();
        transform.forward = _moveDirection;
        _rightVector = Vector3.Cross(_moveDirection, Vector3.up);
    }

    public override void UpdateController()
    {
        base.UpdateController();

        // !!Keep this execution order!!
        _isGrounded = _characterController.isGrounded;

        FakeGroundedTimer();

        _speedBoostComponent.Update();
        _jumpBoostComponent.Update();
        _knockbackComponent.Update();

        Move();
        Jump();
        ApplyGravity();
    }
    private void FakeGroundedTimer()
    {
        // default is grounded
        _isGroundedFake = true;

        // if I have just jumped -> fake grounded = false until _isGrounded happens
        if (_hasJumped == true)
        {
            _isGroundedFake = false;

            // the moment we touch the floor...
            if (_isGrounded == true)
            {
                _isGroundedFake = true;
                _hasJumped = false;
            }

            // don't need to go beyond this line of code if we are in here
            return;
        }


        // if timer exceeds limit, we are not grounded
        if (_isGrounded == false)
        {
            _fakeGroundedTimer += Time.deltaTime;

            if (_fakeGroundedTimer >= _fakeGroundedTimeLimit)
            {
                _isGroundedFake = false;
            }
        }
        else // if CC is grounded, this is grounded
        {
            _fakeGroundedTimer = 0;

            _isGroundedFake = true;
        }
    }

    public void BoostSpeed()
    {
        _speedBoostComponent.Activate();
    }

    public void BoostJump()
    {
        _jumpBoostComponent.Activate();
    }

    public void ForceJump()
    {
        _isJumping = true;
    }

    public override void Collide()
    {
        _knockbackComponent.Activate();
    }

    private void Move()
    {
        var direction = _moveDirection * 1f + _rightVector * -_input.x;
        //Vector3 direction = new Vector3(_input.x, 0f, 1f);
        Vector3 speed = new Vector3(_speedSideways, 0f, _speedForward * _knockbackComponent.Multiplier) * _speedBoostComponent.Multiplier;
        print(direction);
        _moveComponent.Move(_characterController, direction, speed);
    }

    private void Jump()
    {
        if (_isJumping == true)
        {
            _jumpComponent.Jump(ref _yVelocity, _gravityValue, _jumpHeight * _jumpBoostComponent.Multiplier);

            _hasJumped = true;
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
        if (_isGroundedFake == true)
        {
            _isJumping = true;
        }
    }

    private void FixedUpdate()
    {
        // Calculate normalized velocity
        _velocity = (transform.position - _lastPosition) / Time.deltaTime;
        _velocityNormalized = _velocity.normalized;
        _lastPosition = transform.position;

        _target.transform.localPosition = new Vector3(_velocityNormalized.x * 2, 0, 5.14f);

        _visual.transform.LookAt(_target.transform.position);
    }

    public void SetJumpMultiplierComponent(MultiplierTimerComponent multiplierTimerComponent)
    {
        _jumpBoostComponent = multiplierTimerComponent;
    }
    
    public void SetSpeedMultiplierComponent(MultiplierTimerComponent multiplierTimerComponent)
    {
        _speedBoostComponent = multiplierTimerComponent;
    }
}
