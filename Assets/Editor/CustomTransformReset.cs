using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class CustomTransformReset
{
    [MenuItem("Edit/Reset to Collision Point or Scene Camera Near Clip %e")]
    private static void ResetSelectedToCollisionPoint()
    {
        if (Selection.activeTransform != null)
        {
            ResetToCollisionPointOrNearClip(Selection.activeTransform);
        }
    }

    private static void ResetToCollisionPointOrNearClip(Transform transform)
    {
        Camera sceneCamera = SceneView.lastActiveSceneView.camera;
        if (!sceneCamera)
        {
            Debug.LogError("No active scene view camera found.");
            return;
        }

        Vector3 screenCenterPoint = new Vector3(sceneCamera.pixelWidth / 2, sceneCamera.pixelHeight / 2, sceneCamera.nearClipPlane);
        Ray ray = sceneCamera.ScreenPointToRay(screenCenterPoint);
        RaycastHit hit;

        Undo.RecordObject(transform, "Reset transform");

        if (Physics.Raycast(ray, out hit))
        {
            transform.position = hit.point;
            Debug.Log("Object placed at raycast hit point: " + hit.point);
        }
        else
        {
            Vector3 nearClipPoint = sceneCamera.ScreenToWorldPoint(screenCenterPoint);
            transform.position = nearClipPoint;
            Debug.Log("No hit detected, object placed at scene camera near clip point: " + nearClipPoint);
        }

        Undo.RegisterCompleteObjectUndo(transform, "Reset Transform");
    }
}
