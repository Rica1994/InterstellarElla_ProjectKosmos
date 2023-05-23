using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePath : MonoBehaviour
{
    public static CinemachineSmoothPath CreateNewPath(Vector3 startPosition, CinemachineSmoothPath targetPath, float endDistanceOnPath)
    {
        Vector3 endPosition = targetPath.EvaluatePosition(endDistanceOnPath);
        return CreateNewPath(startPosition, endPosition);
    }

    public static CinemachineSmoothPath CreateNewPath(Vector3 startPosition, Vector3 endPosition)
    {
        CinemachineSmoothPath.Waypoint wayPoint = new CinemachineSmoothPath.Waypoint();
        wayPoint.position = startPosition;

        List<CinemachineSmoothPath.Waypoint> wayPoints = new List<CinemachineSmoothPath.Waypoint> { };
        wayPoints.Add(wayPoint);

        wayPoint.position = endPosition;
        wayPoints.Add(wayPoint);

        // Create gameobject
        GameObject knockbackObject = new GameObject("NewPath");
        var knockbackPath = knockbackObject.AddComponent<CinemachineSmoothPath>();
        knockbackPath.m_Waypoints = wayPoints.ToArray();

        return knockbackPath;
    }

    public static CinemachineSmoothPath.Waypoint[] CreateNewWaypoints(List<Vector3> positions)
    {
        List<CinemachineSmoothPath.Waypoint> wayPoints = new List<CinemachineSmoothPath.Waypoint> { };
        CinemachineSmoothPath.Waypoint wayPoint = new CinemachineSmoothPath.Waypoint();

        foreach (var positoin in positions)
        {
            wayPoint.position = positoin;
            wayPoints.Add(wayPoint);
        }

        return wayPoints.ToArray();
    }
}
