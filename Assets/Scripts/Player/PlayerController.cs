using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    public delegate void PlayerControllerDelegate(PlayerController controller);

    public static event PlayerControllerDelegate PlayerControllerEnabledEvent;

    [SerializeField, Tooltip("Maximum Angle that allows a KnockBack or Collision")]
    private float _collisionAngle = 60.0f;

    protected MultiplierTimerComponent _knockbackComponent;
    protected float KnockbackMultiplier => _knockbackComponent == null ? 1.0f : _knockbackComponent.Multiplier;
    protected LayerMask _playerLayerMask;

    public float CollisionAngle => _collisionAngle;

    
    protected void Start()
    {
        _knockbackComponent =
            new MultiplierTimerComponent(0.0f, 1.0f, 0.0f, true, 1f, true, 1f);

        _playerLayerMask = ServiceLocator.Instance.GetService<GameManager>().PlayerLayermask;
    }

    protected virtual void OnEnable()
    {
        PlayerControllerEnabledEvent?.Invoke(this);
    }

    public virtual void UpdateController()
    {
    }

    public virtual void FixedUpdateController()
    {
    }

    public virtual void Collide(MultiplierTimerComponent knockbackComponent)
    {
        if (knockbackComponent != null) _knockbackComponent = knockbackComponent;
        _knockbackComponent.Activate();
    }
}