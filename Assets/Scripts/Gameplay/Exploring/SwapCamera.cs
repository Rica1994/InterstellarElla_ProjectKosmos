using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class SwapCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject _target;

    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;

    [SerializeField]
    private float _blendSpeed = 3f;


    private void Start()
    {
        // disable the camera in the start
        _virtualCamera.gameObject.SetActive(false);

        // parent camera to manager
        _virtualCamera.transform.SetParent(ServiceLocator.Instance.GetService<VirtualCameraManagerExploring>().transform);

        // unparent the visuals (keep arrow in place)
        _target.transform.GetChild(0).transform.SetParent(this.transform);       
    }


    public void SwapToNewVirtualCamera(EllaExploring exploringScript)
    {
        if (_virtualCamera.gameObject.activeSelf == false)
        {
            Debug.Log("Triggered the sawpping to a new cam");

            // parent the target to the player 
            _target.transform.SetParent(exploringScript.CameraTargets.transform);
            _target.transform.localPosition = new Vector3(0, 0, 0);

            // access manager for swapCamera logic
            ServiceLocator.Instance.GetService<VirtualCameraManagerExploring>().SwapCamera(_virtualCamera, _blendSpeed);
        }
    }
}
