using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamoDistance : MonoBehaviour
{
    public float TargetDistance = 35;

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
    private float _timeBetweenVO = 15.0f;

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

    private float _defaultEnginePitch;
    private float _lastDynamoSpeed;
    private float _dynamoDefaultSpeed;
    private float _startDynamoLerpingSpeed;

    public float SpeedFactor = 3;

    private float _currentDistanceToElla;

    [SerializeField]
    private Animator _animator;

    private const string DIGGING_BOOL = "Digging";

    [Header("State colors")]
    [SerializeField]
    private Color _defaultColor = Color.blue;
    [SerializeField]
    private Color _diggingColor = Color.yellow;
    [SerializeField]
    private Color _scaredColor = Color.red;

    [SerializeField]
    private Material _dynamoLightsMat;

    [SerializeField]
    private Vector2 _minMaxDistance = new Vector2(10.0f, 30.0f);

    private float _timePassedSinceLastVO = 0.0f;

    private void Awake()
    {
        _dynamo = GetComponent<CinemachineDollyCart>();
        _dynamoDefaultSpeed = _dynamo.m_Speed;
        _defaultEnginePitch = _engineAudioSource.pitch;
        DefaultState();
    }


    private void FixedUpdate()
    {
        _timePassedSinceLastVO += Time.deltaTime;
        MaintainDistance();

        // BELOW: BEN HIS CODE

        //if (Mathf.Abs(_targetSpeed - _dynamo.m_Speed) > 0.1f)
        //{
        //    // Calculate the difference between the current speed and the target speed
        //    float speedDifference = Mathf.Abs(_dynamo.m_Speed - _targetSpeed);

        //    // Normalize the difference to get a value between 0 and 1
        //    float normalizedDifference = speedDifference / (_targetSpeed - _dynamo.m_Speed + Mathf.Epsilon);

        //    // Use the normalized difference as the lerp factor
        //    float lerpFactor = Time.deltaTime * _speedChangeDuration * normalizedDifference;

        //    // Perform the lerp
        //    _dynamo.m_Speed = Mathf.Lerp(_dynamo.m_Speed, _targetSpeed, lerpFactor);
        //}

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
        _animator.SetBool(DIGGING_BOOL, true);
        DiggingState();

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
        _animator.SetBool(DIGGING_BOOL, false);
        DefaultState();

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

    private void MaintainDistance()
    {
        Vector3 displacement = _dynamo.transform.position - _ellaSpeederGround.transform.position;
        _currentDistanceToElla = Mathf.Abs(displacement.z);
        float dot = Vector3.Dot(displacement, _ellaSpeederGround.transform.forward);

        bool shouldPlayerVo = false;

        if (dot < 0)
        {
            shouldPlayerVo = true;
            // too close clip play
            _voiceAudioSource.clip = _tooCloseClip;
            
            // Calculate the difference between the current speed and the target speed
            float speedDifference = Mathf.Abs(_dynamo.m_Speed - _targetSpeed);

            // Normalize the difference to get a value between 0 and 1
            float normalizedDifference = speedDifference / (_targetSpeed - _dynamo.m_Speed + Mathf.Epsilon);

            // Use the normalized difference as the lerp factor
            float lerpFactor = Time.deltaTime * _speedChangeDuration * normalizedDifference;

            _dynamo.m_Speed = Mathf.Lerp(_dynamo.m_Speed, 80, lerpFactor);
        }
        else
        {
            if (_currentDistanceToElla > _minMaxDistance.y)
            {
                shouldPlayerVo = true;
                _voiceAudioSource.clip = _laughClip;
            }
            else if (_currentDistanceToElla < _minMaxDistance.x)
            {
                shouldPlayerVo = true;
                _voiceAudioSource.clip = _tooCloseClip;
            }

            float distanceDifference = TargetDistance - _currentDistanceToElla;
            _dynamo.m_Speed = distanceDifference * SpeedFactor;
        }

        if (shouldPlayerVo && _voiceAudioSource.isPlaying == false && _voiceAudioSource.clip != null && _timePassedSinceLastVO >= _timeBetweenVO)
        {
            _timePassedSinceLastVO = 0.0f - _voiceAudioSource.clip.length;
            _voiceAudioSource.Play();
        }
    }


    public void DefaultState()
    {
        _dynamoLightsMat.color = _defaultColor;
    }

    public void ScaredState()
    {
        _dynamoLightsMat.color = _scaredColor;
    }

    public void DiggingState()
    {
        _dynamoLightsMat.color = _diggingColor;
    }
}