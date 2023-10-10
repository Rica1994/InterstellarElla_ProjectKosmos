using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.InputSystem;

public class EllaExploring : PlayerController
{
    [Header("Speed")]
    [SerializeField] private Vector3 _moveDirection = new Vector3(0f, 0f, 1f);
    [SerializeField] private float _moveSpeed= 5f;
    private float _rotationSpeed = 1.5f;

    [Header("Boost")]
    [SerializeField] private float _hoverStrengthMax = 7f;
    private double _hoverStrength;
    [SerializeField] private float _hoverDurationLimit = 2f;
    //[SerializeField] private float _hoverCooldownLength = 0.5f;
    //private bool _hoverOnCooldown;
    //private float _hoverCooldownTimer;

    [SerializeField] private Transform _bootParticleLeft;
    [SerializeField] private Transform _bootParticleRight;
    private ParticleSystem _particleBootLeft;
    private ParticleSystem _particleBootRight;
    private bool _playingBoostParticle; // created my own bool instead of "PS.isPlaying", my version works more like I want it to

    [Header("Gravity")]
    [SerializeField] private float _gravityValue = -9.81f;
    [SerializeField] private float _yMaxVelocity = -35f;
    public float Gravity => _gravityValue;

    [Header("Character References")]
    [SerializeField] private GameObject _model;   
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _cameraTargets;
    public GameObject CameraTargets => _cameraTargets;
    [SerializeField] private Collider _collider;
    public Collider Collider => _collider;

    [Header("Camera with brain")]
    [SerializeField] private Camera CameraMain;

    private CharacterController _characterController;
    public CharacterController CharacterControl => _characterController;
    private Rigidbody _rigidbody;
    public Rigidbody Rigid => _rigidbody;

    private Vector3 _rightVector;
    private Vector2 _input;

    private const string _boolGrounded = "IsGrounded";
    private const string _boolBoosting = "IsBoosting";
    private const string _boolMoving = "IsMoving";

    private float _yVelocity = 0f;

    private float _hoverTimer;
    private float _groundedAnimationTimer;
    private float _groundedAnimationBuffer = 0.2f;
    private bool _isHovering = false;
    private bool _isTryingHover = false;
    private bool _canHover = false;
    private bool _startedHoverFromGround = false;

    private bool _isGrounded = false;
    private bool _isApplicationQuitting = false;
    private float _timeNotGrounded = 0f;

    private MoveComponent _moveComponent;
    private HoverComponent _hoverComponent;
    private GravityComponent _gravityComponent;
    private AnimatorComponent _animatorComponent;

    public bool BlockMove;
    public Coroutine CCToggleRoutine;
    private Vector3 _slopeMovement;

    [Header("Sounds")]
    [SerializeField] private AudioSource _sourceBoostBoots;

    [SerializeField] private AudioSource _sourceLandingBoots;

    [SerializeField] private AudioSource _sourceIgnitionBoots;


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _rigidbody = GetComponent<Rigidbody>();
        // Fixes character controller not grounded bug
        _characterController.minMoveDistance = 0f;

        // Initialize player components
        _moveComponent = new MoveComponent();
        _hoverComponent = new HoverComponent();
        _gravityComponent = new GravityComponent();
        _animatorComponent = new AnimatorComponent();

        _moveDirection.Normalize();

        transform.forward = _moveDirection;

