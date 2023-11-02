using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class CrashingMeteor : MonoBehaviour
{
    [SerializeField]
    private Transform _meteorAngle;
    [SerializeField]
    private Transform _crater;
    [SerializeField]
    private AudioElement _crashSound;

    [Header("Settings")]

    // Size
    [SerializeField] private bool _randomizeScale = true;
    [Range(0.5f, 1.5f)] [SerializeField]
    private float _minScale = 0.5f;
    [Range(0.5f, 1.5f)] [SerializeField]
    private float _maxScale = 1.5f;

    // Angle
    [SerializeField] private bool _randomizeAngle = true;
    [Range(30f, 150f)]
    [SerializeField]
    private float _minAngle = 30f;
    [Range(30f, 150f)]
    [SerializeField]
    private float _maxAngle = 150f;

    // Speed
    [SerializeField] private bool _randomizeSpeed = true;
    [Range(0.5f, 2f)]
    [SerializeField]
    private float _minSpeed = 0.5f;
    [Range(0.5f, 2f)]
    [SerializeField]
    private float _maxSpeed = 2f;

    private string METEOR_SPEED = "MeteorSpeed";



    private void Awake()
    {
        // set random rotation to crater
        _crater.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        // Set random scale based on mininum and maximum
        if (_randomizeScale)
        {
            float randomScale = Random.Range(_minScale, _maxScale);
            transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        }

        // Set random angle based on mininum and maximum
        if (_randomizeAngle)
        {
            float randomAngle = Random.Range(_minAngle, _maxAngle);
            _meteorAngle.localRotation = Quaternion.Euler(0, 0, randomAngle);
        }

        // Set random speed based on mininum and maximum
        if (_randomizeAngle)
        {
            float randomSpeed = Random.Range(_minSpeed, _maxSpeed);
            GetComponent<Animator>().SetFloat(METEOR_SPEED, randomSpeed);
        }
    }
}
