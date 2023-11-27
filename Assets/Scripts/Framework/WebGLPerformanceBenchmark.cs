using UnityEngine;
using UnityEngine.Rendering;

public class WebGLPerformanceBenchmark : MonoBehaviour
{
    [SerializeField]
    private float _waitTime = 5.0f;

    [SerializeField]
    private float _benchmarkDuration = 5f; // Duration of the benchmark in seconds
    [SerializeField]
    private float _lowerEndFPSThreshold = 30f;
    [SerializeField]
    private float _midEndFPSThresHold = 60.0f;

    [SerializeField]
    private GameObject _rootTestObjects;

    private float _elapsedTime = 0f;
    private int _frameCount = 0;
    private float _totalDeltaTime = 0f;
    private float _averageFPS = 0f;

    public float BenchMarkingProgress => _elapsedTime / (_benchmarkDuration + _waitTime);

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime < _waitTime) return;

        _frameCount++;
        _totalDeltaTime += Time.deltaTime;

        if (_elapsedTime >= _benchmarkDuration + _waitTime)
        {
            float averageDeltaTime = _totalDeltaTime / _frameCount;
            _averageFPS = 1f / averageDeltaTime;

            Debug.Log("Average FPS: " + _averageFPS);

            var qualityLevel = QualitySettingsManager.QualityRank.Low;

            // Check if the average FPS falls below a threshold for a lower-end device
            if (_averageFPS > _midEndFPSThresHold && (SystemInfo.deviceModel.StartsWith("Safari") && ServiceLocator.Instance.GetService<GameManager>().IsMobileWebGl) == false)
            {
                qualityLevel = QualitySettingsManager.QualityRank.High;
            }
            else if (_averageFPS > _lowerEndFPSThreshold)
            {
                qualityLevel = QualitySettingsManager.QualityRank.Medium;
            }
            else
            {
                qualityLevel = QualitySettingsManager.QualityRank.Low;
            }

            GameManager.Data.QualityRank = qualityLevel;

            // Disable the benchmark script after measuring the performance
            enabled = false;

            _rootTestObjects.SetActive(false);
            ServiceLocator.Instance.GetService<QualitySettingsManager>().SetQualityLevelFeatures(qualityLevel);
        }
    }
}
