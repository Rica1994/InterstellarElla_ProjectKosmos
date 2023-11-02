using UnityEditor;
using UnityEngine;

public class PrefabScaterer : EditorWindow
{
    private GameObject _prefabToSpawn;

    [MenuItem("Tools/Prefab Scaterer")]
    private static void ShowWindow()
    {
        var window = GetWindow<PrefabScaterer>();
        window.titleContent = new GUIContent("Scene Click Spawner");
        window.Show();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        _prefabToSpawn = (GameObject)EditorGUILayout.ObjectField("Prefab To Spawn", _prefabToSpawn, typeof(GameObject), false);

        // Other GUI controls can go here.
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            // This ensures that we're not interacting with other GUI elements or controls.
            if (sceneView.in2DMode || Event.current.alt || Event.current.control || Event.current.shift)
            {
                return;
            }

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.point);
                Event.current.Use(); // This consumes the mouse event.
                Transform hitTransform = null;
                hitTransform.position = hit.point;
                GameObject spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(_prefabToSpawn, hitTransform);
                Undo.RegisterCreatedObjectUndo(spawnedObject, "Spawn Prefab");

                // Set the position of the spawned prefab to the hit point.
                spawnedObject.transform.position = hit.point;
                spawnedObject.transform.up = hit.normal; // Optionally align with surface normal.

                // This will force the scene view to redraw, updating the position of the prefab.
                SceneView.RepaintAll();
            }
        }
    }
}
