using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class BezierCurveDrawer : MonoBehaviour
{
    public Transform startPoint;
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
    }
}