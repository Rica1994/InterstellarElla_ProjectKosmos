using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityCamera : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera == null)
        {
            Debug.LogError("There was no camera");
        }
        ServiceLocator.Instance.GetService<QualitySettingsManager>().OnQualityChanged += QualityCamera_OnQualityChanged;
    }

    private void QualityCamera_OnQualityChanged(RenderTexture currentQualityTexture)
    {
        _camera.targetTexture = currentQualityTexture;
    }
}
