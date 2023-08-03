using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.Serialization;

public class SpeederGround : PlayerController
{
    [Header("Speed")]
    [SerializeField] private Vector3 _moveDirection = new Vector3(0f, 0f, 1f);

    [SerializeField] public float _speedForward = 50f;    
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
    public bool HasJumped = false;
    private bool _isGrounded = false;
    public bool IsGroundedFake = false;
    private bool _isApplicationQuitting = false;

    private MoveComponent _moveComponent;
    private JumpComponent _jumpComponent;
    private GravityComponent _gravityComponent;
    private SpeederGroundHoveringComponent _hoveringComponent;
    
    private MultiplierTimerComponent _speedBoostComponent;
    private MultiplierTimerComponent _jumpBoostComponent;
    private MultiplierTimerComponent _knockbackComponent;

    private Vector3 _lastPosition;
    private Vector3 _velocity;
    
    private float _hoverMovement = 1.0f;

    [SerializeField]
    private Vector3 _velocityNormalized;

    private float _xRotation = 0;

    [SerializeField]
    private Transform _visual;

    
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private LayerMask _playerLayerMask;


    [Header("Sounds")]
    [SerializeField]
    private AudioElement _hoveringAmbienceSound;

    public float additionalDistance = 0.1f;

    [SerializeField]
    private Transform _trail;

    //private void OnValidate()
    //{
    //    SpeedForward = _speedForward;
    //}

    private void Start()
    {
        _lastPosition = transform.position;

        // Hovering ambience sound
        ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_hoveringAmbienceSound, true);
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
        _knockbackComponent = new MultiplierTimerComponent(_knockbackDuration, _knockbackMultiplier, true, false);

        _moveDirection.Normalize();
        transform.forward = _moveDirection;
        _rightVector = Vector3.Cross(_moveDirection, Vector3.up);
    }

    public override void UpdateController()
    {
        base.UpdateController();

        // !!Keep this execution order!!

        bool wasGrounded = _isGrounded;
        var lastYVelocity = _yVelocity;
        _isGrounded = _characterController.isGrounded;

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
        
        Move();
        
        Jump();

        ApplyGravity();
        
        _hoveringComponent.UpdateHovering(_upDownSpeed, _hoverDisplacement);
    }

    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 2f))
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
        IsGroundedFake = true;

        // if I have just jumped -> fake grounded = false until _isGrounded happens
        if (HasJumped == true)
        {
            IsGroundedFake = false;

            // the moment we touch the floor...
            if (_isGrounded == true)
            {
                IsGroundedFake = true;
                HasJumped = false;
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
                IsGroundedFake = false;
            }
        }
        else // if CC is grounded, this is grounded
        {
            _fakeGroundedTimer = 0;

            IsGroundedFake = true;
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
       // float angle = Mathf.Atan2(_input.y, _input.x) * Mathf.Rad2Deg;
       // if (angle < 0.0f) angle += 360.0f;
        
        float inputX = _input.x;
        float inputY = _input.y;

        /*if (angle >= 90 - _forwardAngleRange && angle <= 90 + _forwardAngleRange)
        {
            inputY = Mathf.Sign(_input.y);
        }
//
        if ((angle <= _horizontalAngleRange && angle >= -_horizontalAngleRange) || (angle > 180.0f - _horizontalAngleRange && angle <= 180) || (angle >= -180 && angle < -180 + _horizontalAngleRange))
        {
            inputX = Mathf.Sign(_input.x);
        }*/
        
        var direction = _moveDirection * 1f + _rightVector * -_input.x;
        Vector3 slopeVelocity = AdjustVelocityToSlope(direction);

        if (_xVelocity <= _startSidewaySpeed - 0.1f)
        {
            _xVelocity = Mathf.Clamp(_xVelocity + (_startSidewaySpeed * _startSideWaySpeedAcceleration * Time.deltaTime), 0.0f, _startSidewaySpeed) *
                         Mathf.Abs(inputX);
        }
        else
        {
            _xVelocity =
                Mathf.Clamp(_xVelocity + (_sidewaysAcceleration * Time.deltaTime), _startSidewaySpeed, _speedSideways) *
                Mathf.Abs(inputX);
        }
        
 //       Debug.Log("2  X " + inputX + "\n XVelocity " + _xVelocity);

        
        _zVelocity = _speedForward * (1 + Mathf.Clamp(inputY, -_tiltSpeedUpMultiplier, _tiltSpeedUpMultiplier));
        Vector3 speed = new Vector3(_xVelocity, _speedForward, _zVelocity * _knockbackComponent.Multiplier) *
                        _speedBoostComponent.Multiplier;

        // OG
        //Vector3 speed = new Vector3(
        //    _xVelocity, 
        //    0, 
        //    _speedForward * /*Mathf.Clamp((_input.y * _tiltMultiplier), 0.5f, _tiltMultiplier)*  */ _knockbackComponent.Multiplier)
        //    * _speedBoostComponent.Multiplier;

        _moveComponent.Move(_characterController, slopeVelocity, speed);
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
            if (HasJumped == false)
            {
                _yVelocity = 0.0f;
                HasJumped = true;
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
        if (IsGroundedFake == true)
        {
            _isJumping = true;
        }
    }

    private void FixedUpdate()
    {
        RaycastDownwards();

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

    private void RaycastDownwards()
    {
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

        float raycastLength = capsuleCollider.height * 0.5f + additionalDistance;
        Vector3 raycastOrigin = transform.position;
        Vector3 raycastDirection = Vector3.down;

        Ray ray = new Ray(raycastOrigin, raycastDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastLength))
        {
            // Raycast hit something
            Vector3 hitPosition = hit.point;

            // Do something with the hit position
            Debug.Log("Hit position: " + hitPosition);
            _trail.position = hitPosition;
        }

    }
}