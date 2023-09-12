using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraManager : Service
{
    [SerializeField] private Transform _followTarget;
    [SerializeField] private Transform _lookAtTarget;

    [SerializeField] private float _defaultFov = 75f;
    [SerializeField] private float _zoomOutFov = 90f;

    [SerializeField] private float _zoomSpeed = 1f;

    private List<CinemachineVirtualCamera> _virtualCameras = new List<CinemachineVirtualCamera>();

    [Header("new camera following player")]
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamFollow;

    private bool _isResettingZoom = false;
    private bool _isZoomingOut = false;

    private float _currentFov;

    private void Awake()
    {
        _currentFov = _defaultFov;
    }

    private void Start()
    {
        var virtualCameras = FindObjectsOfType<CinemachineVirtualCamera>();
        _virtualCameras.AddRange(virtualCameras);

        CinemachineVirtualCamera cameraPlayerFollow = null;

        // remove the camera that specifically follows the player
        for (int i = 0; i < _virtualCameras.Count; i++)
        {
            if (virtualCameras[i].TryGetComponent(out CameraPlayerFollow camFollow) == true)
            {
                cameraPlayerFollow = virtualCameras[i];
                _virtualCameras.Remove(virtualCameras[i]);
            }
        }

        foreach (var camera in _virtualCameras)
        {
            camera.Follow = _followTarget;
            camera.LookAt = _lookAtTarget;
            camera.m_Lens.FieldOfView = _defaultFov;
        }


        // !!! set priority higher of new follow cam !!!
        if (cameraPlayerFollow != null)
        {
            cameraPlayerFollow.MoveToTopOfPrioritySubqueue();
            cameraPlayerFollow.m_Priority = 10;
        }
    }

    public void AddVirtualCamera(CinemachineVirtualCamera camera)
    {
        if (!_virtualCameras.Contains(camera))
        {
            camera.m_Lens.FieldOfView = _currentFov;
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
            LerpNew(_defaultFov, ref _isResettingZoom, _zoomSpeed);
        }
        else if (_isZoomingOut)
        {
            LerpNew(_zoomOutFov, ref _isZoomingOut, _zoomSpeed);
        }
    }




    //private void Lerp(float targetFov, ref bool isLerping, float lerpSpeed)
    //{
    //    if (Mathf.Abs(_currentFov - targetFov) > .1f)
    //    {
    //        _currentFov = Mathf.Lerp(_currentFov, targetFov, Time.deltaTime * lerpSpeed);
    //        foreach (var camera in _virtualCameras)
    //        {
    //            camera.m_Lens.FieldOfView = _currentFov;
    //        }
    //    }
    //    else
    //    {
    //        _currentFov = targetFov;
    //        foreach (var camera in _virtualCameras)
    //        {
    //            camera.m_Lens.FieldOfView = _currentFov;
    //        }
    //        isLerping = false;
    //    }
    //}
    private void LerpNew(float targetFov, ref bool isLerping, float lerpSpeed)
    {
        if (Mathf.Abs(_currentFov - targetFov) > .1f)
        {
            _currentFov = Mathf.Lerp(_currentFov, targetFov, Time.deltaTime * lerpSpeed);

            _virtualCamFollow.m_Lens.FieldOfView = _currentFov;
        }
        else
        {
            _currentFov = targetFov;

            _virtualCamFollow.m_Lens.FieldOfView = _currentFov;

            isLerping = false;
        }
    }
}
