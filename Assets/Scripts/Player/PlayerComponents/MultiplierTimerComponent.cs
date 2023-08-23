using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MultiplierTimerComponent : MonoBehaviour
{
    public delegate void BoostCallback();
    public event BoostCallback OnTimerEnded;

    #region Editor Fields

    [SerializeField]
    private float _time;

    [SerializeField]
    private float _targetMultiplier;
    
    [SerializeField]
    private bool _shouldLerpIn = false;
    
    [SerializeField]
    private bool _shouldLerpOut = false;

    [SerializeField]
    private float _lerpInSpeed = 1f;
    
    [SerializeField]
    private float _lerpOutSpeed = 1f;

    [SerializeField]
    private float _startMultiplierValue = 0.0f;
    
    #endregion

    #region Fields

    private float _currentTime;

    private bool _isTicking = false;

    public bool IsTicking => _isTicking;
    public float Multiplier => _currentMultiplier;

    public float Time
    {
        get => _time;
        set => _time = value;
    }
    
    public float LerpInSpeed => _lerpInSpeed;
    public float LerpOutSpeed => _lerpOutSpeed;
    public bool ShouldLerpIn => _shouldLerpIn;
    public bool ShouldLerpOut => _shouldLerpOut;
    
    // Lerp expansion
    private float _currentMultiplier = 1.0f;
    private bool _isLerpingIn = false;
    private bool _isLerpingOut = false;
    private float _currentLerpValue = 0.0f;
    private float _startLerpMultiplierValue = 0.0f;
    
    #endregion
    
    public MultiplierTimerComponent(float time, float multiplier)
        : this(time, multiplier, false, 1f, false, 1f)
    {
    }

    public MultiplierTimerComponent(float time, float multiplier, bool shouldLerpOut, bool shouldLerpIn)
        : this(time, multiplier, shouldLerpIn, 1f, shouldLerpOut, 1f)    
    {
    }

    public MultiplierTimerComponent(float time, float multiplier, bool shouldLerpOut, float lerpInSpeed, bool shouldLerpIn, float lerpOutSpeed)
    {
        _time = time;
        _currentTime = 0f;
        _targetMultiplier = multiplier;
        _currentMultiplier = multiplier;

        if (shouldLerpIn)
        {
            _currentMultiplier = 1f;
        }

        _shouldLerpIn = shouldLerpIn;
        _shouldLerpOut = shouldLerpOut;
        _lerpInSpeed = lerpInSpeed;
        _lerpOutSpeed = lerpOutSpeed;
    }
    
    public MultiplierTimerComponent(float time, float multiplier, float targetMultiplier, bool shouldLerpOut, float lerpInSpeed, bool shouldLerpIn, float lerpOutSpeed)
    {
        _time = time;
        _currentTime = 0f;
        _targetMultiplier = targetMultiplier;
        _currentMultiplier = multiplier;

        if (shouldLerpIn)
        {
            _currentMultiplier = 1f;
        }

        _shouldLerpIn = shouldLerpIn;
        _shouldLerpOut = shouldLerpOut;
        _lerpInSpeed = lerpInSpeed;
        _lerpOutSpeed = lerpOutSpeed;
    }

    private void Awake()
    {
        _currentMultiplier = _startMultiplierValue;
    }

    public void Activate()
    {
        // TARGET MULTIPLIER IS THE ISSUE !!!!

        //Debug.Log(_targetMultiplier + " - " + _isLerpingIn + " - " + _lerpInSpeed);
        //Debug.Log("----");

        _currentTime = _time;
        _isTicking = true;
        _isLerpingIn = true;
        if (_shouldLerpIn == false) _currentMultiplier = _targetMultiplier;
        else _startLerpMultiplierValue = _currentMultiplier;
    }

    public void Update()
    {
        if (_shouldLerpIn && _isLerpingIn)
        {
            //Debug.Log(_targetMultiplier + " - " + _isLerpingIn + " - " + _lerpInSpeed);

            Lerp(_targetMultiplier, ref _isLerpingIn, _lerpInSpeed);
        }
        else if (_shouldLerpOut && _isLerpingOut)
        {
            Lerp(1f, ref _isLerpingOut, _lerpOutSpeed);
        }

        // Only update while active
        if (!_isTicking) return;

        // Track boost time
        if (_currentTime > 0)
        {
            _currentTime -= UnityEngine.Time.deltaTime;
            return;
        }

        // If timer has ended, notify
        _isTicking = false;
        OnTimerEnded?.Invoke();
        
        if (_shouldLerpOut)
        {
            _isLerpingOut = true;
            _startMultiplierValue = _currentMultiplier;
        }
        else
        {
            _currentMultiplier = 1.0f;
        }
    }

    private void Lerp(float targetMultiplier, ref bool isLerping, float lerpSpeed)
    {
        _currentLerpValue = Mathf.Clamp01(_currentLerpValue + UnityEngine.Time.deltaTime * lerpSpeed);
        _currentMultiplier = Mathf.Lerp(_startMultiplierValue, targetMultiplier, _currentLerpValue);
        
        if (Mathf.Approximately(_currentMultiplier,targetMultiplier))
        {
            _currentLerpValue = 0.0f;
            _currentMultiplier = targetMultiplier;
            isLerping = false;
        }
    }
}
