using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class SimpleCarController : PlayerController
{  
    [Header("Wheel collider references")]
    public WheelCollider frontDriverW;
    public WheelCollider frontPassengerW;
    public WheelCollider rearDriverW;
    public WheelCollider rearPassengerW;
    public Transform frontDriverT;
    public Transform frontPassengerT;
    public Transform rearDriverT;
    public Transform rearPassengerT;

    [SerializeField]
    private Transform _centerOfMass;

    [Header("Car values")]
    // how fast can we do a uturn?
    public float maxSteerAngle = 30;
    // how fast we can go. (motor torque)
    public float motorForce = 50;
    public float brakeForce = 300.0f;

    private float m_steeringAngle;

    /// <summary>
    /// Calculates the path for the car.
    /// </summary>
    private NavMeshPath _navMeshPath;

    [SerializeField]
    private LayerMask _ignoreMe;

    private float _brakeTorque = 0.0f;
    private float _motorTorque = 0.0f;

    private Vector3 _resetPosition;
    private Quaternion _resetRotation;

    [SerializeField]
    private float _rotationSpeed = 0.5f;
    [SerializeField]
    private float _boostStrength = 4f;
    [SerializeField]
    private float _boostCooldownDuration = 3f;
    [SerializeField]
    private float _maxSpeed = 8; // top 2 max speeds get assigned to this value when needed

    private float _maxSpeedBoosted;
    private float _speed;

    public bool BoostCoolingDown;
    public bool IsBoosting; // bool used for checking collisions with rock walls
    private bool _hasBoostedRecently, _boostIsDeclining;
      
    private Rigidbody _rigidbody;
    public Rigidbody Rigid => _rigidbody;

    //   private RockWall[] _rockWallScripts;
    private Collider[] _rockWallColliders;

    private bool _inputBeingGiven;

    //    private MovingDirections _currentMovingDirection;
    private Vector2 _currentMoveDirectionVector;
    private bool _movingReverse;

    public delegate void BoostActive();
    public static event BoostActive OnBoost;

    public delegate void NormalSpeed();
    public static event NormalSpeed OnNormalize;

    private Vector2 _input;


    [Header("Audio stuff")]
    [SerializeField]
    private AudioSource _sourceBoost;
    [SerializeField]
    private AudioElement _soundHop, _soundHopBig;

    private AudioController _audioController;


    [Header("custom gravity")]
    [SerializeField]
    private bool _useCustomGravity;
    public bool UseCustomGravity => _useCustomGravity;
    [SerializeField]
    private float _gravityValue = -9.81f;
    public float GravityValue => _gravityValue;

    public bool IsInPerfectJump;

    public Coroutine ToggleFakeGravityRoutine;
    //private GravityComponent _gravityComponent;
    private bool _isPerfectJumping;

    [Header("Camera managing stuff")]
    [SerializeField] private Camera CameraMainBrain;
    [SerializeField] private Transform _transformFollower;
    public Transform TransformFollower => _transformFollower;
    [SerializeField]
    private FollowCamera _followPlayerObject;
    [SerializeField]
    private FixInputOrientation _fixInputOrient;

    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    public CinemachineVirtualCamera VirtualCamera => _virtualCamera;

    private VirtualCameraManagerExploring _virtualCamManager;

    public GameObject CamerasParent;

    [Header("Particles")]
    [SerializeField]
    private GameObject _particleLeftPipe;
    [SerializeField]
    private GameObject _particleRightPipe;
    [SerializeField]
    private GameObject _particleLeftJet;
    [SerializeField]
    private GameObject _particleRightJet;

    [Header("Other")]
    public bool BlockMove;


    #region Unity Functions

    private void Awake()
    {
        //_gravityComponent = new GravityComponent();
    }
    private void Start()
    {
        _ignoreMe = LayerMask.GetMask("UI", "Ignore Raycast");
        GetComponent<Rigidbody>().centerOfMass = transform.InverseTransformPoint(_centerOfMass.position);

        // Reset position
        _resetPosition = transform.position;
        _resetRotation = transform.rotation;

        _rigidbody = GetComponent<Rigidbody>();
        _speed = _maxSpeed;
        _maxSpeedBoosted = _maxSpeed + _boostStrength + 1.5f;


        //    _rockWallScripts = FindObjectsOfType<RockWall>();
        //   _rockWallColliders = new Collider[_rockWallScripts.Length];
        //  for (int i = 0; i < _rockWallScripts.Length; i ++)
        //    {
        /// //       _rockWallColliders[i] = _rockWallScripts[i].GetComponent<Collider>();
        //    }

        //   UIPanel.Instance.BoostButtonElla.onClick.AddListener(BoostCall);

        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;
        playerInput.Action.started += x => OnBoostInput();

        if (_useCustomGravity == true)
        {
            _rigidbody.useGravity = false;
        }
        else
        {
            _rigidbody.useGravity = true;
        }

        _audioController = ServiceLocator.Instance.GetService<AudioController>();
        _virtualCamManager = ServiceLocator.Instance.GetService<VirtualCameraManagerExploring>();
    }
    private void OnEnable()
    {
        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;
        playerInput.Move.performed += OnMoveInput;
        playerInput.Move.canceled += OnMoveInput;
    }
    private void FixedUpdate()
    {
        //  CalculateArrowDirection();
        GetInput();

        // if car is on slope... (should ideally also check for if it's grounded)  ->  slide of
        if (Mathf.Abs(transform.rotation.x) >= 0.3f)
        {
            _brakeTorque = 0;
            _motorTorque = motorForce;
        }

        //ReverseLogic();   

        Steer();
        Accelerate();

        //  CheckForCollision();
        UpdateWheelPoses();
        ApplyBrakes();

        //   // get the wheels spinning, needed to have the boost work from still position
        BoostWheelColliderSpin();

        LimitSpeed();

        if (_useCustomGravity == true)
        {
            ApplyGravity();
        }
    }

    #endregion



    #region Private Functions

    private void OnMoveInput(InputAction.CallbackContext obj)
    {
        _input = obj.ReadValue<Vector2>();
    }
    private void OnBoostInput()
    {
        if (BoostCoolingDown == false)
        {
            //  RemoveBarriers();
            Boost();
        }
    }
    private void GetInput()
    {
        // if no input is detected...
        if (Mathf.Abs(_input.x) + Mathf.Abs(_input.y) == 0 || BlockMove == true)
        {
            _motorTorque = 0.0f;
            _brakeTorque = motorForce;

            // freeze y rot, set angular vel.y to 0
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
            _rigidbody.angularVelocity = Vector3.zero;  //new Vector3(_rigidbody.angularVelocity.x, 0, _rigidbody.angularVelocity.z);
        }
        // else if slight input is being made...
        else if (Mathf.Abs(_input.x) + Mathf.Abs(_input.y) <= 0.18f && IsBoosting == false) 
        {
            _motorTorque = motorForce / 2;
            _brakeTorque = motorForce / 2;
        }
        // else if major input...
        else
        {
            _rigidbody.constraints = RigidbodyConstraints.None;

            _motorTorque = motorForce;
            _brakeTorque = 0.0f;
        }
    }
    private void Steer()
    {
        if (BlockMove == true)
        {
            return;
        }

        // Slight rotation towards the target of this transform
        if (_input.magnitude > 0.1f)
        {
            // Calculate the target rotation based on the input values.
            Quaternion rotation;
            var rotationAngle = Mathf.Atan2(_input.x, _input.y) * Mathf.Rad2Deg;
            rotation = Quaternion.Euler(0.0f, rotationAngle, 0.0f);  // - should x also not rotate depending on ground plane?

            // Create the target rotation for smooth rotation
            //var targetRot = rotation * _followPlayerObject.ObjectToFollowPlayer.transform.rotation;  // NOT working properly on angles
            var targetRot = rotation * _fixInputOrient.transform.rotation;
            // Calculate the difference in angle between the current and target forward vectors
            //var newForward = rotation * _followPlayerObject.ObjectToFollowPlayer.transform.forward;  // NOT working properly on angles
            var newForward = rotation * _fixInputOrient.transform.forward;

            // Project the forward vectors onto the horizontal plane
            var flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

            // Calculate the steering angle based on the angle difference
            float currentAngleDiffTarget;
            currentAngleDiffTarget = Vector2.SignedAngle(new Vector2(flatForward.x, flatForward.z), new Vector2(newForward.x, newForward.z));
            m_steeringAngle = Mathf.Clamp(currentAngleDiffTarget, -maxSteerAngle, maxSteerAngle) * 0.8f;

            // Apply the target rotation smoothly
            //transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 0.1f);
            var step = (360) * Time.deltaTime * _rotationSpeed;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, step);
        }

        // Apply steering angles to the wheels if not moving in reverse
        if (!_movingReverse)
        {
            frontDriverW.steerAngle = -m_steeringAngle;
            frontPassengerW.steerAngle = -m_steeringAngle;
            rearDriverW.steerAngle = -m_steeringAngle / 5.0f;
            rearPassengerW.steerAngle = -m_steeringAngle / 5.0f;
        }
    }
    private void Accelerate()
    {

        var joyStickMagnitude = (_input).normalized.magnitude;
        frontDriverW.motorTorque = Mathf.Abs(joyStickMagnitude) * _motorTorque;
        frontPassengerW.motorTorque = Mathf.Abs(joyStickMagnitude) * _motorTorque;
        rearDriverW.motorTorque = Mathf.Abs(joyStickMagnitude) * _motorTorque;
        rearPassengerW.motorTorque = Mathf.Abs(joyStickMagnitude) * _motorTorque;
    }
    private void ApplyBrakes()
    {
        //  frontDriverW.brakeTorque = _brakeTorque;
        //  frontPassengerW.brakeTorque = _brakeTorque;
        rearDriverW.brakeTorque = _brakeTorque;
        rearPassengerW.brakeTorque = _brakeTorque;
    }
    /// <summary>
    /// updates positions of the wheels, (wheels can be hanging a bit when you drop the car, this prevents that)
    /// </summary>
    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontDriverW, frontDriverT);
        UpdateWheelPose(frontPassengerW, frontPassengerT);
        UpdateWheelPose(rearDriverW, rearDriverT);
        UpdateWheelPose(rearPassengerW, rearPassengerT);
    }
    private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
    {
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);

        _transform.position = _pos;
        _transform.rotation = _quat;
    }
    private void LimitSpeed()
    {
        if (IsInPerfectJump == true)
        {
            // disable limiting speed on perfect jumps
            return;
        }

        // checks for wether boost was used recently, and whether the car should slow down again (dont want it to go too fast)
        if (_hasBoostedRecently == true)
        {
            _speed = _maxSpeedBoosted;
        }
        else if (_boostIsDeclining == true)
        {
            _speed -= 0.1f;
            if (_speed <= _maxSpeedBoosted)
            {
                _speed = _maxSpeedBoosted;
                _boostIsDeclining = false;
            }
        }

        // limiting car velocity
        if (_rigidbody.velocity.magnitude > _speed)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _speed;
        }
    }
    private void Boost()
    {
        // Maybe this !!!
        _rigidbody.AddForce(transform.forward * _boostStrength, ForceMode.VelocityChange);

        //      Camera.main.GetComponentInParent<FollowCam>().ZoomOut();
        //StartCoroutine(ZoomOut());
        _virtualCamManager.ZoomOutCameraDistance(1.6f, 0.05f, 2);
        _virtualCamManager.ZoomOutCameraFOV();
        

        StartCoroutine(ActivateBoostCooldown()); // cooldown period
        StartCoroutine(DecreaseMaxSpeed()); // sets booleans regarding speed / activates particles        
    }
    private void BoostWheelColliderSpin()
    {
        if (IsBoosting == true)
        {
            frontDriverW.motorTorque = 170;
            frontPassengerW.motorTorque = 170;
            rearDriverW.motorTorque = 170;
            rearPassengerW.motorTorque = 170;
        }
    }
    // does not work properly ?
    private void ApplyGravity()
    {
        _rigidbody.AddForce(new Vector3(0, -1.0f, 0) * 1 * _gravityValue * _rigidbody.mass * (-1));
    }
    private void CheckForCollision()
    {
        var hit = new RaycastHit();

        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.0f, layerMask: 1 << 8))
        {
            var side = Vector3.left;
            if (Vector3.SignedAngle(hit.normal, transform.forward, Vector3.up) > 180.0f) side = -side;

            var RunnerRotation = Quaternion.FromToRotation(side, hit.normal);

            //Smooth rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, RunnerRotation, Time.deltaTime * 10);
        }
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
    private IEnumerator ParticleJetsRoutine(float duration = 0.5f)
    {
        _particleLeftJet.SetActive(true);
        _particleRightJet.SetActive(true);

        yield return new WaitForSeconds(duration);

        _particleLeftJet.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        _particleLeftJet.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();

        _particleRightJet.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        _particleRightJet.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();

        yield return new WaitForSeconds(1f);

        _particleLeftJet.SetActive(false);
        _particleRightJet.SetActive(false);

    }
    private IEnumerator ToggleFakeGravity(float timeOfFlight, float fakeGravity)
    {
        _useCustomGravity = true;
        _rigidbody.useGravity = false;
        IsInPerfectJump = true;

        float originalPlayerGravity = _gravityValue;

        if (fakeGravity > 0)
        {
            fakeGravity *= -1;
        }
        _gravityValue = fakeGravity;


        yield return new WaitForSeconds(timeOfFlight);

        _gravityValue = originalPlayerGravity;

        _useCustomGravity = false;
        _rigidbody.useGravity = true;
        IsInPerfectJump = false;
    }
    private IEnumerator DecreaseMaxSpeed()
    {
        _hasBoostedRecently = true;
        IsBoosting = true;
        OnBoost?.Invoke();

        _sourceBoost.Play();

        _particleLeftPipe.SetActive(true);
        _particleRightPipe.SetActive(true);

        yield return new WaitForSeconds(1);

        _sourceBoost.Stop();

        _particleLeftPipe.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        _particleLeftPipe.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();

        _particleRightPipe.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        _particleRightPipe.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();

        yield return new WaitForSeconds(1);

        _particleLeftPipe.SetActive(false);
        _particleRightPipe.SetActive(false);

        IsBoosting = false;
        OnNormalize?.Invoke();
        _hasBoostedRecently = false;
        _boostIsDeclining = true;
    }
    private IEnumerator ActivateBoostCooldown()
    {
        BoostCoolingDown = true;

        yield return new WaitForSeconds(_boostCooldownDuration);

        BoostCoolingDown = false;
    }
    private IEnumerator ZoomOut()
    {
        float zoomTime = 2.0f;
        float passedTimeZoomingOut = 0.0f;

        float initialFOV = VirtualCamera.m_Lens.FieldOfView;
        float targetFOV = 108.0f;

        float maxTimeZoomedOut = 3.0f;

        while (passedTimeZoomingOut < zoomTime)
        {
            passedTimeZoomingOut += Time.deltaTime;
            VirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(initialFOV, targetFOV, passedTimeZoomingOut / zoomTime);

            yield return null;
        }

        passedTimeZoomingOut = 0.0f;
        //   VirtualCamera.m_Lens.FieldOfView = targetFOV;

        yield return new WaitForSeconds(maxTimeZoomedOut);

        targetFOV = initialFOV;
        initialFOV = VirtualCamera.m_Lens.FieldOfView;

        while (passedTimeZoomingOut < zoomTime)
        {
            passedTimeZoomingOut += Time.deltaTime;
            VirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(initialFOV, targetFOV, passedTimeZoomingOut / zoomTime);

            yield return null;
        }
    }
    private IEnumerator ToggleMove(float durationBlocked)
    {
        BlockMove = true;

        yield return new WaitForSeconds(durationBlocked);

        BlockMove = false;
    }

    /*private void RemoveBarriers()
    {
        for (int i = 0; i < _rockWallColliders.Length; i++)
        {
            _rockWallColliders[i].enabled = false;
        }
    }

    private void ApplyBarriers()
    {
        for (int i = 0; i < _rockWallColliders.Length; i++)
        {
            _rockWallColliders[i].enabled = false;
        }
    }*/

    /*private void CalculateArrowDirection()
    {
        float horizontal = 0;
        float vertical = 0;

        if (Input.GetKey("up"))
        {
            vertical = 1;
        }
        else if (Input.GetKey("down"))
        {
            vertical = -1;
        }
        else
        {
            vertical = 0;
        }

        if (Input.GetKey("right"))
        {
            horizontal = 1;
        }
        else if (Input.GetKey("left"))
        {
            horizontal = -1;
        }
        else
        {
            horizontal = 0;
        }

  //      _arrowKeyDirection = new Vector2(horizontal, vertical);
    }*/

    #endregion



    #region Public Functions

    public void ResetCar()
    {
        transform.position = _resetPosition;
        transform.rotation = _resetRotation;
    }
    public void ForceBoost(bool withHop = true, float hopStrength = 10)
    {
        Boost();

        // add the option to hop vertically 
        if (withHop == true)
        {
            //_rigidbody.AddForce(new Vector3(0, 1.0f, 0) * hopStrength);
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, hopStrength, _rigidbody.velocity.z);

            // activate particle for a second or 2
            StartCoroutine(ParticleJetsRoutine());

            Debug.Log("hopped");
            _audioController.PlayAudio(_soundHop);
        }
    }
    public void AutoJumpToggleFakeGravity(float timeOfFlight, float fakeGravity)
    {
        // stop a  possible previous jumppad coroutine
        if (ToggleFakeGravityRoutine != null)
        {
            StopCoroutine(ToggleFakeGravityRoutine);
        }

        // start the new jumppad coroutine
        ToggleFakeGravityRoutine = StartCoroutine(ToggleFakeGravity(timeOfFlight, fakeGravity));

        // particles jets
        StartCoroutine(ParticleJetsRoutine(timeOfFlight / 2f));

        // play sound
        _audioController.PlayAudio(_soundHopBig);
    }
    public void ChangeCamera(GameObject targetSwapCam)
    {
        _followPlayerObject.ChangeObjectToFollow(targetSwapCam);
    }
    public void ToggleMoveInput(float durationBlocked)
    {
        StartCoroutine(ToggleMove(durationBlocked));
    }

    //public void ReverseLogic()
    //{
    //    //if (_inputBeingGiven == true && _motorTorque > 0)

    //    Debug.Log(transform.eulerAngles.y);

    //    if (_motorTorque > 0)
    //    {
    //        if (transform.eulerAngles.y >= 315 || transform.eulerAngles.y <= 45) // north looking
    //        {
    //            Debug.Log("looking up");
    //            if (_currentMovingDirection == MovingDirections.Down)
    //            {
    //                _motorTorque = -_motorTorque;
    //                _movingReverse = true;
    //            }
    //            else
    //            {
    //                _motorTorque = Mathf.Abs(_motorTorque);
    //                _movingReverse = false;
    //            }
    //        }
    //        else if (transform.eulerAngles.y > 45 && transform.eulerAngles.y <= 135) // east looking
    //        {
    //            Debug.Log("looking east");
    //            if (_currentMovingDirection == MovingDirections.Left)
    //            {
    //                _motorTorque = -_motorTorque;
    //                _movingReverse = true;
    //            }
    //            else
    //            {
    //                _motorTorque = Mathf.Abs(_motorTorque);
    //                _movingReverse = false;
    //            }
    //        }
    //        else if (transform.eulerAngles.y > 135 && transform.eulerAngles.y <= 225) // south looking
    //        {
    //            Debug.Log("looking south");
    //            if (_currentMovingDirection == MovingDirections.Up)
    //            {
    //                _motorTorque = -_motorTorque;
    //                _movingReverse = true;
    //            }
    //            else
    //            {
    //                _motorTorque = Mathf.Abs(_motorTorque);
    //                _movingReverse = false;
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("looking west");
    //            if (_currentMovingDirection == MovingDirections.Right)
    //            {
    //                _motorTorque = -_motorTorque;
    //                _movingReverse = true;
    //            }
    //            else
    //            {
    //                _motorTorque = Mathf.Abs(_motorTorque);
    //                _movingReverse = false;
    //            }
    //        }
    //    }

    //  }

    // perhaps also implement this for rear wheel drive? 

    #endregion
}