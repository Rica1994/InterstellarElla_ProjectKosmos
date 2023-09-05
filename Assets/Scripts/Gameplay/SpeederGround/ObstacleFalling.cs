using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObstacleFalling : MonoBehaviour
{
    [SerializeField]
    private TriggerHandler _triggerHandler;

    private Rigidbody _rigidBody;

    private void Start()
    {
        _triggerHandler.OnTriggered += OnTriggerEntered;
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        _triggerHandler.OnTriggered -= OnTriggerEntered;
    }

    private void OnTriggerEntered(TriggerHandler me, Collider other, bool hasEntered)
    {
        if (other.gameObject.TryGetComponent(out PlayerController _) && _rigidBody)
        {
            _rigidBody.useGravity = true;
        }
    }
}
