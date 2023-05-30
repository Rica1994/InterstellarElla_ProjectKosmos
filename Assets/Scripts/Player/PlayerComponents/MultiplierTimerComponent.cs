using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplierTimerComponent
{
    public delegate void BoostCallback();
    public event BoostCallback OnTimerEnded;

    private float _time;
    private float _currentTime;
    private float _multiplier;
    private bool _isTicking = false;

    public bool IsTicking => _isTicking;
    public float Multiplier => IsTicking ? _multiplier : 1f;

    public MultiplierTimerComponent(float time, float multiplier)
    {
        _time = time;
        _currentTime = 0f;
        _multiplier = multiplier;
    }

    public void Activate()
    {
        _currentTime = _time;
        _isTicking = true;
    }

    public void Update()
    {
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
    }
}
