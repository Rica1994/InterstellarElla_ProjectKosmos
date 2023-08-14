using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class SimpleCarController : PlayerController
{

 //   private float m_horizontalInput;
 //   private float m_verticalInput;
    private float m_steeringAngle;

    public WheelCollider frontDriverW, frontPassengerW;
    public WheelCollider rearDriverW, rearPassengerW;
    public Transform frontDriverT, frontPassengerT;
    public Transform rearDriverT, rearPassengerT;

    [SerializeField]
    private Transform _centerOfMass;

    // how fast can we do a uturn?
    public float maxSteerAngle = 30;

    // how fast we can go. (motor torque)
    public float motorForce = 50;

    public float brakeForce = 300.0f;

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
    private float _boostStrength = 4f, _boostCooldownDuration;

    private float _maxSpeedBoosted;
    private float _speed;
    [SerializeField]
    private  float _maxSpeed = 8; // top 2 max speeds get assigned to this value when needed

    private bool _hasBoostedRecently, _boostIsDeclining;
    public bool BoostCoolingDown;
    public bool IsBoosting; // bool used for checking collisions with rock walls

    private Rigidbody _rigidbody;

    [SerializeField]
    private GameObject _particleLeftPipe, _particleRightPipe;

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
    }

    private void OnEnable()
    {
        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;
        playerInput.Move.performed += OnMoveInput;
        playerInput.Move.canceled += OnMoveInput;
    }

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
    public void ResetCar()
    {
        transform.position = _resetPosition;
        transform.rotation = _resetRotation;
    }

      public void GetInput()
      {
          if (Mathf.Abs(_input.x) + Mathf.Abs(_input.y) == 0 && IsBoosting == false) // if there's no input being made...
          {
              _motorTorque = 0.0f;
              _brakeTorque = motorForce;

              // freeze y rot, set angular vel.y to 0
              _rigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
              _rigidbody.angularVelocity = new Vector3(_rigidbody.angularVelocity.x, 0, _rigidbody.angularVelocity.z);
          }
          else if (Mathf.Abs(_input.x) + Mathf.Abs(_input.y) <= 0.18f && IsBoosting == false) // if slight input is being made...
          {
              _motorTorque = motorForce / 2;
              _brakeTorque = motorForce / 2;
          }
          else // if major input...
          {
              _rigidbody.constraints = RigidbodyConstraints.None;

              _motorTorque = motorForce;
              _brakeTorque = 0.0f;
          }

          // additional rotational fix
          if (Mathf.Abs(_input.x) + Mathf.Abs(_input.y) == 0)
          {
              _rigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
              _rigidbody.angularVelocity = new Vector3(_rigidbody.angularVelocity.x, 0, _rigidbody.angularVelocity.z);
          }
      }
      
    private void ApplyBrakes()
    {
        //  frontDriverW.brakeTorque = _brakeTorque;
        //  frontPassengerW.brakeTorque = _brakeTorque;
        rearDriverW.brakeTorque = _brakeTorque;
        rearPassengerW.brakeTorque = _brakeTorque;
    }


    private void Steer()
    {
        // Angle difference between forward and the current target.
        var flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

        float currentAngleDiffTarget;
        currentAngleDiffTarget = Vector2.SignedAngle(new Vector2(flatForward.x, flatForward.z), _input);

        m_steeringAngle = Mathf.Clamp(currentAngleDiffTarget, -maxSteerAngle, maxSteerAngle) * 0.8f;

        // slight rotation towards target of this transform
        if (_input.magnitude > 0.1f)
        {
            Vector3 target;
            target = new Vector3(_input.x, 0.0f, _input.y);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target, Vector3.up), 0.1f);
        }

        if (_movingReverse == false)
        {
            frontDriverW.steerAngle = -m_steeringAngle;
            frontPassengerW.steerAngle = -m_steeringAngle;
            rearDriverW.steerAngle = -m_steeringAngle / 5.0f;
            rearPassengerW.steerAngle = -m_steeringAngle / 5.0f;
        }
    }

    // perhaps also implement this for rear wheel drive? 
    private void Accelerate()
    {
        
        var joyStickMagnitude = (_input).normalized.magnitude;
        frontDriverW.motorTorque = Mathf.Abs(joyStickMagnitude) * _motorTorque;
        frontPassengerW.motorTorque = Mathf.Abs(joyStickMagnitude) * _motorTorque;
        rearDriverW.motorTorque = Mathf.Abs(joyStickMagnitude) * _motorTorque;
        rearPassengerW.motorTorque = Mathf.Abs(joyStickMagnitude) * _motorTorque;
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

    private void LimitSpeed()
    {
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
        _rigidbody.AddForce(transform.forward * _boostStrength, ForceMode.VelocityChange);

  //      Camera.main.GetComponentInParent<FollowCam>().ZoomOut();

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
    
    private IEnumerator DecreaseMaxSpeed()
    {
        _hasBoostedRecently = true;
        IsBoosting = true;
        OnBoost?.Invoke();

   //    _particleLeftPipe.SetActive(true);
   //    _particleRightPipe.SetActive(true);

       yield return new WaitForSeconds(1);

   //    _particleLeftPipe.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
   //    _particleLeftPipe.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();

   //    _particleRightPipe.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
   //    _particleRightPipe.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();

       yield return new WaitForSeconds(1);

   //    _particleLeftPipe.SetActive(false);
   //    _particleRightPipe.SetActive(false);

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
}