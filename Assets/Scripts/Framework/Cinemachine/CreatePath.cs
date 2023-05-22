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
        GameObject knockbackObject = new GameObject("KnockbackPath");
        var knockbackPath = knockbackObject.AddComponent<CinemachineSmoothPath>();
        knockbackPath.m_Waypoints = wayPoints.ToArray();

        return knockbackPath;
    }
}
