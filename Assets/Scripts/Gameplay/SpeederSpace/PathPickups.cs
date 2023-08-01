
#if UNITY_EDITOR

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathPickups : MonoBehaviour
{
    public CinemachineSmoothPath SmoothPath;

    public List<GameObject> PickupsOnPath = new List<GameObject>();
    public List<CinemachineSmoothPath.Waypoint> WaypointsOnPath = new List<CinemachineSmoothPath.Waypoint>();

    [SerializeField]
    private GameObject _pickupPrefab;


    private Quaternion _rotationMe;
    private Quaternion _rotationSmoothPath;


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
            GameObject instantiatedPickup = PrefabUtility.InstantiatePrefab(_pickupPrefab) as GameObject;

            instantiatedPickup.transform.position = SmoothPath.transform.position + waypointOfInterest.position;

            // failed attempt at trying to find world coords //
            //Debug.Log(transform.forward);
            //var adjustedForwardVector = new Vector3((1f - transform.forward.x), (1f - transform.forward.y), (transform.forward.z));
            //var calculatedWorldPosWaypoint = new Vector3(waypointOfInterest.position.x * adjustedForwardVector.x,
            //                                             waypointOfInterest.position.y * adjustedForwardVector.y,
            //                                             waypointOfInterest.position.z * adjustedForwardVector.z);

            //instantiatedPickup.transform.position = this.transform.position + (calculatedWorldPosWaypoint);
            //

            //// This ONLY WORKS IF the rotation is not severe !!!
            ////
            //float closestPathFloat = SmoothPath.FindClosestPoint((this.transform.position + waypointOfInterest.position), 0, -1, 20);
            //Vector3 closestPathPosition = SmoothPath.EvaluatePosition(closestPathFloat);

            //instantiatedPickup.transform.position = closestPathPosition;
            ////

            instantiatedPickup.transform.SetParent(SmoothPath.transform);
            PickupsOnPath.Add(instantiatedPickup);
        }
    }


    public void StoreRotations()
    {
        _rotationMe = this.transform.rotation;
        _rotationSmoothPath = SmoothPath.transform.rotation;
    }
    public void ResetRotations()
    {
        this.transform.rotation = Quaternion.Euler(0,0,0);
        SmoothPath.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    public void RevertRotations()
    {
        this.transform.rotation = _rotationMe;
        SmoothPath.transform.rotation = _rotationSmoothPath;
    }


    public void DestroyMyChildrenPickups()
    {
        for (int i = 0; i < SmoothPath.transform.childCount; i++)
        {
            DestroyImmediate(SmoothPath.transform.GetChild(i).gameObject);
        }

        if (PickupsOnPath.Count > 0)
        {
            PickupsOnPath.Clear();
        }        
    }
}

#endif
