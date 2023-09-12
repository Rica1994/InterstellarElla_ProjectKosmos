#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // Drag the prefab from the Assets folder, not from the scene hierarchy.
    [SerializeField]
    private float spawnInterval = 2.0f;
    private float timeSinceLastSpawn = 0.0f;
    private List<Vector3> positions;

    public string SavePath => Application.persistentDataPath + "/SpawnedObjects.json";

    private void Start()
    {
        positions = new List<Vector3>();
    }

    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnObject();
            timeSinceLastSpawn = 0.0f;
        }
    }

    private void SpawnObject()
    {
#if UNITY_EDITOR
        GameObject instance = UnityEditor.PrefabUtility.InstantiatePrefab(objectToSpawn) as GameObject;
        instance.transform.position = transform.position;
        instance.transform.rotation = transform.rotation;
        positions.Add(instance.transform.position);
#else
        var spawnedObject = Instantiate(objectToSpawn, transform.position, transform.rotation);
        positions.Add(spawnedObject.transform.position);
#endif
    }

    public void SaveObjectPositionsToFile()
    {
        File.WriteAllText(SavePath, JsonUtility.ToJson(new SerializationWrapper<Vector3> { List = positions }));
    }

    public void EditorLoadAndSpawnObjectsFromFile()
    {
        string json = File.ReadAllText(SavePath);
        SerializationWrapper<Vector3> data = JsonUtility.FromJson<SerializationWrapper<Vector3>>(json);

        foreach (var pos in data.List)
        {
            GameObject instance = UnityEditor.PrefabUtility.InstantiatePrefab(objectToSpawn) as GameObject;
            instance.transform.position = pos;
        }
    }

    [System.Serializable]
    public class SerializationWrapper<T>
    {
        public List<T> List;
    }
}
#endif