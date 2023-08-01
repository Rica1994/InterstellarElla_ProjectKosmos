using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPickups : MonoBehaviour
{
    public CinemachineSmoothPath SmoothPath;

    public List<GameObject> PickupsOnPath = new List<GameObject>();
    public List<CinemachineSmoothPath.Waypoint> WaypointsOnPath = new List<CinemachineSmoothPath.Waypoint>();

    [SerializeField]
    private GameObject _pickupPrefab;



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


    /// <summary>
    /// for each waypoint -> add + instantiate a pickup
    /// </summary>
    public void CreatePickups()
    {
        for (int j = 0; j < WaypointsOnPath.Count; j++)
        {
            CinemachineSmoothPath.Waypoint waypointOfInterest = WaypointsOnPath[j];
            GameObject instantiatedPickup = Instantiate(_pickupPrefab, this.transform.position + waypointOfInterest.position, Quaternion.identity);

            PickupsOnPath.Add(instantiatedPickup);
        }
    }
}
