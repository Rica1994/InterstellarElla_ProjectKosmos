using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class SpeederGround : PlayerController
{
    [Header("Speed")]
    public Vector3 moveDirection = new Vector3(0f, 0f, 1f);

    public float speedForward = 50f;
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


    [FormerlySerializedAs("_landingBounceValueFactor")]
    [SerializeField]
    private float _bounceFactor = 0.5f;

    [SerializeField] private float _minimumSpeedToBounce = 30.0f;

    [Header("Hover")]
    [SerializeField]
    private float _hoverDisplacement = 0.5f;

    [SerializeField]
    private float _upDownSpeed = 1.0f;

    private CharacterController _characterController;
    private Vector3 _rightVector;
    private Vector2 _input;
    private float _yVelocity = 0f;
    private float _xVelocity = 0f;
    private float _zVelocity = 0f;

    private Quaternion _standardRotation;
    private float _fakeGroundedTimer;

    [Header("Other")]
    [SerializeField] private float _fakeGroundedTimeLimit = 0.25f;
    private bool _isJumping = false;
    private bool _hasJumped = false;
    private bool _isGrounded = false;
    private bool _isGroundedFake = false;
    [HideInInspector]
    public bool IsGrounded => _isGroundedFake;
    private bool _isApplicationQuitting = false;
    [SerializeField]
    private ParticleSystem _particleDustTrail;

    private MoveComponent _moveComponent;
    private JumpComponent _jumpComponent;
    private GravityComponent _gravityComponent;
    private SpeederGroundHoveringComponent _hoveringComponent;

    private MultiplierTimerComponent _speedBoostComponent;
    private MultiplierTimerComponent _jumpBoostComponent;


    private Vector3 _lastPosition;
    private Vector3 _velocity;

    private LayerMask _playerLayerMask;

    private float _hoverMovement = 1.0f;

    [SerializeField]
    private Vector3 _velocityNormalized;

    private float _xRotation = 0;

    [SerializeField]
    private Transform _visual;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private float _visualLerpSpeed = 2.5f;

    [Header("Auto-jump stuff")]
    public AutoJumpMaster CurrentAutoJumpMaster;
    public Coroutine CCToggleRoutine;
    [SerializeField]
    private Rigidbody _rigidbody;
    public Rigidbody Rigid => _rigidbody;


    [Header("Sounds")]
    [SerializeField] private AudioElement _soundJump;
    [SerializeField] private AudioElement _soundLand;
    [SerializeField] private AudioElement _soundBounce;

    private AudioController _audioController;

    [Header("Exploring settings")]
    [SerializeField]
    private bool _isExploringVersion;

    [Header("Visual Transforms")]
    [SerializeField]
    private Transform _moonscooterTransform;
    [SerializeField]
    private Transform _ellaRyderTransform;

    [SerializeField]
    private TouchButton _speederGroundButton;

    private Vector3 _previousMoonscooterPosition;
    private Vector3 _previousEllaRyderPosition;

    // store a new checkpoint gameobject on here when entering trigger
    // call Gamemanager.respawn if...
    //  the forward distance covered (Mathf.abs) <= threshhold within last 5 seconds (should colliding pause timer ???)
    private float _playerStuckUnknownTimer, _playerStuckMaybeTimer;
    private float _playerStuckUnknownTimeLimit = 2f;
    private float _playerStuckMaybeLimit = 5f;
    private bool _playerMightBeStuck, _playerStuck;
    private float _playerStuckFirstDistance = 20;
    private float _playerStuckSecondDistance = 10;
    private float _playerPositionWorldForward;


    #region Unity Functions

    private void Awake()
    {
        if (_moonscooterTransform != null && _ellaRyderTransform != null)
        {
            _previousMoonscooterPosition = _moonscooterTransform.localPosition;
            _previousEllaRyderPosition = _ellaRyderTransform.localPosition;
        }

        Initialize();
    }

    public void Initialize()
    {
        if (_speederGroundButton != null)
        {
            _speederGroundButton.Pressed += OnPressed;
        }

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

        moveDirection.Normalize();
        transform.forward = moveDirection;
        _rightVector = Vector3.Cross(moveDirection, Vector3.up);

        if (_moonscooterTransform != null && _ellaRyderTransform != null)
        {
            _moonscooterTransform.localPosition = _previousMoonscooterPosition;
            _ellaRyderTransform.localPosition = _previousEllaRyderPosition;

            if (moveDirection.z < 0f)
            {
                _visual.Rotate(0.0f, 180.0f, 0.0f);
                _moonscooterTransform.rotation = Quaternion.identity;
                _ellaRyderTransform.rotation = Quaternion.identity;
                //_moonscooterTransform.Rotate(0.0f, 180.0f, 0.0f);
            }
        }

        

        _standardRotation = transform.rotation;
    }


    private void Start()
    {
        _lastPosition = transform.position;
        _playerLayerMask = ServiceLocator.Instance.GetService<GameManager>().PlayerLayermask;

        _audioController = ServiceLocator.Instance.GetService<AudioController>();

        _playerPositionWorldForward = this.transform.position.z;
    }
    private void OnEnable()
    {
        // Subscribe to events
        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;
        playerInput.Move.performed += OnMoveInput;
        playerInput.Move.canceled += OnMoveInput;
        playerInput.Action.started += OnJumpInput;
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
        playerInput.Move.performed -= OnMoveInput;
        playerInput.Move.canceled -= OnMoveInput;
        playerInput.Action.started -= OnJumpInput;
    }
    private void FixedUpdate()
    {

    }
    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }

    #endregion



    #region Public Functions

    public override void FixedUpdateController()
    {
        base.FixedUpdateController();

        // auto-jump related
        if (_characterController.enabled == false)
        {
            // add personal gravity to rigidbody
            _rigidbody.AddForce(new Vector3(0, -1.0f, 0) * 1 * _gravityValue * (-1));
        }

        // !!Keep this execution order!!

        bool wasGrounded = _isGrounded;
        var lastYVelocity = _yVelocity;
        _isGrounded = _characterController.isGrounded;

        //Debug.Log("Is Grounded: " + _isGrounded);

        // Apply landing
        if (wasGrounded == false && wasGrounded != _isGrounded)
        {
            Land(lastYVelocity);
        }

        FakeGroundedTimer();

        if (_isGrounded == true)
        {
            _particleDustTrail.gameObject.SetActive(true);
        }
        else
        {
            _particleDustTrail.gameObject.SetActive(false);
        }

        if (_characterController.enabled == true)
        {
            Move();
        }

        if (_isExploringVersion == false)
        {
            Jump();
        }

        ApplyGravity();

        if (_isExploringVersion == false)
        {
            _hoveringComponent.UpdateHovering(_upDownSpeed, _hoverDisplacement);

            UpdateVisual();
        }

        // timer for respawning if stuck
        if (GameManager.IsInCutscene == false)
        {
            PlayerStuckLogic();
        }
    }

    private void PlayerStuckLogic()
    {
        if (_playerMightBeStuck == false)
        {
            _playerStuckUnknownTimer += Time.deltaTime;

            if (_playerStuckUnknownTimer >= _playerStuckUnknownTimeLimit)
            {
                _playerStuckUnknownTimer = 0;

                // measure distance...
                float distanceCovered = Mathf.Abs(this.transform.position.z - _playerPositionWorldForward);
                if (distanceCovered <= _playerStuckFirstDistance)
                {
                    _playerMightBeStuck = true;
                }

                // store distance
                _playerPositionWorldForward = this.transform.position.z;
            }
        }
        else if (_playerMightBeStuck == true)
        {
            _playerStuckMaybeTimer += Time.deltaTime;

            if (_playerStuckMaybeTimer >= _playerStuckMaybeLimit)
            {
                _playerStuckMaybeTimer = 0;

                // measure distance
                float distanceCovered = Mathf.Abs(this.transform.position.z - _playerPositionWorldForward);
                if (distanceCovered <= _playerStuckSecondDistance)
                {
                    // player is likely stuck, respawn to latest checkpoint

                    _playerStuck = true;

                    ServiceLocator.Instance.GetService<GameManager>().RespawnPlayer(this.gameObject,
                        ServiceLocator.Instance.GetService<LevelManager>().CurrentCheckpoint);

                    _playerStuck = false;
                }
                else
                {
                    _playerMightBeStuck = false;
                }

                // store distance
                _playerPositionWorldForward = this.transform.position.z;
            }
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
    public void PlayBounceBackSound()
    {
        _audioController.PlayAudio(_soundBounce);
    }
    public void AutoJumpCCToggleFakeGravity(float timeOfFlight, float fakeGravity)
    {
        // stop a  possible previous jumppad coroutine
        if (CCToggleRoutine != null)
        {
            StopCoroutine(CCToggleRoutine);
        }

        // start the new jumppad coroutine
        CCToggleRoutine = StartCoroutine(ToggleCharacterControllerFakeGravity(timeOfFlight, fakeGravity));
        // play sound
        _audioController.PlayAudio(_soundJump);
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

    public void SetInput(Vector2 input)
    {
        _input = input;
    }

    #endregion



    #region Private Functions

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

        var direction = moveDirection * 1f + _rightVector * -_input.x;
        Vector3 slopeVelocity = AdjustVelocityToSlope(direction);

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

        //       Debug.Log("2  X " + inputX + "\n XVelocity " + _xVelocity);

        //   if (_knockbackComponent.IsTicking)
        //   {
        //       _zVelocity = -_speedForward * _knockbackComponent.Multiplier;
        //   }
        //   else
        //   {
        _zVelocity = (speedForward * (1 + Mathf.Clamp(inputY, -_tiltSpeedUpMultiplier, _tiltSpeedUpMultiplier)) * _speedBoostComponent.Multiplier) * KnockbackMultiplier;
        //   }


        Vector3 speed = new Vector3(_xVelocity, speedForward, _zVelocity);

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

            // play sound
            _audioController.PlayAudio(_soundLand);
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

            // play sound
            _audioController.PlayAudio(_soundJump);
        }

        _isJumping = false;
    }
    private void Hover()
    {
    }
    private void ApplyGravity()
    {
        if (_characterController.enabled == true)
        {
            _gravityComponent.ApplyGravity(_characterController, ref _yVelocity,
              _gravityValue /** (1 + (Mathf.Clamp01(_input.y) * _tiltMultiplier))*/, _isGrounded);
        }

    }
    private void UpdateVisual()
    {
        // Calculate normalized velocity
        _velocity = (transform.position - _lastPosition) / Time.deltaTime;
        _velocityNormalized = _velocity.normalized;
        //      _lastPosition = transform.position;
        //

        _target.transform.localPosition = new Vector3(_input.x * 3, 0, 5.14f);
        //
        //      // Rotate towards Target
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
                _visual.transform.rotation = Quaternion.Lerp(_visual.transform.rotation, rot, _visualLerpSpeed * Time.deltaTime);
            }
        }

        // Rotates along the the forward axis according to the left of right velocity
        var rotationalFactor = Mathf.Clamp(_input.x, -1.0f, 1.0f);
        var visualRot = Quaternion.Euler(_visual.transform.rotation.eulerAngles);
        rot = Quaternion.Euler(0.0f, 0.0f, -moveDirection.z * rotationalFactor * 80.0f) * _standardRotation;
        _visual.transform.rotation = Quaternion.Lerp(_visual.transform.rotation, rot, _visualLerpSpeed * Time.deltaTime);

        // Rotate along x axis according to the vertical input
        rotationalFactor = Mathf.Clamp(_input.y, -1.0f, 1.0f);
        rot = Quaternion.Euler(moveDirection.z * rotationalFactor * 90.0f, 0.0f, 0.0f) * _standardRotation;
        _visual.transform.rotation = Quaternion.Lerp(_visual.transform.rotation, rot, _visualLerpSpeed * Time.deltaTime);
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
    private void OnMoveInput(InputAction.CallbackContext obj)
    {
        _input = obj.ReadValue<Vector2>();
        //Debug.LogWarning("Input X: " + _input.x + "\nInput Y: " + _input.y);
    }
    private void OnJumpInput(InputAction.CallbackContext obj)
    {
        JumpInput();
    }
    private void OnPressed()
    {
        JumpInput();
    }

    private void JumpInput()
    {
        // if I'm in a trigger of an auto jump...
        if (CurrentAutoJumpMaster != null && CurrentAutoJumpMaster.PlayerIsInTrigger == true)
        {
            // if it needs player input and perfect jump the player is grounded...
            if (CurrentAutoJumpMaster.IsAutomatic == false && _isGrounded == true)
            {
                // discern what type of jump to make
                if (CurrentAutoJumpMaster.IsNormalPlayerJump == false)
                {
                    CurrentAutoJumpMaster.CreatePerfectJumpCurve(this);

                    CurrentAutoJumpMaster.DoPerfectJumpFakeGravity(this);
                }
                else
                {
                    ForceJump();
                }
            }
        }
        else if (_isGroundedFake == true && _characterController.enabled == true)
        {
            _isJumping = true;
        }
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

    private IEnumerator ToggleCharacterControllerFakeGravity(float timeOfFlight, float fakeGravity)
    {
        float originalPlayerGravity = _gravityValue;

        if (fakeGravity > 0)
        {
            fakeGravity *= -1;
        }
        _gravityValue = fakeGravity;


        _characterController.enabled = false;
        //Collider.enabled = true;

        yield return new WaitForSeconds(timeOfFlight);

        _characterController.enabled = true;
        //Collider.enabled = false;

        _gravityValue = originalPlayerGravity;
    }

    #endregion
}