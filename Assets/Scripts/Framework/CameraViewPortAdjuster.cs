using UnityEngine;

public class CameraViewPortAdjuster : MonoBehaviour
{
    void Start()
    {
        AdjustCameraAspectRatios();
    }

    void Update()
    {
        // Optionally, you can call this in Update if you need to handle screen size changes at runtime.
         AdjustCameraAspectRatios();
    }

    private void AdjustCameraAspectRatios()
    {
        Camera[] cameras = FindObjectsOfType<Camera>();
        float targetAspect = (float)Screen.width / Screen.height;

        foreach (Camera cam in cameras)
        {
            cam.aspect = targetAspect;
        }
    }
}
