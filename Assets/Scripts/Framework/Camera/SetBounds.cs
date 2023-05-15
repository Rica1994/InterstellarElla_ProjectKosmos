using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBounds : MonoBehaviour
{
    [SerializeField] private Vector2 _viewPort;

    private void Start()
    {
        var camera = Camera.main;
        var virtualCamera = GetComponentInParent<CinemachineTrackedDolly>();
        if (virtualCamera)
        {
            var offset = virtualCamera.m_PathOffset;

            var position = camera.ViewportToWorldPoint(new Vector3(_viewPort.x, _viewPort.y, Vector3.Distance(gameObject.transform.position, gameObject.transform.position - offset)));
            gameObject.transform.position = position;
        }
    }
}
