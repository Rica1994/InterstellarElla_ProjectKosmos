using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class StatisticsMeter : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField]
    private bool _fpsCounter = true;
    [SerializeField]
    private bool _qualityTier = true;
    [SerializeField]
    private bool _scenesLoaded = true;
    [SerializeField]
    private bool _deviceInfo = true;
    [SerializeField]
    private float _refreshTime = 0.5f;


    [Header("References")]

    [SerializeField]
    private Text _fpsText;
    [SerializeField]
    private Text _qualitySettingText;
    [SerializeField]
    private Text _scenesLoadedText;
    [SerializeField]
    private Text _deviceNameText;

    private int _frameCounter = 0;
    private float _timeCounter = 0.0f;
    private float _lastFramerate = 0.0f;


    private void Awake()
    {
        if (_deviceInfo)
        {
            _deviceNameText.text = "Device Model: " + SystemInfo.deviceModel + 
                "\nDevice operatingSystem: " + SystemInfo.operatingSystem;
        }
    }

    private void Update()
    {
        if (_fpsCounter)
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
            _fpsText.text = "FPS: " + _lastFramerate.ToString();
        }

        if (_qualityTier)
        {
            _qualitySettingText.text = "Quality: " + QualitySettings.GetQualityLevel();
        }

        if (_scenesLoaded)
        {
            _scenesLoadedText.text = "Scenes loaded: " + SceneManager.sceneCount.ToString();
        }
    }
}