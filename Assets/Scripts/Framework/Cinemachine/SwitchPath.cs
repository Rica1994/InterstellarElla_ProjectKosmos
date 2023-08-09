using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SwitchPath : MonoBehaviour
{
    [SerializeField] private CinemachineSmoothPath _fromSmoothPath;
    [SerializeField] private CinemachineSmoothPath _toSmoothPath;
    private int _closestWaypointIndex = -1;

    private bool _hitTrigger = false;
    private CinemachineDollyCart _playerDollyCart;

    public void SetPathDestination(CinemachinePathBase path)
    {
        var smoothPath = path as CinemachineSmoothPath;
        if (!_toSmoothPath && smoothPath)
        {
            _toSmoothPath = smoothPath;
        }
    }

    private int FindClosestWaypointIndex(Vector3 position, CinemachineSmoothPath path)
    {
        int closestWaypointIndex = 0;
        float smallestMagnitude = float.MaxValue;

        // Check each waypoint which one is closer
        for (int i = 0; i < path.m_Waypoints.Length; i++)
        {
            float magnitude = Vector3.Magnitude(position - path.m_Waypoints[i].position);

            // Check if current waypoint magnitude is closer than previous
            if (magnitude < smallestMagnitude)
            {
                smallestMagnitude = magnitude;
                closestWaypointIndex = i;
            }
        }

        return closestWaypointIndex;
    }

    private void OnTriggerEnter(Collider other)
    {
        _playerDollyCart = other.gameObject.GetComponentInParent<CinemachineDollyCart>();
        if (!_playerDollyCart || _playerDollyCart.m_Path != _fromSmoothPath)
        {
            return;
        }

        // Find closest waypoint from current location on this path
        if (_closestWaypointIndex == -1)
        {
            _closestWaypointIndex = FindClosestWaypointIndex(other.gameObject.transform.position, _fromSmoothPath);
        }

        _hitTrigger = true;
    }

    private void Update()
    {
        if (!_hitTrigger || !_toSmoothPath || !_playerDollyCart)
        {
            return;
        }

        // Closest waypoint in position coordinates
        var distOfWaypoint = _playerDollyCart.m_Path.FromPathNativeUnits(_closestWaypointIndex, CinemachinePathBase.PositionUnits.Distance);

        // Has player reached waypoint
        if (_playerDollyCart.m_Position >= distOfWaypoint)
        {
            Debug.Log("Swapped path");

            // Switch track
            _playerDollyCart.m_Path = _toSmoothPath;
            // Find point on other track
            var targetPoint = _toSmoothPath.FindClosestPoint(_playerDollyCart.gameObject.transform.position, 0, -1, 50);
            // Set position on other track
            _playerDollyCart.m_Position = _playerDollyCart.m_Path.FromPathNativeUnits(targetPoint, CinemachinePathBase.PositionUnits.Distance); ;

             // !! [DISABLED WITH NEW CAM] !! //
            // Adjust priority in cameras so new track camera is active
            //var virtualCamera = _toSmoothPath.gameObject.GetComponentInChildren<CinemachineVirtualCamera>(true);
            //if (virtualCamera)
            //{
            //    virtualCamera.gameObject.SetActive(true);
            //    virtualCamera.MoveToTopOfPrioritySubqueue();
            //}

            Destroy(gameObject);
        }
    }
}
