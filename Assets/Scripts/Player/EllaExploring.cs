using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EllaExploring : PlayerController
{
    [Header("Speed")]
    [SerializeField] private Vector3 _moveDirection = new Vector3(0f, 0f, 1f);
    [SerializeField] private float _moveSpeed= 5f;
    private float _rotationSpeed = 1.5f;

    [Header("Boost")]
    [SerializeField, Range(0.8f, 3.0f)] private float _hoverSpeed = 1.5f;
    [SerializeField, Range(1f, 5.0f)] private float _hoverDistance = 2f;
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

    private float _maxHoverHeight;
    private float _hoverTimer;
    private bool _isHovering = false;
    private bool _isTryingHover = false;
    private bool _canHover = false;

    private bool _isGrounded = false;
    private bool _isApplicationQuitting = false;

    private MoveComponent _moveComponent;
    private HoverComponent _hoverComponent;
    private GravityComponent _gravityComponent;
    private AnimatorComponent _animatorComponent;

    public bool BlockMove;
    public Coroutine CCToggleRoutine;


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
    }
    private void OnEnable()
    {
        // Subscribe to events
        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;

        playerInput.Move.performed += x => OnMoveInput(x.ReadValue<Vector2>());
        playerInput.Move.canceled += x => OnMoveInput(x.ReadValue<Vector2>());
 
        playerInput.Action.performed += x => OnHoverInput();
        playerInput.Action.canceled += x => OnHoverCanceled();
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

        playerInput.Action.performed -= x => OnHoverInput();
        playerInput.Action.canceled -= x => OnHoverCanceled();
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

            // !! Keep this execution order !!
            _isGrounded = _characterController.isGrounded;
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
    public void ToggleMoveInput(float durationBlocked)
    {
        StartCoroutine(ToggleMove(durationBlocked));
    }




    private void OnMoveInput(Vector2 input)
    {
        _input = input;
    }
    private void OnHoverInput()
    {
        // make sure I'm not in a jump pad bounce when trying to do this
        _isTryingHover = true;
    }
    private void OnHoverCanceled()
    {
        // make sure I'm not in a jump pad bounce when trying to do this
        _isTryingHover = false;

        // stop particle
        _particleBootLeft.Stop();
        _particleBootRight.Stop();

        _playingBoostParticle = false;

        _yVelocity = 0;
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

        // moves the character
        _moveComponent.Move(_characterController, ellaMovement, _moveSpeed);

        // adjust the look-at
        LookRotation(ellaMovement);
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
        _animatorComponent.SetAnimatorBool(_animator, _boolGrounded, _isGrounded);
        _animatorComponent.SetAnimatorBool(_animator, _boolBoosting, _isHovering);     
    }
    private void Hover()
    {
        // hover timer
        if (_isHovering == true)
        {
            _hoverTimer += Time.deltaTime;

            if (_hoverTimer >= _hoverDurationLimit)
            {
                _canHover = false;
                //_hoverOnCooldown = true;
            }
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
            // if I'm grounded, set the max height
            if (_isGrounded == true)
            {
                _maxHoverHeight = transform.position.y + _hoverDistance;
            }

            // hover up player
            _hoverComponent.Hover(_characterController, ref _yVelocity, _hoverSpeed, _maxHoverHeight, transform.position.y);

            // play particle
            if (_playingBoostParticle == false)
            {
                _particleBootLeft.Play();
                _particleBootRight.Play();

                _playingBoostParticle = true;
            }

            _isHovering = true;
        }
        else
        {
            _isHovering = false;

            // stop particle
            _particleBootLeft.Stop();
            _particleBootRight.Stop();

            _playingBoostParticle = false;
        }


    }
    private void ApplyGravity()
    {
        if (_isHovering == false)
        {
            _gravityComponent.ApplyGravity(_characterController, ref _canHover, ref _hoverTimer, ref _yVelocity, _gravityValue, _isGrounded);
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
}
