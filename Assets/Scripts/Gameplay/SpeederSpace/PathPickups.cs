using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPickups : MonoBehaviour
{
    public CinemachineSmoothPath SmoothPath;

    public List<PickUp> PickupsOnPath = new List<PickUp>();
    public List<CinemachineSmoothPath.Waypoint> WaypointsOnPath = new List<CinemachineSmoothPath.Waypoint>();

    public void GetWaypoints()
    {
        if (WaypointsOnPath.Count > 0)
        {
            WaypointsOnPath.Clear();
        }
        
        for (int i = 0; i < SmoothPath.m_Waypoints.Length; i++)
        {
            WaypointsOnPath.Add(SmoothPath.m_Waypoints[i]);
        }
    }


    public void CreatePickupsOnPath()
    {
        for (int i = 0; i < WaypointsOnPath.Count; i++)
        {

        }
    }
}
