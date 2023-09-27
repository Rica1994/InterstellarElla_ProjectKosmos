using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamoDistance : MonoBehaviour
{
    [SerializeField]
    private Vector2 _minMaxDistance = new Vector2(5.0f, 70.0f);

    private CinemachineDollyCart _dynamo;
    private float _targetSpeed = 41;
    private float _speedChangeStartTime;
    [SerializeField]
    private float _speedChangeDuration = 1;

    [SerializeField]
    private Transform _ellaSpeederGround;

    [Header("Particles and Audio")]
    [SerializeField]
    private ParticleSystem _particleDigging;
    [SerializeField]
    private ParticleSystem _particleFire;

    [SerializeField]
    private bool _isTogglingParticles;

    [SerializeField]
    private AudioSource _engineAudioSource;

    [SerializeField]
    private AudioSource _voiceAudioSource;

    [SerializeField]
    private AudioClip _laughClip;

    [SerializeField]
    private AudioClip _scaredClip;

    [SerializeField]
    private AudioClip _tooCloseClip;

    private float _currentDistanceToElla;
    private float _defaultEnginePitch;
    private float _lastDynamoSpeed;
    private float _dynamoDefaultSpeed;
    private float _startDynamoLerpingSpeed;

    private void Awake()
    {
        _dynamo = GetComponent<CinemachineDollyCart>();
        _dynamoDefaultSpeed = _dynamo.m_Speed;
        _defaultEnginePitch = _engineAudioSource.pitch;
    }


    private void FixedUpdate()
    {
        _currentDistanceToElla = Mathf.Abs(_ellaSpeederGround.position.z - transform.position.z);


        if (_currentDistanceToElla < _minMaxDistance.x)
        {
            // too close clip play
            _voiceAudioSource.clip = _tooCloseClip;
            if (_voiceAudioSource.isPlaying == false) _voiceAudioSource.Play();

            // Set new target speed
            _targetSpeed = 80;
        }
        else if (_currentDistanceToElla > (_minMaxDistance.y + _minMaxDistance.x) / 2.0f)
        {
            _targetSpeed = _dynamoDefaultSpeed;
        }
        else if (_currentDistanceToElla > _minMaxDistance.y)
        {
            // laughable distance
            _voiceAudioSource.clip = _laughClip;
            if (_voiceAudioSource.isPlaying == false) _voiceAudioSource.Play();

            _targetSpeed = 10;
        }

        if (Mathf.Abs(_targetSpeed - _dynamo.m_Speed) > 0.1f)
        {
            // Calculate the difference between the current speed and the target speed
            float speedDifference = Mathf.Abs(_dynamo.m_Speed - _targetSpeed);

            // Normalize the difference to get a value between 0 and 1
            float normalizedDifference = speedDifference / (_targetSpeed - _dynamo.m_Speed + Mathf.Epsilon);

            // Use the normalized difference as the lerp factor
            float lerpFactor = Time.deltaTime * _speedChangeDuration * normalizedDifference;

            // Perform the lerp
            _dynamo.m_Speed = Mathf.Lerp(_dynamo.m_Speed, _targetSpeed, lerpFactor);
        }

        var dif = Mathf.Abs(_lastDynamoSpeed - _dynamo.m_Speed);
        if (dif > 2.0f)
        {
            _lastDynamoSpeed = _dynamo.m_Speed;
            _engineAudioSource.pitch = _defaultEnginePitch * (_dynamo.m_Speed / _dynamoDefaultSpeed);
        }



    }

    // call thesee from triggers Dynamo passes
    public void DynamoGoesDigging()
    {
        if (_isTogglingParticles == false)
        {
            _particleDigging.gameObject.SetActive(true);
            StartCoroutine(ParticleDisablingRoutine(_particleFire, 1.5f));
        }
        else
        {
            Debug.LogWarning(" Dynamo was still toggling another particle ! wait longer or adjust wai time in code !");
        }
    }
    public void DynamoStopsDigging()
    {
        if (_isTogglingParticles == false)
        {
            _particleFire.gameObject.SetActive(true);
            StartCoroutine(ParticleDisablingRoutine(_particleDigging, 3f));
        }
        else
        {
            Debug.LogWarning(" Dynamo was still toggling another particle ! wait longer or adjust wai time in code !");
        }
    }



    private IEnumerator ParticleDisablingRoutine(ParticleSystem particleSystemToStop, float waitTimeDisable)
    {
        _isTogglingParticles = true;

        particleSystemToStop.Stop();

        yield return new WaitForSeconds(waitTimeDisable);

        particleSystemToStop.gameObject.SetActive(false);

        _isTogglingParticles = false;
    }
}