        _rightVector = Vector3.Cross(_moveDirection, Vector3.up);
    }
    private void Start()
    {
        // create needed particle systems for boost
        _particleBootLeft = ServiceLocator.Instance.GetService<ParticleManager>().CreateParticleLocalSpacePermanent
            (ParticleType.PS_BoostBoots, _bootParticleLeft);
        _particleBootLeft.Stop();

        _particleBootRight = ServiceLocator.Instance.GetService<ParticleManager>().CreateParticleLocalSpacePermanent
            (ParticleType.PS_BoostBoots, _bootParticleRight);
        _particleBootRight.Stop();

        var touchButton = ServiceLocator.Instance.GetService<HudManager>().TouchButton;
        touchButton.CooldownLength = -1.0f;
        touchButton.Pressed += OnTouchButtonPressed;
        touchButton.Unpressed += OnTouchButtonUnPressed;
    }

    private void OnEnable()
    {
        // Subscribe to events
        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;

        playerInput.MoveNormalized.performed += OnMoveInput;
        playerInput.MoveNormalized.canceled += OnMoveInput;
 
        playerInput.Action.performed += OnHoverInput;
        playerInput.Action.canceled += OnHoverCanceled;
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
        playerInput.MoveNormalized.performed -= OnMoveInput;
        playerInput.MoveNormalized.canceled -= OnMoveInput;

        playerInput.Action.performed -= OnHoverInput;
        playerInput.Action.canceled -= OnHoverCanceled;
    }

    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }




    public override void FixedUpdateController()
    {
        base.FixedUpdateController();

        if (_characterController.enabled == false)
        {
            // add personal gravity to rigidbody
            _rigidbody.AddForce(new Vector3(0, -1.0f, 0) * 1 * _gravityValue*(-1));
        }

        // moves the character
        //_moveComponent.Move(_characterController, _slopeMovement, _moveSpeed);
    }

    public override void UpdateController()
    {
        base.UpdateController();

        if (_characterController.enabled == true)
        {
            // add Y+ movement with a limit dependant on Ella's last grounded position
            Hover();

            // move the player in x-y axis
            Move();

            // only apply gravity when not hovering
            ApplyGravity();

            // animate the character
            AnimateRig();

            bool wasGrounded = _isGrounded;
            
            // !! Keep this execution order !!
            _isGrounded = _characterController.isGrounded;

            Land(wasGrounded);
        }
    }


    public void JumpPadAnimater(Transform XZtransform)
    {
        // set grounded bool
        _isGrounded = false;

        // calculate look vector for ella
        Vector3 forward = XZtransform.forward;
        Vector3 right = XZtransform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 xzNormalized = forward + right;
        Vector3 lookVector = new Vector3(xzNormalized.x, 0.0f, xzNormalized.z);

        // rotate player towards XZ-direction
        _model.transform.rotation = Quaternion.LookRotation(lookVector);

        // set animator bools
        _animatorComponent.SetAnimatorBool(_animator, _boolMoving, true);
        _animatorComponent.SetAnimatorBool(_animator, _boolGrounded, false);
        _animatorComponent.SetAnimatorBool(_animator, _boolBoosting, true);
    }
    public void JumpPadCCToggle(float timeOfFlight)
    {
        // stop a  possible previous jumppad coroutine
        if (CCToggleRoutine != null)
        {
            StopCoroutine(CCToggleRoutine);
        }
        
        // start the new jumppad coroutine
        CCToggleRoutine = StartCoroutine(ToggleCharacterController(timeOfFlight));        
    }
    public void JumpPadCCToggleFakeGravity(float timeOfFlight, float fakeGravity)
    {
        // stop a  possible previous jumppad coroutine
        if (CCToggleRoutine != null)
        {
            StopCoroutine(CCToggleRoutine);
        }

        // start the new jumppad coroutine
        CCToggleRoutine = StartCoroutine(ToggleCharacterControllerFakeGravity(timeOfFlight, fakeGravity));
    }
    public void ToggleMoveInput(float durationBlocked)
    {
        StartCoroutine(ToggleMove(durationBlocked));
    }
    
    private void OnMoveInput(InputAction.CallbackContext obj)
    {
        var x = obj.ReadValue<Vector2>();
        _input = x;
    }
    private void OnHoverInput(InputAction.CallbackContext obj)
    {
        HoverInput();
    }
    private void OnHoverCanceled(InputAction.CallbackContext obj)
    {
        HoverCanceled();
    }

    private void OnTouchButtonPressed()
    {
        HoverInput();
    }
    private void OnTouchButtonUnPressed()
    {
        HoverCanceled();
    }

    private void HoverInput()
    {
        // make sure I'm not in a jump pad bounce when trying to do this
        _isTryingHover = true;
    }

    private void HoverCanceled()
    {
        // make sure I'm not in a jump pad bounce when trying to do this
        _isTryingHover = false;
        _startedHoverFromGround = false;
        _hoverComponent.HoverValueReset();

        //Debug.Log(this.name);
        // stop particle
        _particleBootLeft.Stop();
        _particleBootRight.Stop();

        _playingBoostParticle = false;

        if (_yVelocity > 0)
        {
            _yVelocity = 0;
        }
    }

    private void Land(bool wasGrounded)
    {                        
        if (wasGrounded == false && _isGrounded && _timeNotGrounded > 0.5f)
        {
            _timeNotGrounded = 0;
            if (_sourceLandingBoots.isPlaying == false) _sourceLandingBoots.Play();
        }

        if (_isGrounded == false)
        {
            _timeNotGrounded += Time.deltaTime;
        }
        else
        {
            _timeNotGrounded = 0;
        }
    }

    private void Move()
    {
        if (BlockMove == true)
        {
            return;
        }

        Vector3 forward = CameraMain.transform.forward;
        Vector3 right = CameraMain.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        // this needs to be checked for always
        Vector3 rightRelativeHorizontalInput = _input.x * right;
        Vector3 forwardRelativeVerticalInput = _input.y * forward;
        Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeHorizontalInput;
        var ellaMovement = new Vector3(cameraRelativeMovement.x, 0.0f, cameraRelativeMovement.z);

        // check for slopes
        Vector3 slopeMovement = AdjustVelocityToSlope(ellaMovement);
        //_slopeMovement = AdjustVelocityToSlope(ellaMovement);

        // moves the character
        _moveComponent.Move(_characterController, slopeMovement, _moveSpeed);

        // adjust the look-at
        LookRotation(ellaMovement);
    }
    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.1f))
        {
            var slopeDirection = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeDirection * velocity;

            if (adjustedVelocity.y < 0)
            {
                //Debug.Log("returning slope vel");
                return adjustedVelocity;
            }
        }

        return velocity;
    }
    private void LookRotation(Vector3 movementVector)
    {
        // only rotate if we are detecting input
        if (_input != Vector2.zero)
        {
            var step = 360 * Time.deltaTime * _rotationSpeed;
            _model.transform.rotation = Quaternion.RotateTowards(_model.transform.rotation, Quaternion.LookRotation(movementVector), step);
        }
    }
    private void AnimateRig()
    {
        if (_input != Vector2.zero && BlockMove == false)
        {
            _animatorComponent.SetAnimatorBool(_animator, _boolMoving, true);
        }
        else
        {
            _animatorComponent.SetAnimatorBool(_animator, _boolMoving, false);
        }


        // if grounded ==false, only set the grounded bool if this has been false for 0.2 seconds + IF IM NOT BOOSTING
        if (_isGrounded == false && _isHovering == true)
        {
            // instant
            _groundedAnimationTimer = _groundedAnimationBuffer;
            _animatorComponent.SetAnimatorBool(_animator, _boolGrounded, _isGrounded);
        }
        else if (_isGrounded == false)
        {
            // if my timer exceeds the limit -> start falling
            _groundedAnimationTimer += Time.deltaTime;          
            if (_groundedAnimationTimer >= _groundedAnimationBuffer)
            {
                _animatorComponent.SetAnimatorBool(_animator, _boolGrounded, _isGrounded);
            }
            else
            {
                _animatorComponent.SetAnimatorBool(_animator, _boolGrounded, true);
            }        
        }
        else
        {
            _groundedAnimationTimer = 0;
            _animatorComponent.SetAnimatorBool(_animator, _boolGrounded, _isGrounded);
        }
        
        _animatorComponent.SetAnimatorBool(_animator, _boolBoosting, _isHovering);     
    }

    private void Hover()
    {
        // hover timer
        if (_isHovering == true)
        {
            // set CC step offset
            _characterController.stepOffset = 0f;

            _hoverTimer += Time.deltaTime;
            if (_hoverTimer >= _hoverDurationLimit)
            {
                _hoverTimer = _hoverDurationLimit;
                _canHover = false;
                //_hoverOnCooldown = true;

                // set CC step offset
                _characterController.stepOffset = 0.1f;
            }
        }
        else
        {
            // set CC step offset
            _characterController.stepOffset = 0.1f;
        }


        //// hover cooldown timer
        //if (_hoverOnCooldown == true)
        //{
        //    _hoverCooldownTimer += Time.deltaTime;

        //    if (_hoverCooldownTimer >= _hoverCooldownLength)
        //    {
        //        _canHover = true;
        //    }
        //}



        // hover logic
        if (_canHover == true && _isTryingHover == true)
        {
            // if I'm grounded, set the max boost strength
            if (_isGrounded == true)
            {
                _hoverStrength = _hoverStrengthMax;

                _startedHoverFromGround = true;
                _sourceIgnitionBoots.Play();
            }
            else
            {
                // adjust hover strength exponentially
                float t = _hoverTimer / _hoverDurationLimit;
                float timePercent = SmoothStop5(t);

                _hoverStrength = timePercent * _hoverStrengthMax;
            }

            _hoverComponent.HoverNew(_characterController, ref _yVelocity, _hoverStrength);
            
            // play particle
            if (_playingBoostParticle == false)
            {
                _particleBootLeft.Play();
                _particleBootRight.Play();

                _playingBoostParticle = true;
            }

            // play sound
            if (_sourceBoostBoots.isActiveAndEnabled == false)
            {
                _sourceBoostBoots.enabled = true;
            }              

            _isHovering = true;
        }
        else
        {
            _isHovering = false;
            _startedHoverFromGround = false;

            // stop particle
            _particleBootLeft.Stop();
            _particleBootRight.Stop();

            _playingBoostParticle = false;

            // stop sound
            if (_sourceBoostBoots.isActiveAndEnabled == true)
            {
                _sourceBoostBoots.enabled = false;
            }
        }
    }



    // Source -> https://gizma.com/easing/
    //private double EaseOutCirc()
    //{
    //    float caluclatedStrength = Mathf.Sqrt((float)(1 - Math.Pow(_hoverStrength - 1, 2)));

    //    Debug.Log(caluclatedStrength);

    //    return caluclatedStrength;
    //}
    private float SmoothStop5(float t) // returns a smooth'd value in range[0,1], starting 0 at first, increasing rapidly, to then teeter off at the end
    {
        float s = (1f - t);

        return Mathf.Pow(s, 3);
    }






    private void ApplyGravity()
    {
        if (_isHovering == false)
        {
            _gravityComponent.ApplyGravity(_characterController, ref _canHover, ref _hoverTimer, ref _yVelocity, _gravityValue, _yMaxVelocity, _isGrounded);
        }       
    }



    private IEnumerator ToggleMove(float durationBlocked)
    {
        BlockMove = true;

        yield return new WaitForSeconds(durationBlocked);

        BlockMove = false;
    }
    private IEnumerator ToggleCharacterController(float timeOfFlight)
    {

        CharacterControl.enabled = false;
        Collider.enabled = true;

        yield return new WaitForSeconds(timeOfFlight);

        CharacterControl.enabled = true;
        Collider.enabled = false;
    }
    private IEnumerator ToggleCharacterControllerFakeGravity(float timeOfFlight, float fakeGravity)
    {
        float originalPlayerGravity = _gravityValue;

        if (fakeGravity > 0)
        {
            fakeGravity *= -1;
        }
        _gravityValue = fakeGravity;


        CharacterControl.enabled = false;
        Collider.enabled = true;

        yield return new WaitForSeconds(timeOfFlight);

        CharacterControl.enabled = true;
        Collider.enabled = false;

        _gravityValue = originalPlayerGravity;
    }
}
