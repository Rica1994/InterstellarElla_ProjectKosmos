using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamoPickUp : MonoBehaviour
{
    private bool _spawnedPickUp = false;
    
    [SerializeField]
    private PickUp _pickUp;

    private void Start()
    {
        _pickUp.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_spawnedPickUp) return;

        // animation that makes dynamo dig up the ground. 
        // play here
        
        if (other.CompareTag("Dynamo"))
        {
            _pickUp.gameObject.SetActive(true);
            _spawnedPickUp = true;
        }
    }
}