using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostComponent
{
    private float _boostTime;
    private float _currentBoostTime;
    private float _boostMultiplier;

    public bool IsBoosting => _currentBoostTime > 0f;
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
    }

    public void Update()
    {
        if (_currentBoostTime > 0)
        {
            _currentBoostTime -= Time.deltaTime;
        }
    }
}
