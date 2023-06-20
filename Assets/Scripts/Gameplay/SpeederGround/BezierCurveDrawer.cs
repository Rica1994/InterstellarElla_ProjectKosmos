#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class BezierCurveDrawer : MonoBehaviour
{
    /*public Transform startPoint;
    public float speed = 5f;
    public float gravity = 9.8f;
    public float startSlope = 0f;

    private void OnDrawGizmos()
    {
        if (startPoint == null)
            return;

        Handles.color = Color.white;
        Handles.DrawBezier(startPoint.position, GetCurveEndPoint(), startPoint.position + Vector3.forward, GetCurveEndPoint() + Vector3.forward, Color.white, null, 2f);
    }

    
    public Vector3 GetCurveEndPoint()
    {
        float time = CalculateCurveTime();
        Vector3 endPoint = startPoint.position + (Vector3.right * speed * time) + (Vector3.down * gravity * time * time / 2f) + (Vector3.up * startSlope * time);
        return endPoint;
    }

    private float CalculateCurveTime()
    {
        float maxTime = 10f; // Set a maximum time value to avoid an infinite loop
        float timeStep = 0.1f;
        float time = 0f;

        while (time <= maxTime)
        {
            Vector3 currentPosition = startPoint.position + (Vector3.right * speed * time) + (Vector3.down * gravity * time * time / 2f) + (Vector3.up * startSlope * time);

            if (currentPosition.y <= 0f)
                break;

            time += timeStep;
        }

        return time;
    }*/

    public float h = 0f; // Initial height
    public float alpha = 45f; // Launch angle in degrees
    public float g = 9.8f; // Acceleration due to gravity
    public float V0 = 10f; // Initial velocity

    public int numPoints = 50; // Number of trajectory points
    public float maxDistance = 50f; // Maximum distance of the trajectory

    void OnDrawGizmos()
    {
        float angleRad = alpha * Mathf.Deg2Rad;
        float cosAngleSq = Mathf.Cos(angleRad) * Mathf.Cos(angleRad);

        for (int i = 0; i < numPoints; i++)
        {
            float x = i * maxDistance / numPoints;
            float y = h + x * Mathf.Tan(angleRad) - (g * x * x) / (2f * V0 * V0 * cosAngleSq);
            Vector3 point = new Vector3(0, y, x);
            Gizmos.DrawSphere(transform.position + point, 0.5f);
        }
    }

}

#endif
