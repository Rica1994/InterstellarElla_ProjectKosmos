using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEngine;

public class AttachToWaypoint : MonoBehaviour
{
    [SerializeField] private CinemachineSmoothPath _pathToAttachTo;
    [SerializeField] private CinemachineSmoothPath _pathToAttachFrom;

    private void Awake()
    {
        CinemachineSmoothPath.Waypoint[] waypointsFrom = _pathToAttachFrom.m_Waypoints;
        if (waypointsFrom.Length <= 0)
        {
            return;
        }

        // Get the world position of waypoint 0 
        Vector3 worldPosition = waypointsFrom[0].position + _pathToAttachFrom.gameObject.transform.position;
        // Find the closest path on the other track, close to waypoint 0
        float targetStartDistance = _pathToAttachTo.FindClosestPoint(worldPosition, 0, -1, 50);

        // Get the world position of waypoint last
        worldPosition = waypointsFrom[waypointsFrom.Length - 1].position + _pathToAttachFrom.gameObject.transform.position;
        // Find the closest path on the other track, close to waypoint last
        float targetEndDistance = _pathToAttachTo.FindClosestPoint(worldPosition, 0, -1, 50);

        List<CinemachineSmoothPath.Waypoint> newWaypoints = new List<CinemachineSmoothPath.Waypoint>();
        newWaypoints.AddRange(waypointsFrom);

        CinemachineSmoothPath.Waypoint newPoint = new CinemachineSmoothPath.Waypoint();

        // Get position from targetstart point in path units
        newPoint.position = _pathToAttachTo.EvaluatePositionAtUnit(targetStartDistance, CinemachinePathBase.PositionUnits.PathUnits);
        // Transform position to local space
        newPoint.position -= _pathToAttachFrom.transform.position;

        // Replace first waypoint with new waypoint
        newWaypoints[0] = newPoint;

        // Get position from target end 
        newPoint.position = _pathToAttachTo.EvaluatePositionAtUnit(targetEndDistance, CinemachinePathBase.PositionUnits.PathUnits);
        // Transform position to local space
        newPoint.position -= _pathToAttachFrom.transform.position;

        // Replace last waypoint with new wayopint
        newWaypoints[newWaypoints.Count - 1] = newPoint;

        // Convert list to array and save as waypoints
        _pathToAttachFrom.m_Waypoints = newWaypoints.ToArray();
    }
}
