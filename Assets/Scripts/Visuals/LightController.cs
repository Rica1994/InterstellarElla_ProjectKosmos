using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField]
    private Light _light;

    [SerializeField]
    private Color _startColor;
    [SerializeField]
    private Color _endColor;

    [SerializeField]
    private float _startIntensity;
    [SerializeField]
    private float _endIntensity;

    private float _lerpValue;

    [SerializeField]
    private Transform _playerTransform;

    private Vector3 _startPosition;

    [SerializeField]
    private Transform _endTransform;

    private float _sqrDistance;

    private void Start()
    {
        _startPosition = _playerTransform.position;
        _sqrDistance = (_endTransform.position - _startPosition).sqrMagnitude;
    }


    private void Update()
    {
        float currentSqrDistance = (_playerTransform.position - _startPosition).sqrMagnitude;
        _lerpValue = currentSqrDistance / _sqrDistance;
        _light.color = Color.Lerp(_startColor, _endColor, _lerpValue);
        _light.intensity = Mathf.Lerp(_startIntensity, _endIntensity, _lerpValue);

    }
}