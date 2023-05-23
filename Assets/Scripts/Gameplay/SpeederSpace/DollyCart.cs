using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Assertions;

public class DollyCart : MonoBehaviour
{
    [SerializeField] private GameObject _knockbackPathPrefab;
    private SpeederSpace _playerSpeeder;
    private CinemachineDollyCart _dollyCart;
    private CinemachineSmoothPath _knockbackPath;
    private CinemachinePathBase _originalPath = null;
    private CinemachineVirtualCamera _knockbackCamera;
    private Collider _collider;
    private SwitchPath _switchPath;
    private float _lastDistance = -1;

    private void Start()
    {
        _playerSpeeder = GetComponentInChildren<SpeederSpace>();
        _playerSpeeder.OnCollision += OnCollision;
        _playerSpeeder.OnKnockbackEnded += OnKnockbackEnded;

        _dollyCart = GetComponentInChildren<CinemachineDollyCart>();
    }

    private void OnCollision(Vector3 position)
    {
        // Instantiate knockback path
        GameObject knockbackObject = Instantiate(_knockbackPathPrefab);
        _knockbackPath = knockbackObject.GetComponentInChildren<CinemachineSmoothPath>();
        Assert.IsNotNull(knockbackObject, "[SpeederSpace] - Knockback object is null");

        // Get camera from track
        _knockbackCamera = _knockbackPath.gameObject.GetComponentInChildren<CinemachineVirtualCamera>(true);
        Assert.IsNotNull(_knockbackCamera, "[SpeederSpace] - VirtualCamera is null");

        // Get swith track trigger
        _collider = knockbackObject.GetComponentInChildren<Collider>();
        Assert.IsNotNull(_collider, "[SpeederSpace] - Collider is null");

        // Get switch path logic
        _switchPath = knockbackObject.GetComponentInChildren<SwitchPath>();
        Assert.IsNotNull(_switchPath, "[SpeederSpace] - SwitchPath is null");

        if (_lastDistance == -1)
        {
            float currentDistance = _dollyCart.m_Path.ToNativePathUnits(_dollyCart.m_Position, CinemachinePathBase.PositionUnits.Distance);
            _lastDistance = currentDistance + .1f;
            _originalPath = _dollyCart.m_Path;
        }

        Vector3 lastPoint = _originalPath.EvaluatePosition(_lastDistance);
        var waypointList = new List<Vector3> { position, transform.position, lastPoint };
        _knockbackPath.m_Waypoints = CreatePath.CreateNewWaypoints(waypointList);

        _knockbackCamera.gameObject.SetActive(true);
        _knockbackCamera.MoveToTopOfPrioritySubqueue();
        _knockbackCamera.Follow = _playerSpeeder.transform;
        _knockbackCamera.LookAt = _dollyCart.gameObject.transform;

        _collider.gameObject.transform.position = _knockbackPath.m_Waypoints[_knockbackPath.m_Waypoints.Length - 1].position;

        _switchPath.SetPathDestination(_originalPath);
    }

    private void OnKnockbackEnded(Vector3 position)
    {
        _knockbackPath.m_Waypoints[0].position = _playerSpeeder.transform.position;

        _knockbackCamera.gameObject.SetActive(true);
        _knockbackCamera.MoveToTopOfPrioritySubqueue();
        _knockbackCamera.Follow = _dollyCart.gameObject.transform;
        _knockbackCamera.LookAt = _dollyCart.gameObject.transform;

        _dollyCart.m_Position = 0f;
        _dollyCart.m_Path = _knockbackPath;
    }

    private void LateUpdate()
    {
        // Is back on path 
        if (_lastDistance != -1 && _dollyCart.m_Path == _originalPath)
        {
            // Reached last position on original path
            if (_dollyCart.m_Position > _lastDistance)
            {
                _lastDistance = -1;
            }
        }
    }
}
