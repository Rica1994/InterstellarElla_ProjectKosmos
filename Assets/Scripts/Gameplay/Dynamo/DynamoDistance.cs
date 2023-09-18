using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamoDistance : MonoBehaviour
{
    [SerializeField]
    private float _distanceScale = 1;
    private float _distance;
    private CinemachineDollyCart _dynamo;
    private float _targetSpeed = 41;
    private float _speedChangeStartTime;
    [SerializeField]
    private float _speedChangeDuration = 1;

    [SerializeField]
    private Transform _ellaSpeederGround;

    [SerializeField]
    private bool _inverseDistance = false;

    private void Awake()
    {
        _dynamo = GetComponent<CinemachineDollyCart>();
    }

    private void FixedUpdate()
    {
        if (_inverseDistance)
        {
            _distance = _ellaSpeederGround.position.z - transform.position.z;

        }
        else
        {
            _distance = transform.position.z - _ellaSpeederGround.position.z;
        }

        if (_distance < 5 * _distanceScale)
        {
            _targetSpeed = 80;
        }
        else if (_distance < 10 * _distanceScale)
        {
            _targetSpeed = 70;
        }
        else if (_distance < 20 * _distanceScale)
        {
            _targetSpeed = 50;
        }
        else if (_distance <= 35 * _distanceScale) // added this condition for clarity, though it's optional
        {
            _targetSpeed = 41; // default speed if distance is between 20 and 50
        }
        else if (_distance <= 50 * _distanceScale) // distance is between 51 and 65
        {
            _targetSpeed = 25;
        }
        else if (_distance <= 70 * _distanceScale) // distance is between 66 and 90
        {
            _targetSpeed = 10;
        }
        else // distance is greater than 90
        {
            _targetSpeed = 0;
        }

        if (_dynamo.m_Speed != _targetSpeed)
        {
            float elapsedTime = Time.time;
            float t = Mathf.Clamp01(elapsedTime / _speedChangeDuration);
            _dynamo.m_Speed = Mathf.Lerp(_dynamo.m_Speed, _targetSpeed, Time.deltaTime * t); 
        }
    }
}
