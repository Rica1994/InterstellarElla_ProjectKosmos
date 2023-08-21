#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class BezierCurveDrawer : MonoBehaviour
{
    /// <summary>
    /// The transform used to calculate the angle compared to the ground, which signifies the inclination.
    /// </summary>
    [SerializeField]
    private Transform _forwardVectorInclinationTransform;

    [SerializeField]
    private Transform _startingPoint;
    
    public float h = 0f; // Initial height
    public float alpha = 45f; // Launch angle in degrees
    public float g = 9.8f; // Acceleration due to gravity
    public float V0 = 10f; // Initial velocity

    public int numPoints = 50; // Number of trajectory points
    public float maxTime = 3f; // Maximum distance of the trajectory

    void OnDrawGizmos()
    {
        if (_forwardVectorInclinationTransform != null)
        {
            alpha = Vector3.Angle(Vector3.forward, _forwardVectorInclinationTransform.forward);
        } 
        float angleRad = alpha * Mathf.Deg2Rad;
        float cosAngleSq = Mathf.Cos(angleRad) * Mathf.Cos(angleRad);
        float maxDistance = maxTime * V0;

        for (int i = 0; i < numPoints; i++)
        {
            float x = i * maxDistance / numPoints;
            float y = h + x * Mathf.Tan(angleRad) - (g * x * x) / (2f * V0 * V0 * cosAngleSq);
            Vector3 point = new Vector3(0, y, x);

            Vector3 startPosition = _startingPoint == null ? transform.position : _startingPoint.position;
            Gizmos.DrawSphere(startPosition + point, 0.5f);
        }
    }
}

#endif
