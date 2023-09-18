using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class SwapCameraSpeederGround : SwapCameraBase
{
    public override void Start()
    {
        base.Start();

        //    // disable the camera in the start
        //    _virtualCamera.gameObject.SetActive(false);

        //    // parent camera to manager
        //    _virtualCamera.transform.SetParent(ServiceLocator.Instance.GetService<VirtualCameraManagerSpeederGround>().transform);
    }


    // THIS
    public void SwapToNewVirtualCamera(SpeederGround speeder)
    {
        if (_virtualCamera.gameObject.activeSelf == false)
        {
            Debug.Log("Triggered the sawpping to a new cam");

            _virtualCamera.gameObject.SetActive(true);
            _virtualCamera.Follow = speeder.transform;
            _virtualCamera.LookAt = speeder.transform;

            // access manager for swapCamera logic
            ServiceLocator.Instance.GetService<VirtualCameraManagerSpeederGround>().SwapCamera(_virtualCamera, _blendSpeed);
        }
    }
}
