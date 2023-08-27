using UnityEditor;
using UnityEngine;

public class PrefabSpawnWindow : EditorWindow
{
    private GameObject prefabToSpawn;

    [MenuItem("Window/Prefab Spawn Tool")]
    public static void ShowWindow()
    {
        GetWindow<PrefabSpawnWindow>("Prefab Spawn Tool");
    }

    private void OnGUI()
    {
        prefabToSpawn = (GameObject)EditorGUILayout.ObjectField("Prefab to Spawn", prefabToSpawn, typeof(GameObject), false);

        if (GUILayout.Button("Spawn Prefabs"))
        {
            SpawnPrefabs();
        }
    }

    private void SpawnPrefabs()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError("Prefab to spawn is not assigned!");
            return;
        }

        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            GameObject spawnedObject = PrefabUtility.InstantiatePrefab(prefabToSpawn) as GameObject;
            if (spawnedObject != null)
            {
                spawnedObject.transform.position = selectedObject.transform.position;
                spawnedObject.transform.rotation = selectedObject.transform.rotation;
            }
        }
    }
}
