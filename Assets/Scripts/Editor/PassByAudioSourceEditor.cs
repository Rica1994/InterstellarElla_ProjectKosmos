using UnityEngine;
using UnityEditor;

public class AudioSourceRecalculator : EditorWindow
{
    [MenuItem("Tools/AudioSource Recalculator")]
    public static void ShowWindow()
    {
        GetWindow<AudioSourceRecalculator>("AudioSource Recalculator");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Recalculate All AudioSource Distances"))
        {
            RecalculateAllAudioSourceDistances();
        }
    }

    private void RecalculateAllAudioSourceDistances()
    {
        // Find all objects of type PassByAudioSource in the scene
        PassByAudioSource[] audioSources = FindObjectsOfType<PassByAudioSource>();

        // Iterate through them and call the RecalculateAudioSourceDistances method
        foreach (PassByAudioSource source in audioSources)
        {
            source.RecalculateAudioSourceDistances();
        }

        // Notify the user
        Debug.Log($"{audioSources.Length} AudioSource distances recalculated!");
    }
}
