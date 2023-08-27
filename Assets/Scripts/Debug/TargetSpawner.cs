using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField]
    private float _spawnInterval = 2.0f; // Time interval between spawns in seconds
    private float _timeSinceLastSpawn = 0.0f;

    private GameObject _targetObject;

    private void Awake()
    {
        _targetObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _targetObject.name = "TargetObject";

        // Remove the collider component from the sphere
        Destroy(_targetObject.GetComponent<Collider>());
        _targetObject.transform.position = transform.position;
        _targetObject.transform.rotation = transform.rotation;
    }

    private void Update()
    {
        _timeSinceLastSpawn += Time.deltaTime;

        if (_timeSinceLastSpawn >= _spawnInterval)
        {
            SpawnObject();
            _timeSinceLastSpawn = 0.0f;
        }
    }

    private void SpawnObject()
    {
        Instantiate(_targetObject, transform.position, transform.rotation);
    }
}