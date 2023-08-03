using UnityEditor;
using UnityEngine;

public class FramerateControllerWindow : EditorWindow
{
    private int frameRateLimit = 60;
    private int frameRateStep = 10;

    [MenuItem("Window/Framerate Controller")]
    public static void ShowWindow()
    {
        GetWindow<FramerateControllerWindow>("Framerate Controller");
    }

    private void OnGUI()
    {
        GUILayout.Label("Framerate Settings", EditorStyles.boldLabel);
        frameRateLimit = EditorGUILayout.IntField("Frame Rate Limit", frameRateLimit);
        frameRateStep = EditorGUILayout.IntField("Frame Rate Step", frameRateStep);

        if (GUILayout.Button("Apply"))
        {
            ApplyFrameRateLimit();
        }
    }

    private void ApplyFrameRateLimit()
    {
        Application.targetFrameRate = frameRateLimit;
        Debug.Log("Frame rate limit set to: " + frameRateLimit);
    }
}
