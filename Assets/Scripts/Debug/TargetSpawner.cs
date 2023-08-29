using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
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
        var spawnedObject = Instantiate(objectToSpawn, transform.position, transform.rotation);
        positions.Add(spawnedObject.transform.position);
    }

    public void SaveObjectPositionsToFile()
    {
        File.WriteAllText(SavePath, JsonUtility.ToJson(new SerializationWrapper<Vector3> { List = positions }));
    }

    [System.Serializable]
    public class SerializationWrapper<T>
    {
        public List<T> List;
    }
}
