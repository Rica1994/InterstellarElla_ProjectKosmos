using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineVirtualCamera _boostCamera;

    public void ActivateCamera(SpeederSpace player)
    {
        _virtualCamera.gameObject.SetActive(true);
        _boostCamera.gameObject.SetActive(true);
        if (player.IsBoosting)
        {
            _virtualCamera.MoveToTopOfPrioritySubqueue();
        }
        else
        {
            _boostCamera.MoveToTopOfPrioritySubqueue();
        }
    }
}
