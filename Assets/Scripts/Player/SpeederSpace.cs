using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpeederSpace : PlayerController
{
    // Parameters
    [SerializeField] private float _boostDuration = 3.0f;
    [SerializeField, Range(1.0f, 3.0f)] private float _boostMultiplier = 1.5f;

    [SerializeField] private float _knockbackDuration = 3f;
    [SerializeField, Range(0.0f, 1.0f)] private float _knockbackMultiplier = .3f;

    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private float _cameraBoundOffset = 1f;

    // Controllers
    private CharacterController _characterController;
    private CinemachineDollyCart _dollyCart; 

    // Components
    private MoveComponent _moveComponent;
    private MultiplierTimerComponent _boostComponent;
    private MultiplierTimerComponent _knockbackComponent;

    // Movement
    private Vector2 _input;
    private float _baseSpeed;

    // Utility
    private bool _isApplicationQuitting = false;

    // Player bounds inside camera
    private Vector3 _leftBottomBounds;
    private Vector3 _rightTopBounds;

    public bool IsBoosting => _boostComponent.IsTicking;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        Assert.IsNotNull(_characterController, $"[{GetType()}] - CharacterController is null");

        _dollyCart = GetComponentInParent<CinemachineDollyCart>();
        Assert.IsNotNull(_dollyCart, $"[{GetType()}] - DollyCart is null");
        _baseSpeed = _dollyCart.m_Speed;

        // Create components
        _moveComponent = new MoveComponent();
        _boostComponent = new MultiplierTimerComponent(_boostDuration, _boostMultiplier, true, 2f, true, 1f);
        _boostComponent.OnTimerEnded += BoostEnded;

        _knockbackComponent = new MultiplierTimerComponent(_knockbackDuration, _knockbackMultiplier, true, false);
    }

    private void Start()
    {
        StartCoroutine(SetBounds());
    }

    private IEnumerator SetBounds()
    {
        if (CinemachineCore.Instance.BrainCount > 0)
        {
            // Get Cinemachine information: dolly track camera offset
            CinemachineBrain cmBrain = CinemachineCore.Instance.GetActiveBrain(0);
            CinemachineVirtualCamera virtualCamera = cmBrain.ActiveVirtualCamera as CinemachineVirtualCamera;

            // virtual camera is null at start
            while(!virtualCamera)
            {
                yield return null;
                virtualCamera = cmBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
            }

            var dollyCamera = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
            Assert.IsNotNull(dollyCamera, $"[{GetType()}] - Virtual camera does not have a dolly track component");
            Vector3 offset = dollyCamera.m_PathOffset;

            // Get local camera bounds by temporarily creating a camera at 0,0,0 without rotation
            GameObject obj = new GameObject("TestCamera");
            Camera cam = obj.AddComponent<Camera>();
            
            // Set bounds of camera based off of dolly track offset
            float distance = Vector3.Distance(gameObject.transform.position, gameObject.transform.position - offset);

            _leftBottomBounds = cam.ViewportToWorldPoint(new Vector3(0f, 0f, distance));
            _rightTopBounds = cam.ViewportToWorldPoint(new Vector3(1f, 1f, distance));

            // Destroy camera used for local bound calculation
            Destroy(obj);
        }
    }

    public override void UpdateController()
    {
        base.UpdateController();

        _boostComponent.Update();
        _knockbackComponent.Update();

        Move();
    }

    public void Boost()
    {
        _boostComponent.Activate();
        ServiceLocator.Instance.GetService<VirtualCameraManager>().ZoomOut();
    }

    private void BoostEnded()
    {
        ServiceLocator.Instance.GetService<VirtualCameraManager>().ResetZoom();
    }

    public override void Collide()
    {
        _knockbackComponent.Activate();
        ServiceLocator.Instance.GetService<VirtualCameraManager>().ResetZoom();
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

        _dollyCart.m_Speed = _baseSpeed * _boostComponent.Multiplier * _knockbackComponent.Multiplier;
        
        // Calculate direction to move 
        Vector3 direction = _dollyCart.transform.right * _input.x;
        direction += _dollyCart.transform.up * _input.y;

        _moveComponent.Move(_characterController, direction, _moveSpeed);

        // Allow player to only move/rotate within local bounds and never on the z axis
        _characterController.enabled = false;
        var pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, pos.y, 0f);
        transform.localRotation = Quaternion.identity;
        _characterController.enabled = true;
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
