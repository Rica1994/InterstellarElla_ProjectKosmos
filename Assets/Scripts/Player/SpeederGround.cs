using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Serialization;

public class SpeederGround : PlayerController
{
    [Header("Speed")]
    [SerializeField] private Vector3 _moveDirection = new Vector3(0f, 0f, 1f);

    [SerializeField] private float _speedForward = 50f;
    [SerializeField, Range(0.1f, 0.5f)] private float _tiltSpeedUpMultiplier = 0.3f;
    [SerializeField] private float _startSidewaySpeed = 20.0f;
    [SerializeField] private float _speedSideways = 15f;
    [SerializeField] private float _startSideWaySpeedAcceleration = 5.0f;

    [SerializeField] private float _sidewaysAcceleration = 5.0f;
    //public static float SpeedForward;

    [Header("Maximum Input ranges joystick")]
    [SerializeField,] private float _forwardAngleRange = 120f;

    [SerializeField] private float _horizontalAngleRange = 60f;


    [Header("Boost")]
    [SerializeField, Range(1.0f, 3.0f)] private float _boostSpeedMultiplier = 2f;

    [SerializeField, Range(1.0f, 3.0f)] private float _boostJumpMultiplier = 2f;
    [SerializeField] private float _boostDuration = 2f;

    [Header("Jump & Gravity")]
    [SerializeField] private float _jumpHeight = 8f;

    [SerializeField] private float _gravityValue = -9.81f;


    [FormerlySerializedAs("_landingBounceValueFactor")] [SerializeField]
    private float _bounceFactor = 0.5f;

    [SerializeField] private float _minimumSpeedToBounce = 30.0f;

    [Header("Hover")]
    [SerializeField]
    private float _hoverDisplacement = 0.5f;

    [SerializeField]
    private float _upDownSpeed = 1.0f;

    [Header("Knockback")]
    [SerializeField] private float _knockbackDuration = 3f;

    [SerializeField, Range(0.0f, 1.0f)] private float _knockbackMultiplier = .3f;

    private CharacterController _characterController;
    private Vector3 _rightVector;
    private Vector2 _input;
    private float _yVelocity = 0f;
    private float _xVelocity = 0f;
    private float _zVelocity = 0f;

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
    private SpeederGroundHoveringComponent _hoveringComponent;

    private MultiplierTimerComponent _speedBoostComponent;
    private MultiplierTimerComponent _jumpBoostComponent;

    private Vector3 _lastPosition;
    private Vector3 _velocity;
    private float _slopeYVelocity;

    private LayerMask _playerLayerMask;

    private float _hoverMovement = 1.0f;

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
        _playerLayerMask = ServiceLocator.Instance.GetService<GameManager>().PlayerLayermask;
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
        _hoveringComponent = new SpeederGroundHoveringComponent(_visual, _characterController);

        _speedBoostComponent = new MultiplierTimerComponent(_boostDuration, _boostSpeedMultiplier, true, 2f, true, 1f);
        _jumpBoostComponent = new MultiplierTimerComponent(_boostDuration, _boostJumpMultiplier, true, 2f, true, 1f);
        _knockbackComponent =
            new MultiplierTimerComponent(_knockbackDuration, 0.0f, _knockbackMultiplier, true, 1f, true, 1f);

