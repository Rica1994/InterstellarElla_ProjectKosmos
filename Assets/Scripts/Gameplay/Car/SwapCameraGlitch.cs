using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(-100)]
public class SwapCameraGlitch : SwapCameraBase
{
    public void SwapToNewVirtualCamera(SimpleCarController glitch)
    {
        if (_virtualCamera.gameObject.activeSelf == false)
        {
            Debug.Log("Triggered the swapping to a new cam");

            if (_isStatic == true)
            {
                // do nothing in particular with the target
            }
            else
            {
                // parent the target to the player 
                _target.transform.SetParent(glitch.CamerasParent.transform);
                _target.transform.localPosition = new Vector3(0, 0, 0);
            }

            glitch.ChangeCamera(_target);

            // access manager for swapCamera logic
            ServiceLocator.Instance.GetService<VirtualCameraManagerExploring>().SwapCamera(_virtualCamera, _blendSpeed);
        }
    }
}
