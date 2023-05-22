using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpeederSpace : PlayerController
{
    // Parameters
    [SerializeField] private float _boostDuration = 3.0f;
    [SerializeField] private float _boostMultiplier = 1.5f;

    [SerializeField] private GameObject _knockbackPathPrefab;
    [SerializeField] private float _knockbackForce = 20f;

    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private float _cameraBoundOffset = 1f;

    // Controllers
    private CharacterController _characterController;
    private CinemachineDollyCart _dollyCart; 

    // Components
    private MoveComponent _moveComponent;
    private BoostComponent _boostComponent;
    private ImpactRecieverComponent _impactRecieverComponent;

    // Movement
    private Vector2 _input;
    private float _baseSpeed;
    private Vector3 _velocity;
    private Vector3 _previousPoition;

    // Knockback
    private CinemachineSmoothPath _knockbackPath;
    private CinemachineVirtualCamera _knockbackCamera;

    // Utility
    private bool _isApplicationQuitting = false;

    // Player bounds inside camera
    private Vector3 _leftBottomBounds;
    private Vector3 _rightTopBounds;

    public bool IsBoosting => _boostComponent.IsBoosting;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        _dollyCart = GetComponentInParent<CinemachineDollyCart>();
        _baseSpeed = _dollyCart.m_Speed;

        _moveComponent = new MoveComponent();
        _boostComponent = new BoostComponent(_boostDuration, _boostMultiplier);
        _boostComponent.OnBoostEnded += OnBoostEnded;

        _impactRecieverComponent = new ImpactRecieverComponent(_characterController, 3f);
        _impactRecieverComponent.OnKnockbackEnded += OnKnockbackEnded;
    }

    private void Start()
    {
        // Get local camera bounds
        GameObject obj = new GameObject("TestCamera");
        Camera cam = obj.AddComponent<Camera>();

        if (CinemachineCore.Instance.BrainCount > 0)
        {
            // Get Cinemachine information: dolly track camera offset
            CinemachineBrain cmBrain = CinemachineCore.Instance.GetActiveBrain(0);
            var virtualCamera = cmBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
            var dollyCamera = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
            Vector3 offset = dollyCamera.m_PathOffset;

            // Set bounds of camera based off of dolly track offset
            float distance = Vector3.Distance(gameObject.transform.position, gameObject.transform.position - offset);
            _leftBottomBounds = cam.ViewportToWorldPoint(new Vector3(0f, 0f, distance));
            _rightTopBounds = cam.ViewportToWorldPoint(new Vector3(1f, 1f, distance));
        }

        // Destroy camera used for local bound calculation
        Destroy(obj);
    }

    public override void UpdateController()
    {
        base.UpdateController();

        _boostComponent.Update();
        _impactRecieverComponent.Update();

        if (!_impactRecieverComponent.IsColliding)
        {
            Move();
        }
    }

    private void LateUpdate()
    {
        
    }

    public void Boost()
    {
        _boostComponent.Boost();
        _dollyCart.m_Speed = _baseSpeed * _boostComponent.BoostMultiplier;

        //if (CinemachineCore.Instance.BrainCount > 0)
        //{
        //    CinemachineBrain cmBrain = CinemachineCore.Instance.GetActiveBrain(0);
        //    var virtualCamera = cmBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
        //    virtualCamera.m_Lens.FieldOfView = 70f;
        //}
    }

    private void OnBoostEnded()
    {
        _dollyCart.m_Speed = _baseSpeed;
    }

    public override void Collide()
    {
        // Dolly cart speed and unparent
        _dollyCart.m_Speed = 0f;
        transform.parent = null;

        // Add impact to player
        _impactRecieverComponent.AddImpact(-transform.forward, _knockbackForce, true);

        SetupKnockbackPath();
    }

    private void SetupKnockbackPath()
    {
        // Instantiate knockback path
        GameObject knockbackObject = Instantiate(_knockbackPathPrefab);
        _knockbackPath = knockbackObject.GetComponentInChildren<CinemachineSmoothPath>();
        Assert.IsNotNull(knockbackObject, "[SpeederSpace] - Knockback object is null");

        var waypointList = new List<Vector3> { transform.position + _impactRecieverComponent.Destination, transform.position };
        _knockbackPath.m_Waypoints = CreatePath.CreateNewWaypoints(waypointList);

        // Get camera from track
        _knockbackCamera = knockbackObject.GetComponentInChildren<CinemachineVirtualCamera>();
        Assert.IsNotNull(_knockbackCamera, "[SpeederSpace] - VirtualCamera is null");

        _knockbackCamera.gameObject.SetActive(true);
        _knockbackCamera.MoveToTopOfPrioritySubqueue();
        _knockbackCamera.Follow = transform;
        _knockbackCamera.LookAt = _dollyCart.gameObject.transform;

        // Get swith track trigger
        var collider = knockbackObject.GetComponentInChildren<Collider>();
        Assert.IsNotNull(collider, "[SpeederSpace] - Collider is null");
        
        collider.gameObject.transform.position = _knockbackPath.m_Waypoints[_knockbackPath.m_Waypoints.Length - 1].position;


        // Get switch path logic
        var switchPath = knockbackObject.GetComponentInChildren<SwitchPath>();
        Assert.IsNotNull(switchPath, "[SpeederSpace] - SwitchPath is null");
        
        switchPath.SetPathDestination(_dollyCart.m_Path);
    }

    private void OnKnockbackEnded()
    {
        _knockbackPath.m_Waypoints[0].position = transform.position;

        _knockbackCamera.gameObject.SetActive(true);
        _knockbackCamera.MoveToTopOfPrioritySubqueue();
        _knockbackCamera.Follow = _dollyCart.gameObject.transform;
        _knockbackCamera.LookAt = _dollyCart.gameObject.transform;

        _dollyCart.m_Position = 0f;
        _dollyCart.m_Path = _knockbackPath;
        _dollyCart.m_Speed = _baseSpeed;

        transform.SetParent(_dollyCart.gameObject.transform, true);
        transform.localPosition = Vector3.zero;
    }

    private void CheckBounds()
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

    private void Move()
    {
        // Adjust input according to bounds
        CheckBounds();

        // Calculate direction to move 
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
