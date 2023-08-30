using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachinePath))]
public class ConvertSmoothPathToPath : MonoBehaviour
{
    [SerializeField]
    private CinemachineSmoothPath _smoothPath;
    [Tooltip("Default tangent for each waypoint.")]
    [SerializeField]
    private Vector3 _defaultTangent = Vector3.forward;

    private void Start()
    {
        if (_smoothPath == null) return;

        CinemachinePath cinemachinePath = GetComponent<CinemachinePath>();

        // Copy waypoints
        cinemachinePath.m_Waypoints = new CinemachinePath.Waypoint[_smoothPath.m_Waypoints.Length];

        for (int i = 0; i < _smoothPath.m_Waypoints.Length; i++)
        {
            cinemachinePath.m_Waypoints[i] = new CinemachinePath.Waypoint
            {
                position = _smoothPath.m_Waypoints[i].position,
                tangent = _defaultTangent,
                roll = _smoothPath.m_Waypoints[i].roll
            };
        }
    }
}
