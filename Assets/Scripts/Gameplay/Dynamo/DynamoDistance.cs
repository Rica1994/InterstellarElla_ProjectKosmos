using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamoDistance : MonoBehaviour
{
    private float _distance;
    private CinemachineDollyCart _dynamo;

    [SerializeField]
    private Transform _ellaSpeederGround;

    private void Awake()
    {
        _dynamo = GetComponent<CinemachineDollyCart>();
    }

    private void Update()
    {
        _distance = transform.position.z - _ellaSpeederGround.position.z;

        if (_distance < 20)
        {
            _dynamo.m_Speed = 45;
        }
        else if (_distance <= 50) // added this condition for clarity, though it's optional
        {
            _dynamo.m_Speed = 41; // default speed if distance is between 20 and 50
        }
        else if (_distance <= 65) // distance is between 51 and 65
        {
            _dynamo.m_Speed = 38;
        }
        else if (_distance <= 90) // distance is between 66 and 90
        {
            _dynamo.m_Speed = 30;
        }
        else // distance is greater than 90
        {
            _dynamo.m_Speed = 10;
        }
    }
}
