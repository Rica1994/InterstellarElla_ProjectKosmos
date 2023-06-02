using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ObstacleCollision : MonoBehaviour
{
    [SerializeField] private bool _destroyOnHit = false;
    private Collider[] _colliders;

    private void Awake()
    {
        // Do not call get component when object will get destroyed when hit
        if (_destroyOnHit)
        {
            _colliders = new Collider[] {};
            return;
        }

        _colliders = GetComponentsInChildren<Collider>();
        Assert.IsNotNull( _colliders, $"[{GetType()}] - colliders not found" );
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController player))
        {
            player.Collide();

            foreach (Collider collider in _colliders)
            {
                collider.enabled = false;
            }

            if (_destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
