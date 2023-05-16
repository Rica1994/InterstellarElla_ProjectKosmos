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
        var worldPosition = waypointsFrom[0].position + _pathToAttachFrom.gameObject.transform.position;
        var targetStartPoint = _pathToAttachTo.FindClosestPoint(worldPosition, 0, -1, 50);

        worldPosition = waypointsFrom[waypointsFrom.Length - 1].position + _pathToAttachFrom.gameObject.transform.position;
        var targetEndPoint = _pathToAttachTo.FindClosestPoint(worldPosition, 0, -1, 50);

        List<CinemachineSmoothPath.Waypoint> newWaypoints = new List<CinemachineSmoothPath.Waypoint>();

        CinemachineSmoothPath.Waypoint newPoint = new CinemachineSmoothPath.Waypoint();

        // Get position from targetstart point in path units
        newPoint.position = _pathToAttachTo.EvaluatePositionAtUnit(targetStartPoint, CinemachinePathBase.PositionUnits.PathUnits);
        newPoint.position -= _pathToAttachFrom.transform.position;

        newWaypoints.AddRange(waypointsFrom);
        newWaypoints.RemoveAt(0);
        newWaypoints.RemoveAt(newWaypoints.Count - 1);

        newWaypoints.Insert(0, newPoint);
        newPoint.position = _pathToAttachTo.EvaluatePositionAtUnit(targetEndPoint, CinemachinePathBase.PositionUnits.PathUnits);
        newPoint.position -= _pathToAttachFrom.transform.position;
        newWaypoints.Add(newPoint);

        _pathToAttachFrom.m_Waypoints = newWaypoints.ToArray();
    }

    private float FindPreviousWaypointDistance(CinemachineSmoothPath.Waypoint[] waypointsFrom, int index, bool isClosestToStart = true)
    {
        var point = _pathToAttachTo.FindClosestPoint(waypointsFrom[index].position, 0, -1, 50);
        var waypointIndex = FindClosestWaypointIndex(waypointsFrom[index].position, _pathToAttachTo);
        var waypoint = _pathToAttachTo.FindClosestPoint(_pathToAttachTo.m_Waypoints[waypointIndex].position, 0, -1, 50);
        if (isClosestToStart && waypoint < point || waypoint > point)
        {
            return waypoint;
        }
        else if (isClosestToStart && waypointIndex > 0)
        {
            waypoint = _pathToAttachTo.FindClosestPoint(_pathToAttachTo.m_Waypoints[waypointIndex - 1].position, 0, -1, 50);
            if (waypoint < point)
            {
                return waypoint;
            }
        }
        else if (!isClosestToStart && waypointIndex < _pathToAttachTo.m_Waypoints.Length - 1)
        {
            waypoint = _pathToAttachTo.FindClosestPoint(_pathToAttachTo.m_Waypoints[waypointIndex + 1].position, 0, -1, 50);
            if (waypoint > point)
            {
                return waypoint;
            }
        }
        return point;
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

}
