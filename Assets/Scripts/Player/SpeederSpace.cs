using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class SpeederSpace : PlayerController
{
    // Parameters
    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private float _rotationSpeed = 1f;
    [SerializeField] private float _cameraBoundOffset = 1f;

    // Controllers
    private CharacterController _characterController;
    private CinemachineDollyCart _dollyCart;

    // Components
    private MoveComponent _moveComponent;
    private MultiplierTimerComponent _boostComponent;

    // Camera
    [Header("put in the StartPath camera")]
    [SerializeField]
    private CinemachineVirtualCamera CameraWithDollyTrack;

    [Header("actual camera that will be rendering")]
    [SerializeField]
    private CinemachineVirtualCamera CameraFollow;

    private CinemachineTransposer CameraFollowTransposer;

    // Movement
    private Vector2 _input;

    private float _baseSpeed;

    // Movement visuals
    private Vector3 TargetRotation;
    private Vector3 CurrentRotation;

    [Header("Player visuals")]
    [SerializeField] private GameObject _playerVisuals;

    [SerializeField] private GameObject _lookTarget;

    // Utility
    private bool _isApplicationQuitting = false;

    // Player bounds inside camera
    private Vector3 _leftBottomBounds;
    private Vector3 _rightTopBounds;

    public bool IsBoosting => _boostComponent.IsTicking;

    [Header("Inversion of controls")]
    public bool InvertControls;

    [Header("Magnet")]
    [SerializeField]
    private MagnetPlayerSpace _magnetSpace;

    private bool _magnetIsActive;
    private bool _magnetOnCooldown;

    [SerializeField]
    private float _magnetDuration = 3;
    [SerializeField]
    private float _magnetCooldownTime = 3;

    [Header("Audio")]
    [SerializeField]
    private AudioElement _soundCollision;

    private AudioController _audioController;
    private ParticleManager _particleManager;

    [Header("Animations")]
    [SerializeField]
    private Animation _animationVisual;



    #region Unity Functions

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        Assert.IsNotNull(_characterController, $"[{GetType()}] - CharacterController is null");

        _dollyCart = GetComponentInParent<CinemachineDollyCart>();
        Assert.IsNotNull(_dollyCart, $"[{GetType()}] - DollyCart is null");
        _baseSpeed = _dollyCart.m_Speed;

        // Create components
        _moveComponent = new MoveComponent();
        _boostComponent = new MultiplierTimerComponent(0.0f, 1.5f, true, 2f, true, 1f);
    }
    void Start()
    {
        base.Start();

        _magnetSpace.EnableFullObject(false);

        _audioController = ServiceLocator.Instance.GetService<AudioController>();
        _particleManager = ServiceLocator.Instance.GetService<ParticleManager>();

        SetBounds();    
    }
    private void OnEnable()
    {
        // Subscribe to events
        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;
        playerInput.Move.performed += OnMoveInput;
        playerInput.Move.canceled += OnMoveInput;

        playerInput.Action.performed += OnMagnetInput;
    }
    private void OnDisable()
    {
        if (_isApplicationQuitting)
        {
            return;
        }

        // Unsubscribe to events
        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;
        playerInput.Move.performed -= OnMoveInput;
        playerInput.Move.canceled -= OnMoveInput;

        playerInput.Action.performed -= OnMagnetInput;
    }
    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }

    #endregion



    #region Private Functions

    private void OnMoveInput(InputAction.CallbackContext obj)
    {
        _input = obj.ReadValue<Vector2>();
    }
    private void OnMagnetInput(InputAction.CallbackContext obj)
    {
        if (_magnetIsActive == false && _magnetOnCooldown == false)
        {
            StartCoroutine(ActivateMagnet());
        }
    }
    private void BoostEnded()
    {
        ServiceLocator.Instance.GetService<VirtualCameraManager>().ResetZoom();
        _boostComponent.OnTimerEnded -= BoostEnded;
    }
    private void SetBounds()
    {
        if (CinemachineCore.Instance.BrainCount > 0)
        {
            var dollyCamera = CameraWithDollyTrack.GetCinemachineComponent<CinemachineTrackedDolly>();

            Assert.IsNotNull(dollyCamera, $"[{GetType()}] - Virtual camera does not have a dolly track component");

            Vector3 offset = dollyCamera.m_PathOffset;

            // Get local camera bounds by temporarily creating a camera at 0,0,0 without rotation
            GameObject obj = new GameObject("TestCamera");
            Camera cam = obj.AddComponent<Camera>();

            // Set bounds of camera based off of dolly track offset
            float distance = Vector3.Distance(gameObject.transform.position, gameObject.transform.position - offset);

            _leftBottomBounds = cam.ViewportToWorldPoint(new Vector3(0f, 0f, distance));
            _rightTopBounds = cam.ViewportToWorldPoint(new Vector3(1f, 1f, distance));

            Debug.Log(_leftBottomBounds + "Left bottom vector");
            Debug.Log(_rightTopBounds + "Right Top vector");
            // adding extra distance for 2019 Unity (it shrank when changing versions...)
            _leftBottomBounds += new Vector3(-1.5f,0,0);
            _rightTopBounds += new Vector3(1.5f, 0, 0);


            // Destroy camera used for local bound calculation
            Destroy(obj);
        }
    }
    private void CheckBounds()
    {
        if (InvertControls == false)
        {
            // If player is at the left or right bounds
            if ((transform.localPosition.x <= (_leftBottomBounds.x + _cameraBoundOffset) && _input.x < 0f) ||
                (transform.localPosition.x >= (_rightTopBounds.x - _cameraBoundOffset) && _input.x > 0f))
            {
                _input.x = 0f;
            }

            // If player at the top or bottom bounds
            if ((transform.localPosition.y <= (_leftBottomBounds.y + _cameraBoundOffset) && _input.y < 0f) ||
                (transform.localPosition.y >= (_rightTopBounds.y - _cameraBoundOffset) && _input.y > 0f))
            {
                _input.y = 0f;
            }
        }
        else
        {
            // If player is at the left or right bounds
            if ((transform.localPosition.x <= (_leftBottomBounds.x + _cameraBoundOffset) && _input.x > 0f) ||
                (transform.localPosition.x >= (_rightTopBounds.x - _cameraBoundOffset) && _input.x < 0f))
            {
                _input.x = 0f;
            }

            // If player at the top or bottom bounds
            if ((transform.localPosition.y <= (_leftBottomBounds.y + _cameraBoundOffset) && _input.y > 0f) ||
                (transform.localPosition.y >= (_rightTopBounds.y - _cameraBoundOffset) && _input.y < 0f))
            {
                _input.y = 0f;
            }
        }
    }
    private void Move()
    {
        // Adjust input according to bounds
        CheckBounds();

        _dollyCart.m_Speed = _baseSpeed * _boostComponent.Multiplier * KnockbackMultiplier;

        // Calculate direction to move 
        Vector3 direction = _dollyCart.transform.right * _input.x;
        direction += _dollyCart.transform.up * _input.y;

        if (InvertControls == true)
        {
            direction = new Vector3(-direction.x, -direction.y, -direction.z);
        }

        _moveComponent.Move(_characterController, direction, _moveSpeed);

        // visualize the model rotating
        AnimateModel();

        // Allow player to only move/rotate within local bounds and never on the z axis
        _characterController.enabled = false;
        var pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, pos.y, 0f);
        transform.localRotation = Quaternion.identity;
        _characterController.enabled = true;
    }
    private void AnimateModel()
    {
        // Rotate towards Target
        var rot = Quaternion.FromToRotation(_playerVisuals.transform.forward,
            _lookTarget.transform.position - _playerVisuals.transform.position) * _playerVisuals.transform.rotation;

        _playerVisuals.transform.rotation = Quaternion.Lerp(_playerVisuals.transform.rotation, rot, 0.2f);

        // Rotates along the the forward axis according to the left of right velocity
        var rotationalFactor = Mathf.Clamp(_input.x, -1.0f, 1.0f);
        if (InvertControls == true)
        {
            rotationalFactor *= -1;
        }

        rot = Quaternion.Euler(0.0f, rotationalFactor * 45.0f, 0.0f);
        //Debug.Log("old rotation for me to use -> " + rot.eulerAngles);
        rot = Quaternion.Euler(_dollyCart.transform.rotation.eulerAngles.x,
            _dollyCart.transform.rotation.eulerAngles.y + (rotationalFactor * 45.0f),
            _dollyCart.transform.rotation.eulerAngles.z);

        //Debug.Log("dolly cart rotation -> " + _dollyCart.transform.rotation.eulerAngles);
        //Debug.Log("new rotation -> " + rot.eulerAngles);

        _playerVisuals.transform.rotation = Quaternion.Lerp(_playerVisuals.transform.rotation, rot, 0.1f);

        // Rotate along x axis according to the vertical input
        rotationalFactor = Mathf.Clamp(_input.y, -1.0f, 1.0f);
        if (InvertControls == true)
        {
            rotationalFactor *= -1;
        }

        rot = Quaternion.Euler(_dollyCart.transform.rotation.eulerAngles.x + (-rotationalFactor * 90.0f),
            _dollyCart.transform.rotation.eulerAngles.y,
            _dollyCart.transform.rotation.eulerAngles.z);

        _playerVisuals.transform.rotation = Quaternion.Lerp(_playerVisuals.transform.rotation, rot, 0.1f);
    }
    private void UpdateCameraOffset()
    {
        // angleX and angleY don't make much sense anymore when both are being adjusted ... fu*k rotations

        // get negative numbers instead of 0-360 range (this way i get the (-10)-(10) range)
        float angleX = _dollyCart.transform.rotation.eulerAngles.x;
        angleX = (angleX > 180) ? angleX - 360 : angleX;
        float angleY = _dollyCart.transform.rotation.eulerAngles.y;
        angleY = (angleY > 180) ? angleY - 360 : angleY;

        // normalize / lerp the angles
        float normalizedRotationX = Mathf.InverseLerp(-80f, 80f, angleX);
        float offsetX = Mathf.Lerp(-2.5f, 2.5f, normalizedRotationX);

        float normalizedRotationY = Mathf.InverseLerp(-35f, 35f, angleY);
        float offsetY = Mathf.Lerp(-2f, 2f, normalizedRotationY);

        Vector3 lerpedVector = new Vector3(offsetX, offsetY, -4.75f);
        //  CameraFollowTransposer.m_FollowOffset = lerpedVector;
    }
    private IEnumerator ActivateMagnet()
    {
        _magnetIsActive = true;
        _magnetOnCooldown = true;

        // anable object and particles
        _magnetSpace.EnableFullObject(true);
        _magnetSpace.StartParticleSystem(true);

        yield return new WaitForSeconds(_magnetDuration);

        // disbale particles
        _magnetSpace.StartParticleSystem(false);

        yield return new WaitForSeconds(1f);

        // disable the full object
        _magnetSpace.EnableFullObject(false);
        _magnetIsActive = false;

        yield return new WaitForSeconds(_magnetCooldownTime);

        // reset when cooldown reached
        _magnetOnCooldown = false;
    }

    #endregion


    #region Public Functions

    public override void UpdateController()
    {
        base.UpdateController();

        //   _boostComponent.Update();

        //UpdateCameraOffset();
        //Debug.Log(_dollyCart.transform.forward);

        Move();
    }
    public override void Collide(MultiplierTimerComponent knockbackComponent)
    {
        base.Collide(knockbackComponent);
        ServiceLocator.Instance.GetService<VirtualCameraManager>().ResetZoom();
    }
    public void Boost(MultiplierTimerComponent boostComponent)
    {
        _boostComponent = boostComponent;
        _boostComponent.Activate();
        _boostComponent.OnTimerEnded += BoostEnded;
        ServiceLocator.Instance.GetService<VirtualCameraManager>().ZoomOut();
    }
    public void PlayCollisionSound()
    {
        _audioController.PlayAudio(_soundCollision);
    }
    public void PlayCollisionAnimation()
    {
        _animationVisual.Play();
    }
    public void LosePickups()
    {
        _particleManager.CreateParticleLocalSpace(ParticleType.PS_PickupTrigger, this.transform);
    }


    #endregion
}