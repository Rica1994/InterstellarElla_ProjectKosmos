using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ObstacleCollision : MonoBehaviour
{
    private Collider[] _colliders;

    private void Awake()
    {
        _colliders = GetComponents<Collider>();
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
        }
    }
}
