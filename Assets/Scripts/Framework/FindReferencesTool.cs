#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;

public class FindComponentReferences : EditorWindow
{
    private MonoScript scriptToFind = null;

    [MenuItem("Tools/Find Component References")]
    public static void ShowWindow()
    {
        GetWindow<FindComponentReferences>("Find Component");
    }

    void OnGUI()
    {
        scriptToFind = EditorGUILayout.ObjectField("Component Script", scriptToFind, typeof(MonoScript), false) as MonoScript;

        if (GUILayout.Button("Find References"))
        {
            FindReferencesInAllScenes();
        }
    }

    private void FindReferencesInAllScenes()
    {
        string originalScenePath = EditorSceneManager.GetActiveScene().path;

        if (scriptToFind == null)
        {
            Debug.LogError("No script selected.");
            return;
        }

        Type componentType = scriptToFind.GetClass();
        if (componentType == null || !componentType.IsSubclassOf(typeof(Component)))
        {
            Debug.LogError("The selected script is not a Component.");
            return;
        }

        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                EditorSceneManager.OpenScene(scene.path);
                UnityEngine.Object[] foundObjects = FindObjectsOfType(componentType);
                foreach (var obj in foundObjects)
                {
                    Debug.Log("Found " + obj.name + " in " + scene.path);
                }
            }
        }

        // Return to the original scene
        if (!string.IsNullOrEmpty(originalScenePath))
        {
            EditorSceneManager.OpenScene(originalScenePath);
        }
        else
        {
            Debug.Log("Original scene was not set or has been deleted.");
        }
    }
}
#endif