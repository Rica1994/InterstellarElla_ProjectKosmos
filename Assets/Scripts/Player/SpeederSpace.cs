using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class SpeederSpace : MonoBehaviour
{
    [SerializeField] private float _boostDuration = 3.0f;
    [SerializeField] private float _boostMultiplier = 1.5f;
    [SerializeField] private float _knockbackForce = 20f;

    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private float _cameraBoundOffset = 1f;

    private Vector3 _velocity;
    private Vector3 _previousPosition;

    private CinemachineDollyCart _dollyCart; 

    private CharacterController _characterController;

    private MoveComponent _moveComponent;
    private BoostComponent _boostComponent;
    private ImpactRecieverComponent _impactRecieverComponent;

    private Vector2 _input;
    private float _baseSpeed;
    private bool _isApplicationQuitting = false;

    private Vector3 _leftBottom;
    private Vector3 _rightTop;



    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        _dollyCart = GetComponentInParent<CinemachineDollyCart>();
        _baseSpeed = _dollyCart.m_Speed;

        _moveComponent = new MoveComponent();
        _boostComponent = new BoostComponent(_boostDuration, _boostMultiplier);
        _impactRecieverComponent = new ImpactRecieverComponent(_characterController, 3f);

        _previousPosition = transform.position;
    }

    private void Start()
    {
        // Get local camera bounds
        GameObject obj = new GameObject("TestCamera");
        Camera cam = obj.AddComponent<Camera>();

        // Get Cinemachine information: dolly track camera offset
        var cmBrain = Camera.main.GetComponent<CinemachineBrain>();
        var virtualCamera = cmBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
        var dollyCamera = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        Vector3 offset = dollyCamera.m_PathOffset;

        // Set bounds of camera based off of dolly track offset
        float distance = Vector3.Distance(gameObject.transform.position, gameObject.transform.position - offset);
        _leftBottom = cam.ViewportToWorldPoint(new Vector3(0f, 0f, distance));
        _rightTop = cam.ViewportToWorldPoint(new Vector3(1f, 1f, distance));

        // Destroy camera used for local bound calculation
        Destroy(obj);
    }

    public void Boost()
    {
        _boostComponent.Boost();
        _dollyCart.m_Speed = _baseSpeed * _boostComponent.BoostMultiplier;
    }

    //public void Collide()
    //{
    //    // Knockback backwards and whatever velocity on x
    //    var velocity = _velocity.normalized;
    //    Vector3 knockbackDirection = new Vector3(-velocity.x, -velocity.y, -velocity.z);
    //    _impactRecieverComponent.AddImpact(knockbackDirection.normalized, _knockbackForce);
    //}

    private void Update()
    {
        // Remember previous location
        _previousPosition = gameObject.transform.position;

        // If is not boosting but was boosting prevoius frame
        if (!_boostComponent.IsBoosting && !_dollyCart.m_Speed.Equals(_baseSpeed))
        {
            _dollyCart.m_Speed = _baseSpeed;
        }

        Move();

        // Calculate and save player velocity
        _velocity = (transform.position - _previousPosition) / Time.deltaTime;
    }

    private void CheckBounds()
    {
        if (transform.localPosition.x <= (_leftBottom.x + _cameraBoundOffset) && _input.x < 0f)
        {
            _input.x = 0f;
        }
        else if (transform.localPosition.x >= (_rightTop.x - _cameraBoundOffset) && _input.x > 0f)
        {
            _input.x = 0f;
        }

        if (transform.localPosition.y <= (_leftBottom.y + _cameraBoundOffset) && _input.y < 0f)
        {
            _input.y = 0f;
        }
        else if (transform.localPosition.y >= (_rightTop.y - _cameraBoundOffset) && _input.y > 0f)
        {
            _input.y = 0f;
        }
    }

    private void Move()
    {
        CheckBounds();

        Vector3 direction = transform.right * _input.x;
        direction += transform.up * _input.y;
        _moveComponent.Move(_characterController, direction, _moveSpeed);
    }

    private void OnEnable()
    {
        // Subscribe to events
        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;
        playerInput.Move.performed += x => OnMoveInput(x.ReadValue<Vector2>());
        playerInput.Move.canceled += x => OnMoveInput(x.ReadValue<Vector2>());
    }

    private void OnDisable()
    {
        if (_isApplicationQuitting)
        {
            return;
        }
        
        // Unsubscribe to events
        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;
        playerInput.Move.performed -= x => OnMoveInput(x.ReadValue<Vector2>());
        playerInput.Move.canceled -= x => OnMoveInput(x.ReadValue<Vector2>());
    }
    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }

    private void OnMoveInput(Vector2 input)
    {
        _input = input;
    }
}
