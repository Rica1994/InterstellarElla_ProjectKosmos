using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToDeleteCameraScript : MonoBehaviour
{
    [SerializeField] private GameObject _target; // The GameObject to follow
    [SerializeField] private float _distance = 10f; // The distance to keep from the target

    private void LateUpdate()
    {
        // Ensure the target GameObject has been set
        if (_target == null) return;

        // Move the camera to be at the desired distance behind the target
        Vector3 targetPos = _target.transform.position;
        Vector3 cameraPos = targetPos + (_target.transform.up * _distance) - (_target.transform.forward * _distance);
        transform.position = cameraPos;

        // Look at the target GameObject
        transform.LookAt(targetPos);
    }
}
