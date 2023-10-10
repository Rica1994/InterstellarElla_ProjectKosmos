using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;
    [SerializeField]
    private SpeederGround _speederGround;
    [SerializeField]
    private CinemachineDollyCart _dynamoDollyCart;
    [SerializeField]
    private GameObject _Sequence;

    [SerializeField]
    private float _targetFPS = 40f;

    // Timer
    [SerializeField]
    private float _duration = 90f;  // Total duration of the timer
    private float _currentTimer = 0f;  // Current value of the timer (between 0 and 1)

    private float _elapsedTime = 0f;  // Elapsed time since the timer started

    private float _deltaTime = 0.0f;
    private float _timer = 0.0f;
    private int _frameCount = 0;
    private float _averageFPS = 0.0f;

    // FPS Counter
    private int _frameCounter = 0;
    private float _timeCounter = 0.0f;
    private float _lastFramerate = 0.0f;
    private float _refreshTime = 0.5f;

    private bool _startLoading = false;

    private void Awake()
    {
        GameManager.IsInCutscene = true;
        _speederGround.speedForward = 0;
        _dynamoDollyCart.m_Speed = 0;
        _Sequence.SetActive(false);
        if (!SystemInfo.deviceModel.StartsWith("Safari"))
        {
        }
    }

    private void Start()
    {
        StartCoroutine(StartLoading());
    }

    private void Update()
    {
        if (_startLoading)
        {
            CalculateAverageFPS();
            CalculateFPS();

            if (_elapsedTime < _duration)
            {
                _elapsedTime += Time.deltaTime;
                _currentTimer = Mathf.Clamp01(_elapsedTime / _duration);  // Normalize the value between 0 and 1

                // Output the current value
                Debug.Log("Current value: " + _currentTimer);
                _slider.value = _currentTimer;
            }
            else
            {
                // Timer has finished
                Debug.Log("Timer finished!");
                StartGameplay();
            }


            // Adjust elasped time according to currentframerate
            if (_lastFramerate / _targetFPS > _currentTimer)
            {
                _elapsedTime = (_lastFramerate / _targetFPS) * _duration;
            }

            if (_averageFPS > _targetFPS)
            {
                StartGameplay();
            }
        }
    }

    private void CalculateAverageFPS()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        _frameCount++;
        _timer += Time.unscaledDeltaTime;

        // Calculate average FPS every 5 seconds
        if (_timer >= 5.0f)
        {
            _averageFPS = _frameCount / _timer;



            if (_averageFPS / _targetFPS > _currentTimer)
            {
                _currentTimer = _averageFPS / _targetFPS;
            }

            // Reset timer and frame count for the next interval
            _timer = 0.0f;
            _frameCount = 0;
        }
    }

    private void StartGameplay()
    {
        _speederGround.speedForward = 26;
        _dynamoDollyCart.m_Speed = 41;
        _Sequence.SetActive(true);
        Destroy(gameObject);
    }

    private void CalculateFPS()
    {
        if (_timeCounter < _refreshTime)
        {
            _timeCounter += Time.deltaTime;
            _frameCounter++;
        }
        else
        {
            //This code will break if you set your m_refreshTime to 0, which makes no sense.
            _lastFramerate = (float)_frameCounter / _timeCounter;
            _frameCounter = 0;
            _timeCounter = 0.0f;
        }
    }

    IEnumerator StartLoading()
    {
        yield return new WaitForSeconds(5);
        _startLoading = true;
    }
}
