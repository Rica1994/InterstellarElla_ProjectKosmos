using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPath : MonoBehaviour
{
    [SerializeField] private CinemachineSmoothPath _targetSmoothPath;
    [SerializeField] private CinemachineSmoothPath _originalSmoothPath;
    [SerializeField] private float _smoothPathWaypoint;

    private void OnTriggerStay(Collider other)
    {
        var cart = other.gameObject.GetComponentInParent<CinemachineDollyCart>();
        if (!cart)
        {
            return;
        }

        var distToWaypoint = cart.m_Path.FromPathNativeUnits(_smoothPathWaypoint, CinemachinePathBase.PositionUnits.Distance);
        if (cart.m_Position >= distToWaypoint)
        {
            cart.m_Path = _targetSmoothPath;

            // Temp: switch camera from path -> better, have own camera per path and swtich between these
            var cmBrain = Camera.main.GetComponent<CinemachineBrain>();
            var virtualCamera = cmBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
            var dollyCamera = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
            dollyCamera.m_Path = _targetSmoothPath;
            
            
            Destroy(gameObject);
        }
    }
}
