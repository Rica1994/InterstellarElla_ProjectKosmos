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

    private bool _runningZoomCoroutine;

    private float _originalCameraDistance_1, _originalCameraDistance_2;

    private float _currentZoomFactor;


    private void Start()
    {
        // add current virtual camera to active virtual camera (list for zooming)
        _currentlyActiveVirtualCameras.Add(_firstVirtualCam);

        var thirdPersonFollow = _firstVirtualCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        var framingTransposer = _firstVirtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (thirdPersonFollow != null)
        {
            // set original cam distance
            _originalCameraDistance_1 = thirdPersonFollow.CameraDistance;

            // add the 3rd person component to a list as well
            _virtualCameraFollows.Add(thirdPersonFollow);
        }
        else if (framingTransposer != null)
        {
            // set original cam distance
            _originalCameraDistance_1 = framingTransposer.m_CameraDistance;

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
    public void SwapCutsceneCamera(CinemachineVirtualCamera virtualCamToActivate, float cutsceneLength)
    {
        StartCoroutine(CutsceneRoutine(virtualCamToActivate, cutsceneLength));
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
            _originalCameraDistance_2 = thirdPersonFollow.CameraDistance;
            //  set this new cameras 'CameraDistance' to its value + the currently adjusted zoom (in case we get a camera swap mid-zoom)
            thirdPersonFollow.CameraDistance += _currentZoomFactor;

            // add the 3rd person component to a list as well
            _virtualCameraFollows.Add(virtualCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>());
        }
        else if(framingTransposer != null)
        {
            // set original cam distance
            _originalCameraDistance_2 = framingTransposer.m_CameraDistance;
            //  set this new cameras 'CameraDistance' to its value + the currently adjusted zoom (in case we get a camera swap mid-zoom)
            framingTransposer.m_CameraDistance += _currentZoomFactor;

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
    /// Activates a coroutine which will affect the Camera Distance on the virtual cameras that matter (typically 1 or 2)
    /// </summary>
    /// <param name="zoomDuration"></param>
    /// <param name="zoomFactor"></param>
    /// <param name="zoomLimit"></param>
    public void ZoomOutCamera(float zoomDuration = 3f, float zoomFactor = 0.01f, float zoomLimit = 1f)
    {
        // NOTE : currently would have the issue of stacking zooms...
        if (_runningZoomCoroutine == true)
        {
            Debug.LogWarning("A previous zoom has not been finished ! Current zoom not being applied");
            return;
        }

        StartCoroutine(ZoomOutRoutine(zoomDuration, zoomFactor, zoomLimit));      
    }



    private IEnumerator CutsceneRoutine(CinemachineVirtualCamera virtualCam, float cutsceneLength)
    {
        // set cinemachine brain blending to cut
        _camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;

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
        yield return new WaitForSeconds(_camBrain.m_DefaultBlend.m_Time + 1f);

        if (_currentlyActiveVirtualCameras.Count > 1)
        {
            GameObject previousCamera = _currentlyActiveVirtualCameras[0].gameObject;           
            previousCamera.SetActive(false);

            if (_virtualCameraFollows.Count > 0)
            {
                // set camera distance to original of camera getting removed
                _virtualCameraFollows[0].CameraDistance = _originalCameraDistance_1;

                _virtualCameraFollows.RemoveAt(0);
            }
            else if (_virtualCameraFramingTransposers.Count > 0)
            {
                // set camera distance to original of camera getting removed
                _virtualCameraFramingTransposers[0].m_CameraDistance = _originalCameraDistance_1;

                _virtualCameraFramingTransposers.RemoveAt(0);
            }

            // re-place the camera distance value of the 1st float
            _originalCameraDistance_1 = _originalCameraDistance_2;

            _currentlyActiveVirtualCameras.RemoveAt(0);      
        }
    }

    
    private IEnumerator ZoomOutRoutine(float zoomDuration = 3f, float zoomFactor = 0.05f, float zoomLimit = 1f)
    {
        _runningZoomCoroutine = true;
        float timePassed = 0f;

        // start zooming out
        while (_currentZoomFactor < zoomLimit)
        {
            _currentZoomFactor += zoomFactor;
            timePassed += Time.deltaTime;

            //for (int i = 0; i < _currentlyActiveVirtualCameras.Count; i++)
            //{
            //    _virtualCameraFollows[i].CameraDistance += zoomFactor;
            //}
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
        while (_currentZoomFactor > 0)
        {
            _currentZoomFactor -= zoomFactor;
            //for (int i = 0; i < _currentlyActiveVirtualCameras.Count; i++)
            //{
            //    _virtualCameraFollows[i].CameraDistance -= zoomFactor;
            //}
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

        _runningZoomCoroutine = false;
    }

    
}
