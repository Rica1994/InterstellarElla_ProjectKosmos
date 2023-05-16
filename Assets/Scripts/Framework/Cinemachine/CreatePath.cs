using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePath : MonoBehaviour
{
    [SerializeField] private CinemachineSmoothPath _baseSmoothPath;
    [SerializeField] private List<int> _wayPoints;
    
    private CinemachineSmoothPath _smoothPath;

    private void Start()
    {
        _smoothPath = GetComponent<CinemachineSmoothPath>();

        List<CinemachineSmoothPath.Waypoint> waypoints = new List<CinemachineSmoothPath.Waypoint>();
        foreach (int waypoint in _wayPoints)
        {
            waypoints.Add(_baseSmoothPath.m_Waypoints[waypoint]);
        }
        _smoothPath.m_Waypoints = waypoints.ToArray();
    }
}
