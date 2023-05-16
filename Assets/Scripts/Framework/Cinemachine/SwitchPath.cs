using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPath : MonoBehaviour
{
    [SerializeField] private CinemachineSmoothPath _fromSmoothPath;
    [SerializeField] private CinemachineSmoothPath _toSmoothPath;

    private float _waypointSearchPrecision = 0.2f;
    private int _closestWaypointIndex = -1;

    private int FindClosestWaypointIndex(GameObject go)
    {
        int closestWaypointIndex = 0;
        float smallestMagnitude = float.MaxValue;
        for (int i = 0; i < _fromSmoothPath.m_Waypoints.Length; i++)
        {
            float magnitude = Vector3.Magnitude(go.transform.position - _fromSmoothPath.m_Waypoints[i].position);
            if (magnitude < smallestMagnitude)
            {
                smallestMagnitude = magnitude;
                closestWaypointIndex = i;
            }
        }
        return closestWaypointIndex;
    }

    private void OnTriggerStay(Collider other)
    {
        var cart = other.gameObject.GetComponentInParent<CinemachineDollyCart>();
        if (!cart)
        {
            return;
        }

        // Find closest waypoint once
        if (_closestWaypointIndex == -1)
        {
            _closestWaypointIndex = FindClosestWaypointIndex(other.gameObject);
        }
        
        // How far is player away from this waypoint?
        var distToWaypoint = cart.m_Path.FromPathNativeUnits(_closestWaypointIndex, CinemachinePathBase.PositionUnits.Distance);

        // Player reached waypoint
        if (cart.m_Position >= distToWaypoint)
        {
            var targetPoint = _toSmoothPath.FindClosestPoint(other.gameObject.transform.position, 0, -1, 50);
            cart.m_Path = _toSmoothPath;
            cart.m_Position = cart.m_Path.FromPathNativeUnits(targetPoint, CinemachinePathBase.PositionUnits.Distance); ;
            
            var virtualCamera = _toSmoothPath.gameObject.GetComponentInChildren<CinemachineVirtualCamera>();
            if (virtualCamera)
            {
                virtualCamera.MoveToTopOfPrioritySubqueue();
            }

            Destroy(gameObject);
        }
    }
}
