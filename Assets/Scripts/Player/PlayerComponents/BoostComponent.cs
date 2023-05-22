using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostComponent
{
    public delegate void BoostCallback();
    public event BoostCallback OnBoostEnded;

    private float _boostTime;
    private float _currentBoostTime;
    private float _boostMultiplier;
    private bool _isBoosting = false;

    public bool IsBoosting => _isBoosting;
    public float BoostMultiplier => IsBoosting ? _boostMultiplier : 1f;

    public BoostComponent(float boostTime, float multiplier)
    {
        _boostTime = boostTime;
        _currentBoostTime = 0f;
        _boostMultiplier = multiplier;
    }

    public void Boost()
    {
        _currentBoostTime = _boostTime;
        _isBoosting = true;
    }

    public void Update()
    {
        // Only update while boosting
        if (!_isBoosting) return;

        // Track boost time
        if (_currentBoostTime > 0)
        {
            _currentBoostTime -= Time.deltaTime;
            return;
        }

        // If boost time has ended, notify
        _isBoosting = false;
        OnBoostEnded?.Invoke();
    }
}