        _moveDirection.Normalize();
        transform.forward = _moveDirection;
        _rightVector = Vector3.Cross(_moveDirection, Vector3.up);
    }

    private bool IsGrounded()
    {
        var ray = new Ray(transform.position, Vector3.down);

        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 2f, Color.red);
        return Physics.Raycast(ray, out RaycastHit hitInfo, 2f, ~_playerLayerMask);
    }

    public override void UpdateController()
    {
        base.UpdateController();

        // !!Keep this execution order!!

        bool wasGrounded = _isGrounded;
        var lastYVelocity = _yVelocity;
        _isGrounded = IsGrounded();

        Debug.Log("Is Grounded: " + _isGrounded);

        // Apply landing
        if (wasGrounded == false && wasGrounded != _isGrounded)
        {
            Land(lastYVelocity);
        }

        FakeGroundedTimer();

        _speedBoostComponent.Update();
        _jumpBoostComponent.Update();
        _knockbackComponent.Update();

        Jump();
        ApplyGravity();

        Move();


        _hoveringComponent.UpdateHovering(_upDownSpeed, _hoverDisplacement);
    }

    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 2f, ~_playerLayerMask))
        {
            var slopeDirection = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeDirection * velocity;


            if (adjustedVelocity.y < 0)
            {
                // Debug.Log("returning slope vel");
                return adjustedVelocity;
            }
        }

        return velocity;
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

    private void Move()
    { 
        float inputX = _input.x;
        float inputY = _input.y;
  
        var direction = _moveDirection * 1f + _rightVector * -_input.x;

        if (_xVelocity <= _startSidewaySpeed - 0.1f)
        {
            _xVelocity =
                Mathf.Clamp(_xVelocity + (_startSidewaySpeed * _startSideWaySpeedAcceleration * Time.deltaTime), 0.0f,
                    _startSidewaySpeed) *
                Mathf.Abs(inputX);
        }
        else
        {
            _xVelocity =
                Mathf.Clamp(_xVelocity + (_sidewaysAcceleration * Time.deltaTime), _startSidewaySpeed, _speedSideways) *
                Mathf.Abs(inputX);
        }

        
        _zVelocity =
            (_speedForward * (1 + Mathf.Clamp(inputY, -_tiltSpeedUpMultiplier, _tiltSpeedUpMultiplier)) *
             _speedBoostComponent.Multiplier) * _knockbackComponent.Multiplier;

        var lastSlopeVelocity = _slopeYVelocity;
        Vector3 speed = new Vector3(_xVelocity, _yVelocity, _zVelocity);
        Vector3 move = new Vector3(direction.x * speed.x, 0.0f, direction.z * speed.z);
        move = AdjustVelocityToSlope(move);
        _slopeYVelocity = move.y;
        move.y += speed.y;

        // Add slope Y-velocity when transitioning from a slope
        if (!_isGrounded)
        {
            if (_yVelocity < 0f) // Only add slope velocity if the current Y-velocity is negative (downward)
            {
                _yVelocity += lastSlopeVelocity; // Add the slope Y-velocity
            }
        }

        _characterController.Move(move * Time.deltaTime);
        // _moveComponent.Move(_characterController, slopeVelocity, speed);
    }

    private void Land(float landingVelocity)
    {
        landingVelocity = Mathf.Abs(landingVelocity);
        if (landingVelocity > _minimumSpeedToBounce)
        {
            _yVelocity = landingVelocity * _bounceFactor;
        }
    }

    private void Jump()
    {
        if (_isJumping == true)
        {
            if (_hasJumped == false)
            {
                _yVelocity = 0.0f;
                _hasJumped = true;
            }

            _jumpComponent.Jump(ref _yVelocity, _gravityValue, _jumpHeight * _jumpBoostComponent.Multiplier);
        }

        _isJumping = false;
    }

    private void Hover()
    {
    }

    private void ApplyGravity()
    {
        if (_isGrounded)
        {
            _slopeYVelocity = _yVelocity;
        }

        _gravityComponent.ApplyGravity(_characterController, ref _yVelocity,
            _gravityValue /** (1 + (Mathf.Clamp01(_input.y) * _tiltMultiplier))*/, _isGrounded);
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
        Debug.LogWarning("Input X: " + _input.x + "\nInput Y: " + _input.y);
    }

    private void OnJumpInput()
    {
        if (_isGroundedFake == true && _knockbackComponent.IsTicking == false)
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

        // Rotate towards Target
        var rot = Quaternion.FromToRotation(_visual.transform.forward,
            _target.transform.position - _visual.transform.position) * _visual.transform.rotation;
        _visual.transform.rotation = Quaternion.Lerp(_visual.transform.rotation, rot, 0.2f);

        //Rotate towards Normal
        RaycastHit hitInfo = new RaycastHit();

        // floor
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 2.0f, ~_playerLayerMask))
        {
            var angle = Vector3.Angle(Vector3.up, hitInfo.normal);
            //Debug.Log("Angle: " + angle + "\nOn Object: " + hitInfo.transform.name);
            if (Mathf.Abs(angle) > 10f)
            {
                // Calculate the rotation needed from the up vector to the normal
                rot = Quaternion.FromToRotation(_visual.transform.up, hitInfo.normal) * _visual.transform.rotation;
                _visual.transform.rotation = Quaternion.Lerp(_visual.transform.rotation, rot, 0.1f);
            }
        }

        // Rotates along the the forward axis according to the left of right velocity
        var rotationalFactor = Mathf.Clamp(_velocityNormalized.x, -1.0f, 1.0f);
        rot = Quaternion.Euler(0.0f, 0.0f, -rotationalFactor * 80.0f);
        _visual.transform.rotation = Quaternion.Lerp(_visual.transform.rotation, rot, 0.1f);

        // Rotate along x axis according to the vertical input
        rotationalFactor = Mathf.Clamp(_input.y, -1.0f, 1.0f);
        rot = Quaternion.Euler(rotationalFactor * 90.0f, 0.0f, 0.0f);
        _visual.transform.rotation = Quaternion.Lerp(_visual.transform.rotation, rot, 0.1f);
    }

    public void SetJumpMultiplierComponent(MultiplierTimerComponent multiplierTimerComponent)
    {
        _jumpBoostComponent = multiplierTimerComponent;
    }

    public void SetSpeedMultiplierComponent(MultiplierTimerComponent multiplierTimerComponent)
    {
        _speedBoostComponent = multiplierTimerComponent;
    }

    public void SetKnockBackMultiplierComponent(MultiplierTimerComponent multiplierTimerComponent)
    {
        _knockbackComponent = multiplierTimerComponent;
    }
}