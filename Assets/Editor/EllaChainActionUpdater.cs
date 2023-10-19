#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class EllaChainActionUpdater : Editor
{
    [MenuItem("Tools/Update EllaChainAction Components In Scenes")]
    public static void UpdateEllaChainActionComponentsInScenes()
    {
        // Backup the currently open scene's path
        string currentScenePath = SceneManager.GetActiveScene().path;

        // Iterate over all scenes
        foreach (EditorBuildSettingsScene editorScene in EditorBuildSettings.scenes)
        {
            if (editorScene.enabled)
            {
                Scene scene = EditorSceneManager.OpenScene(editorScene.path, OpenSceneMode.Single);

                // Find all EllaChainAction components in the scene
                EllaChainAction[] components = Object.FindObjectsOfType<EllaChainAction>();

                // For each component, call OnValidate or any other update logic
                foreach (EllaChainAction component in components)
                {
              //      component.OnValidate();
                }

                // Save the changes to the scene
                EditorSceneManager.SaveScene(scene);
            }
        }

        // Reopen the initially open scene
        if (!string.IsNullOrEmpty(currentScenePath))
        {
            EditorSceneManager.OpenScene(currentScenePath);
        }

        Debug.Log("EllaChainAction components in all scenes updated!");
    }
}
#endif
