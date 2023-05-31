using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraManager : Service
{
    [SerializeField] private Transform _followTarget;
    [SerializeField] private Transform _lookAtTarget;

    [SerializeField] private float _defaultFov;
    [SerializeField] private float _zoomOutFov;

    [SerializeField] private float _zoomSpeed = 1f;

    private List<CinemachineVirtualCamera> _virtualCameras = new List<CinemachineVirtualCamera>();

    private bool _isResettingZoom = false;
    private bool _isZoomingOut = false;

    private float _currentFov;

    private void Awake()
    {
        _currentFov = _defaultFov;
    }

    private void Start()
    {
        var virtualCameras = FindObjectsByType<CinemachineVirtualCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        _virtualCameras.AddRange(virtualCameras);

        foreach (var camera in _virtualCameras)
        {
            camera.Follow = _followTarget;
            camera.LookAt = _lookAtTarget;
            camera.m_Lens.FieldOfView = _defaultFov;
        }
    }

    public void AddVirtualCamera(CinemachineVirtualCamera camera)
    {
        if (!_virtualCameras.Contains(camera))
        {
            _virtualCameras.Add(camera);
        }
    }

    public void RemoveVirtualCamera(CinemachineVirtualCamera camera)
    {
        _virtualCameras.Remove(camera);
    }

    public void ZoomOut()
    {
        _isZoomingOut = true;
        _isResettingZoom = false;
    }

    public void ResetZoom()
    {
        _isResettingZoom = true;
        _isZoomingOut = false;
    }

    private void Update()
    {
        if (_isResettingZoom)
        {
            Lerp(_defaultFov, ref _isResettingZoom, _zoomSpeed);
        }
        else if (_isZoomingOut)
        {
            Lerp(_zoomOutFov, ref _isZoomingOut, _zoomSpeed);
        }
    }

    private void Lerp(float targetFov, ref bool isLerping, float lerpSpeed)
    {
        if (Mathf.Abs(_currentFov - targetFov) > .1f)
        {
            _currentFov = Mathf.Lerp(_currentFov, targetFov, Time.deltaTime * lerpSpeed);
            foreach (var camera in _virtualCameras)
            {
                camera.m_Lens.FieldOfView = _currentFov;
            }
        }
        else
        {
            _currentFov = targetFov;
            foreach (var camera in _virtualCameras)
            {
                camera.m_Lens.FieldOfView = _currentFov;
            }
            isLerping = false;
        }
    }
}
