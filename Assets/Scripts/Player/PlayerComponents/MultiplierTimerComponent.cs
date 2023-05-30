using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplierTimerComponent
{
    public delegate void BoostCallback();
    public event BoostCallback OnTimerEnded;

    private float _time;
    private float _currentTime;
    private float _targetMultiplier;
    private bool _isTicking = false;

    public bool IsTicking => _isTicking;
    public float Multiplier => IsTicking ? _currentMultiplier : 1f;

    // Lerp expansion
    private float _currentMultiplier;
    private bool _isLerpingIn = false;
    private bool _isLerpingOut = false;

    private bool _shouldLerpIn = false;
    private bool _shouldLerpOut = false;

    private float _lerpInSpeed = 1f;
    private float _lerpOutSpeed = 1f;


    public MultiplierTimerComponent(float time, float multiplier, bool shouldLerpOut = false, float lerpInSpeed = 1f, bool shouldLerpIn = false, float lerpOutSpeed = 1f)
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

    public void Activate()
    {
        _currentTime = _time;
        _isTicking = true;
        _isLerpingIn = true;
    }

    public void Update()
    {
        if (_shouldLerpIn && _isLerpingIn)
        {
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
            _currentTime -= Time.deltaTime;
            return;
        }

        // If timer has ended, notify
        _isTicking = false;
        OnTimerEnded?.Invoke();
        _isLerpingOut = true;
    }

    private void Lerp(float targetMultiplier, ref bool isLerping, float lerpSpeed)
    {
        if (Mathf.Abs(_currentMultiplier - targetMultiplier) > .1f)
        {
            _currentMultiplier = Mathf.Lerp(_currentMultiplier, targetMultiplier, Time.deltaTime * lerpSpeed);
        }
        else
        {
            _currentMultiplier = targetMultiplier;
            isLerping = false;
        }
    }

}
