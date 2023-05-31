using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTriggerPosition : MonoBehaviour
{
	enum WaypointPosition
	{
		First,
		Last,
		Custom,
	}

	[SerializeField] private WaypointPosition _waypointPosition;

    private void Start()
    {
		// Get smooth path
		var smoothPath = GetComponentInParent<CinemachineSmoothPath>();

        if (smoothPath.m_Waypoints.Length <= 0)
        {
			return;
        }

		// Get index according to waypoint enum
        int index = -1;

		switch (_waypointPosition)
		{
			case WaypointPosition.First:
                index = 0;
				break;
			case WaypointPosition.Last:
				index = smoothPath.m_Waypoints.Length - 1;
				break;
			default:
                break;
		}

		// Set position if index is filled in
		if (index != -1)
		{
			transform.position = smoothPath.m_Waypoints[index].position + transform.position;
		}
    }
}
