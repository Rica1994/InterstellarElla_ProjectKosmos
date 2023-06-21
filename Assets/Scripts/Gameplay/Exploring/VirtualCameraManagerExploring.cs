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

    private float _originalCameraDistance_1, _originalCameraDistance_2;

    private float _currentZoomFactor;


    private void Start()
    {
        var thirdPersonFollow = _firstVirtualCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

        // set original cam distance
        _originalCameraDistance_1 = thirdPersonFollow.CameraDistance;
        // add current virtual camera to active virtual camera (list for zooming)
        _currentlyActiveVirtualCameras.Add(_firstVirtualCam);
        // add the 3rd person component to a list as well
        _virtualCameraFollows.Add(thirdPersonFollow);

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
        var thirdPersonFollow = virtualCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        // set original cam distance
        _originalCameraDistance_2 = thirdPersonFollow.CameraDistance;
        //  set this new cameras 'CameraDistance' to its value + the currently adjusted zoom (in case we get a camera swap mid-zoom)
        thirdPersonFollow.CameraDistance += _currentZoomFactor;

        // set the blend speed
        _camBrain.m_DefaultBlend.m_Time = blendSpeed;

        // enable the new camera
        virtualCam.gameObject.SetActive(true);
        virtualCam.MoveToTopOfPrioritySubqueue();

        // add current virtual camera to active virtual camera (list for zooming)
        _currentlyActiveVirtualCameras.Add(virtualCam);
        // add the 3rd person component to a list as well
        _virtualCameraFollows.Add(virtualCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>());

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

        yield return new WaitForSeconds(0.5f);

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

            // set camera distance to original of camera getting removed
            _virtualCameraFollows[0].CameraDistance = _originalCameraDistance_1;
            // re-place the camera distance value of the 1st float
            _originalCameraDistance_1 = _originalCameraDistance_2;

            _currentlyActiveVirtualCameras.RemoveAt(0);
            _virtualCameraFollows.RemoveAt(0);
        }
    }

    
    private IEnumerator ZoomOutRoutine(float zoomDuration = 3f, float zoomFactor = 0.05f, float zoomLimit = 1f)
    {
        float timePassed = 0f;

        // start zooming out
        while (_currentZoomFactor < zoomLimit)
        {
            _currentZoomFactor += zoomFactor;
            timePassed += Time.deltaTime;

            for (int i = 0; i < _currentlyActiveVirtualCameras.Count; i++)
            {
                _virtualCameraFollows[i].CameraDistance += zoomFactor;
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
            for (int i = 0; i < _currentlyActiveVirtualCameras.Count; i++)
            {
                _virtualCameraFollows[i].CameraDistance -= zoomFactor;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    
}
