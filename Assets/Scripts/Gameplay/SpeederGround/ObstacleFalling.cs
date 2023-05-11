using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFalling : MonoBehaviour
{
    private Rigidbody _rigidBody;

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && _rigidBody)
        {
            _rigidBody.useGravity = true;
        }
    }
}
