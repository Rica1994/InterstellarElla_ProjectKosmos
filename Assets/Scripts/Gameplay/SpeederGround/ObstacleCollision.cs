using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ObstacleCollision : MonoBehaviour
{
    public delegate void ObstacleCollisionDelegate(PlayerController player, ObstacleCollision obstacle);

    public event ObstacleCollisionDelegate CollidedEvent;

    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController player))
        {
            CollidedEvent?.Invoke(player, this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerController player))
        {
            CollidedEvent?.Invoke(player, this);
        }
    }

    public void SetCollider(bool enable)
    {
        _collider.enabled = enable;
    }
}