using UnityEngine;
using UnityEngine.Rendering;

public class WebGLPerformanceBenchmark : MonoBehaviour
{
    [SerializeField]
    private float _benchmarkDuration = 5f; // Duration of the benchmark in seconds
    [SerializeField]
    private float _lowerEndFPSThreshold = 30f;

    private float _elapsedTime = 0f;
    private int _frameCount = 0;
    private float _totalDeltaTime = 0f;
    private float _averageFPS = 0f;

    private void Update()
    {

        _elapsedTime += Time.deltaTime;
        _frameCount++;
        _totalDeltaTime += Time.deltaTime;

        if (_elapsedTime >= _benchmarkDuration)
        {
            float averageDeltaTime = _totalDeltaTime / _frameCount;
            _averageFPS = 1f / averageDeltaTime;

            Debug.Log("Average FPS: " + _averageFPS);

            // Check if the average FPS falls below a threshold for a lower-end device
            if (_averageFPS > _lowerEndFPSThreshold && Graphics.activeTier != GraphicsTier.Tier2)
            {
                // Adjust graphics settings for high-end devices
                QualitySettings.SetQualityLevel(1); // Set the highest quality level or adjust as needed
            }
            else
            {

            }

            // Disable the benchmark script after measuring the performance
            enabled = false;
        }
    }
}
