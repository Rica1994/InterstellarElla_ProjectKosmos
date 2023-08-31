#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public class PlayModeStateListener
{
    static PlayModeStateListener()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChange;
    }

    private static void OnPlayModeStateChange(PlayModeStateChange state)
    {
        TargetSpawner[] spawners = UnityEngine.Object.FindObjectsOfType<TargetSpawner>();
        if (spawners.Length <= 0) return; // Exit if no spawners

        TargetSpawner spawner = spawners[0];  // Assuming you only have one spawner

        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            spawner.SaveObjectPositionsToFile();
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            if (File.Exists(spawner.SavePath)) // Using instance to access SavePath
            {
                if (EditorUtility.DisplayDialog("Spawn objects?", "Would you like to spawn the saved objects?", "Yes", "No"))
                {
                    spawner.EditorLoadAndSpawnObjectsFromFile();
                }
            }
        }
    }
}
#endif
