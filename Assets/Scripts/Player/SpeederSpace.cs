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

    private Vector3 _velocity;
    private Vector3 _previousPosition;

    private CinemachineDollyCart _dollyCart; 

    private CharacterController _characterController;

    private MoveComponent _moveComponent;
    private BoostComponent _boostComponent;
    private ImpactRecieverComponent _impactRecieverComponent;

    private Vector2 _input;
    private float _baseSpeed;

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

    private void Move()
    {
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
        // Unsubscribe to events
        var playerInput = ServiceLocator.Instance.GetService<InputManager>().PlayerInput;
        playerInput.Move.performed -= x => OnMoveInput(x.ReadValue<Vector2>());
        playerInput.Move.canceled -= x => OnMoveInput(x.ReadValue<Vector2>());
    }

    private void OnMoveInput(Vector2 input)
    {
        _input = input;
    }
}
