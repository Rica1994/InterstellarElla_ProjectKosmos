using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCameraSpeederGround : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;

    [SerializeField]
    private float _blendSpeed = 3f;

    [SerializeField]
    private bool _isStatic = false;


    private void Start()
    {
        // disable the camera in the start
        _virtualCamera.gameObject.SetActive(false);

        // parent camera to manager
        _virtualCamera.transform.SetParent(ServiceLocator.Instance.GetService<VirtualCameraManagerSpeederGround>().transform);

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
