using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisRotation : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private Vector3 _rotationAxis = new Vector3(0.0f, 1.0f, 0.0f);

    private void Update()
    {
        this.transform.Rotate(_rotationAxis.x * _speed * Time.deltaTime, _rotationAxis.y * _speed * Time.deltaTime, _rotationAxis.z * _speed * Time.deltaTime);
    }

}
