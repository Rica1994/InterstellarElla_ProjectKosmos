using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Assertions;

public class SpeederSpace : PlayerController
{
    public delegate void SpeederSpaceKnockbackContext(Vector3 position);
    public event SpeederSpaceKnockbackContext OnCollision;
    public event SpeederSpaceKnockbackContext OnKnockbackEnded;

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
        _impactRecieverComponent.OnKnockbackEnded += OnKnockbackFinished;
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

        // Calculate knockback velocity
        var velocity = -_dollyCart.transform.forward;
        //velocity += transform.right * _input.x;
        //velocity += transform.up * _input.y;
        velocity.Normalize();

        // Add impact to player
        _impactRecieverComponent.AddImpact(velocity, _knockbackForce, true);

        OnCollision?.Invoke(transform.position + _impactRecieverComponent.Destination);
    }

    private void OnKnockbackFinished()
    {
        OnKnockbackEnded?.Invoke(transform.position);

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
        Vector3 direction = _dollyCart.transform.right * _input.x;
        direction += _dollyCart.transform.up * _input.y;

        _moveComponent.Move(_characterController, direction, _moveSpeed);

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
