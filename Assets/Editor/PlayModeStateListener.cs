#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor;

[InitializeOnLoad]
public class PlayModeStateListener
{
    static PlayModeStateListener()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChange;
    }

    private static void OnPlayModeStateChange(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            var spawner = Object.FindObjectOfType<TargetSpawner>();
            spawner?.SaveObjectPositionsToFile();
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            var spawner = Object.FindObjectOfType<TargetSpawner>();
            if (spawner != null)
            {
                var savePath = spawner.SavePath;
                if (File.Exists(savePath))
                {
                    var jsonData = File.ReadAllText(savePath);
                    var data = JsonUtility.FromJson<TargetSpawner.SerializationWrapper<Vector3>>(jsonData);
                    foreach (var position in data.List)
                    {
                        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(spawner.objectToSpawn);
                        if (instance != null)
                        {
                            instance.transform.position = position;
                        }
                    }
                }
            }
        }
    }
}
#endif
