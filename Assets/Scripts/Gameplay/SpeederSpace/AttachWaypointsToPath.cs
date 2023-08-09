using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachWaypointsToPath : MonoBehaviour
{
    [SerializeField] private CinemachineSmoothPath _pathToAttachTo;
    private CinemachineSmoothPath _pathToAttachFrom;

    [Header("Trigger object which the player needs to enter")]
    public GameObject StartTrigger;

    private void Awake()
    {
        if(!TryGetComponent(out _pathToAttachFrom))
        {
            return;
        }

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

        var newWaypoints = new List<CinemachineSmoothPath.Waypoint>();
        newWaypoints.AddRange(waypointsFrom);

        var newWaypoint = new CinemachineSmoothPath.Waypoint();

        // Get position from targetstart point
        newWaypoint.position = _pathToAttachTo.EvaluatePositionAtUnit(targetStartDistance, CinemachinePathBase.PositionUnits.PathUnits);
        // Transform position to local space
        newWaypoint.position -= _pathToAttachFrom.transform.position;

        // Replace first waypoint with new start waypoint
        newWaypoints[0] = newWaypoint;

        // Get position from target end 
        newWaypoint.position = _pathToAttachTo.EvaluatePositionAtUnit(targetEndDistance, CinemachinePathBase.PositionUnits.PathUnits);
        // Transform position to local space
        newWaypoint.position -= _pathToAttachFrom.transform.position;

        // Replace last waypoint with new end wayopint
        newWaypoints[newWaypoints.Count - 1] = newWaypoint;

        // Convert list to array and save as waypoints
        _pathToAttachFrom.m_Waypoints = newWaypoints.ToArray();
    }
}
