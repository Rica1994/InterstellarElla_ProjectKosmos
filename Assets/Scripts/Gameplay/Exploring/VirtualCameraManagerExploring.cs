using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraManagerExploring : Service
{
    [Header("Brain")]
    [SerializeField]
    private CinemachineBrain _camBrain;

    [Header("Starting virtual camera")]
    [SerializeField]
    private CinemachineVirtualCamera _firstVirtualCam;

    private List<CinemachineVirtualCamera> _currentlyActiveVirtualCameras = new List<CinemachineVirtualCamera>();
    private List<Cinemachine3rdPersonFollow> _virtualCameraFollows = new List<Cinemachine3rdPersonFollow>();
    private List<CinemachineFramingTransposer> _virtualCameraFramingTransposers = new List<CinemachineFramingTransposer>();

    private bool _runningZoomDistanceCoroutine, _runningZoomFOVCoroutine;

    private float _originalCameraDistance, _newCameraDistance;
    private float _originalCameraFOV, _newCameraFOV;

    private float _currentZoomDistanceFactor, _currentZoomFOVFactor;


    private void Start()
    {
        // add current virtual camera to active virtual camera (list for zooming)
        _currentlyActiveVirtualCameras.Add(_firstVirtualCam);

        var thirdPersonFollow = _firstVirtualCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        var framingTransposer = _firstVirtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (thirdPersonFollow != null)
        {
            // set original cam distance
            _originalCameraDistance = thirdPersonFollow.CameraDistance;
            // set original cam fov
            _originalCameraFOV = _firstVirtualCam.m_Lens.FieldOfView;

            // add the 3rd person component to a list as well
            _virtualCameraFollows.Add(thirdPersonFollow);
        }
        else if (framingTransposer != null)
        {
            // set original cam distance
            _originalCameraDistance = framingTransposer.m_CameraDistance;
            // set original cam fov
            _originalCameraFOV = _firstVirtualCam.m_Lens.FieldOfView;

            // add the framing transposer component to a list as well
            _virtualCameraFramingTransposers.Add(framingTransposer);
        }

        // parent the virtual camera under the manager
        _firstVirtualCam.transform.SetParent(this.transform);

        // reset priority of first cam (if all virtual cameras are not on same priority, blending seems to have issues)
        _firstVirtualCam.Priority = 10;
    }


    /// <summary>
    /// Should cut from our current 3rd person virtual camera to a new virtual camera used in a cutscene, and also reset back to the other camera at the end.
    /// </summary>
    /// <param name="virtualCamToActivate"></param>
    public void SwapCutsceneCamera(CinemachineVirtualCamera virtualCamToActivate, float cutsceneLength, bool smoothCameraBlend = false)
    {
        StartCoroutine(CutsceneRoutine(virtualCamToActivate, cutsceneLength, smoothCameraBlend));
    }

    /// <summary>
    /// Starts blending from our current 3rd person virtual camera to a new 3rd person virtual camera
    /// </summary>
    /// <param name="virtualCam"></param>
    public void SwapCamera(CinemachineVirtualCamera virtualCam, float blendSpeed)
    {
        // add current virtual camera to active virtual camera (list for zooming)
        _currentlyActiveVirtualCameras.Add(virtualCam);

        // check for framing transposer or thirdperson camera
        var thirdPersonFollow = virtualCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        var framingTransposer = virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();


        if (thirdPersonFollow != null)
        {
            // set original cam distance
            _newCameraDistance = thirdPersonFollow.CameraDistance;
            //  set this new cameras 'CameraDistance' to its value + the currently adjusted zoom (in case we get a camera swap mid-zoom)
            thirdPersonFollow.CameraDistance += _currentZoomDistanceFactor;
            // also store/set the FOV
            _newCameraFOV = virtualCam.m_Lens.FieldOfView;
            virtualCam.m_Lens.FieldOfView += _currentZoomFOVFactor;

            // add the 3rd person component to a list as well
            _virtualCameraFollows.Add(virtualCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>());
        }
        else if(framingTransposer != null)
        {
            // set original cam distance
            _newCameraDistance = framingTransposer.m_CameraDistance;
            //  set this new cameras 'CameraDistance' to its value + the currently adjusted zoom (in case we get a camera swap mid-zoom)
            framingTransposer.m_CameraDistance += _currentZoomDistanceFactor;
            // also store/set the FOV
            _newCameraFOV = virtualCam.m_Lens.FieldOfView;
            virtualCam.m_Lens.FieldOfView += _currentZoomFOVFactor;

            // add the framing transposer component to a list as well
            _virtualCameraFramingTransposers.Add(framingTransposer);
        }
        
        // set the blend speed
        _camBrain.m_DefaultBlend.m_Time = blendSpeed;

        // enable the new camera
        virtualCam.gameObject.SetActive(true);
        virtualCam.MoveToTopOfPrioritySubqueue();

        // this will remove the previous camera once the blending time on Brain has passed
        StartCoroutine(RemoveCameraAfterBlend());
    }




    /// <summary>
    /// Activates a coroutine which will affect the Camera Distance on the virtual cameras that matter (typically 1 or 2 cameras affected)
    /// </summary>
    /// <param name="zoomDuration"></param>
    /// <param name="zoomFactor"></param>
    /// <param name="zoomLimit"></param>
    public void ZoomOutCameraDistance(float zoomDuration = 3f, float zoomFactor = 0.01f, float zoomLimit = 1f)
    {
        // NOTE : currently would have the issue of stacking zooms...
        if (_runningZoomDistanceCoroutine == true)
        {
            Debug.LogWarning("A previous zoom has not been finished ! Current zoom not being applied");
            return;
        }

        StartCoroutine(ZoomOutDistanceRoutine(zoomDuration, zoomFactor, zoomLimit));      
    }

    /// <summary>
    /// Activates a coroutine which will affect the Camera FOV on the virtual cameras that matter (typically 1 or 2 cameras affected)
    /// </summary>
    /// <param name="zoomDuration"></param>
    /// <param name="zoomFactor"></param>
    /// <param name="zoomLimit"></param>
    public void ZoomOutCameraFOV(float zoomDuration = 1.5f, float zoomFactor = 0.5f, float zoomLimit = 15f)
    {
        // NOTE : currently would have the issue of stacking zooms...
        if (_runningZoomFOVCoroutine == true)
        {
            Debug.LogWarning("A previous zoom has not been finished ! Current zoom not being applied");
            return;
        }

        StartCoroutine(ZoomOutFOVRoutine(zoomDuration, zoomFactor, zoomLimit));
    }


    private IEnumerator CutsceneRoutine(CinemachineVirtualCamera virtualCam, float cutsceneLength, bool smoothCameraBlend = false)
    {
        if (smoothCameraBlend)
        {

        }
        else
        {
            // set cinemachine brain blending to cut
            _camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
        }

        // activate/prioritize camera 
        virtualCam.MoveToTopOfPrioritySubqueue();

        yield return new WaitForSeconds(cutsceneLength);

        // re-enable the 3rd person virtual cam and/or move to top of stack
        _currentlyActiveVirtualCameras[0].MoveToTopOfPrioritySubqueue();
        virtualCam.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        // reset blending mode
        _camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
    }
    private IEnumerator RemoveCameraAfterBlend()
    {
        yield return new WaitForSeconds(_camBrain.m_DefaultBlend.m_Time + 0.1f);

        if (_currentlyActiveVirtualCameras.Count > 1)
        {
            CinemachineVirtualCamera previousCamera = _currentlyActiveVirtualCameras[0];           
            previousCamera.gameObject.SetActive(false);

            // check for framing transposer or thirdperson camera
            Cinemachine3rdPersonFollow thirdPersonFollowPreviousCam = previousCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            CinemachineFramingTransposer framingTransposerPreviousCam;
            if (thirdPersonFollowPreviousCam != null)
            {
                thirdPersonFollowPreviousCam.CameraDistance = _originalCameraDistance;
                _virtualCameraFollows.RemoveAt(0);
            }
            else
            {
                framingTransposerPreviousCam = previousCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

                if (framingTransposerPreviousCam != null)
                {
                    framingTransposerPreviousCam.m_CameraDistance = _originalCameraDistance;
                }

                if (!_virtualCameraFramingTransposers[0])
                {
                    _virtualCameraFramingTransposers.RemoveAt(0);              
                }
            }
            previousCamera.m_Lens.FieldOfView = _originalCameraFOV;

            // re-fresh the camera distance value 
            _originalCameraDistance = _newCameraDistance;
            // re-fresh the cam FOV
            _originalCameraFOV = _newCameraFOV;

            _currentlyActiveVirtualCameras.RemoveAt(0);      
        }
    }

    
    private IEnumerator ZoomOutDistanceRoutine(float zoomDuration = 3f, float zoomFactor = 0.05f, float zoomLimit = 1f)
    {
        _runningZoomDistanceCoroutine = true;
        float timePassed = 0f;


        // start zooming out
        while (_currentZoomDistanceFactor < zoomLimit)
        {
            _currentZoomDistanceFactor += zoomFactor;
            timePassed += Time.deltaTime;


            for (int i = 0; i < _virtualCameraFollows.Count; i++)
            {
                _virtualCameraFollows[i].CameraDistance += zoomFactor;
            }
            for (int i = 0; i < _virtualCameraFramingTransposers.Count; i++)
            {
                _virtualCameraFramingTransposers[i].m_CameraDistance += zoomFactor;
            }

            yield return new WaitForEndOfFrame();
        }


        // have a zoomed out camera (do nothing here)
        float adjustedZoomDuration = zoomDuration - (timePassed * 2);
        yield return new WaitForSeconds(adjustedZoomDuration);


        // zoom back in to original
        while (_currentZoomDistanceFactor > 0)
        {
            _currentZoomDistanceFactor -= zoomFactor;


            for (int i = 0; i < _virtualCameraFollows.Count; i++)
            {
                _virtualCameraFollows[i].CameraDistance -= zoomFactor;
            }
            for (int i = 0; i < _virtualCameraFramingTransposers.Count; i++)
            {
                _virtualCameraFramingTransposers[i].m_CameraDistance -= zoomFactor;
            }

            yield return new WaitForEndOfFrame();
        }

        _runningZoomDistanceCoroutine = false;
    }
    private IEnumerator ZoomOutFOVRoutine(float zoomDuration = 1.5f, float zoomFactor = 0.5f, float zoomLimit = 15f)
    {
        _runningZoomFOVCoroutine = true;
        float timePassed = 0f;

        // start zooming out
        while (_currentZoomFOVFactor < zoomLimit)
        {
            _currentZoomFOVFactor += zoomFactor;
            timePassed += Time.deltaTime;

            for (int i = 0; i < _currentlyActiveVirtualCameras.Count; i++)
            {
                _currentlyActiveVirtualCameras[i].m_Lens.FieldOfView += zoomFactor;
            }

            yield return new WaitForEndOfFrame();
        }


        // have a zoomed out camera (do nothing here)
        float adjustedZoomDuration = zoomDuration - (timePassed * 2);
        yield return new WaitForSeconds(adjustedZoomDuration);


        // zoom back in to original
        while (_currentZoomFOVFactor > 0)
        {
            _currentZoomFOVFactor -= zoomFactor;

            for (int i = 0; i < _currentlyActiveVirtualCameras.Count; i++)
            {
                _currentlyActiveVirtualCameras[i].m_Lens.FieldOfView -= zoomFactor;
            }

            yield return new WaitForEndOfFrame();
        }

        _runningZoomFOVCoroutine = false;
    }

}
