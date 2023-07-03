using UnityEngine;

public class WebGLPerformanceBenchmark : MonoBehaviour
{
    public float benchmarkDuration = 5f; // Duration of the benchmark in seconds

    private float elapsedTime = 0f;
    private int frameCount = 0;
    private float totalDeltaTime = 0f;
    private float averageFPS = 0f;

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        frameCount++;
        totalDeltaTime += Time.deltaTime;

        if (elapsedTime >= benchmarkDuration)
        {
            float averageDeltaTime = totalDeltaTime / frameCount;
            averageFPS = 1f / averageDeltaTime;

            Debug.Log("Average FPS: " + averageFPS);

            // Check if the average FPS falls below a threshold for a lower-end device
            float lowerEndThreshold = 30f; // Adjust this threshold as needed
            if (averageFPS < lowerEndThreshold)
            {
                // Adjust graphics settings for lower-end devices
                QualitySettings.SetQualityLevel(0); // Set the lowest quality level or adjust as needed
            }

            // Disable the benchmark script after measuring the performance
            enabled = false;
        }
    }
}
