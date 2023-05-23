using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPathEnd : MonoBehaviour
{
    private CinemachineSmoothPath _fromSmoothPath;
    [SerializeField] private CinemachineSmoothPath _toSmoothPath;
    private int _endWaypointIndex = -1;

    private CinemachineDollyCart _playerDollyCart;

    public void SetPathDestination(CinemachinePathBase path)
    {
        var smoothPath = path as CinemachineSmoothPath;
        if (!_toSmoothPath && smoothPath)
        {
            _toSmoothPath = smoothPath;
        }
    }

    private void Start()
    {
        _playerDollyCart = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<CinemachineDollyCart>();
        _fromSmoothPath = GetComponent<CinemachineSmoothPath>();
    }

    private void Update()
    {
        if (!_playerDollyCart || _playerDollyCart.m_Path != _fromSmoothPath)
        {
            return;
        }

        // Find end waypoint
        _endWaypointIndex = _fromSmoothPath.m_Waypoints.Length - 1;

        if (!_toSmoothPath || !_playerDollyCart)
        {
            return;
        }

        // How far is player away from this waypoint?
        var distToWaypoint = _playerDollyCart.m_Path.FromPathNativeUnits(_endWaypointIndex, CinemachinePathBase.PositionUnits.Distance);

        // Has player reached waypoint
        if (_playerDollyCart.m_Position >= distToWaypoint)
        {
            // Switch track
            _playerDollyCart.m_Path = _toSmoothPath;
            // Find point on other track
            var targetPoint = _toSmoothPath.FindClosestPoint(_playerDollyCart.gameObject.transform.position, 0, -1, 50);
            // Set position on other track
            _playerDollyCart.m_Position = _playerDollyCart.m_Path.FromPathNativeUnits(targetPoint, CinemachinePathBase.PositionUnits.Distance); ;

            // Adjust priority in cameras so new track camera is active
            var virtualCamera = _toSmoothPath.gameObject.GetComponentInChildren<CinemachineVirtualCamera>(true);
            if (virtualCamera)
            {
                virtualCamera.gameObject.SetActive(true);
                virtualCamera.MoveToTopOfPrioritySubqueue();
            }
        }
    }
}
