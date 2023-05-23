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

        // Check if last waypoint distance has been filled in, only overwrite in once previous last waypoint has been passed
        if (_lastDistance == -1)
        {
            // Take a new position a few units in front of current position on track
            _lastDistance = _dollyCart.m_Path.ToNativePathUnits(_dollyCart.m_Position + 2f, CinemachinePathBase.PositionUnits.Distance);
            _originalPath = _dollyCart.m_Path;
        }

        // Create waypoint list for new knockback track
        Vector3 lastPoint = _originalPath.EvaluatePosition(_lastDistance);
        var waypointList = new List<Vector3> { position, transform.position, lastPoint };
        _knockbackPath.m_Waypoints = CreatePath.CreateNewWaypoints(waypointList);

        // Set knockback camera
        _knockbackCamera.gameObject.SetActive(true);
        _knockbackCamera.MoveToTopOfPrioritySubqueue();
        _knockbackCamera.Follow = _playerSpeeder.transform;
        _knockbackCamera.LookAt = _playerSpeeder.transform;

        // Position exit collider correctly
        _collider.gameObject.transform.position = _knockbackPath.m_Waypoints[_knockbackPath.m_Waypoints.Length - 1].position;

        // Set switch path destination
        _switchPath.SetPathDestination(_originalPath);

        // Position dolly cart correctly for future forwards movement
        _dollyCart.m_Position = 0f;
        _dollyCart.m_Path = _knockbackPath;
    }

    private void OnKnockbackEnded(Vector3 position)
    {
        // Adjust first waypoint to where player actually ended
        _knockbackPath.m_Waypoints[0].position = _playerSpeeder.transform.position;

        // Adjust knockback camera follow and lookat
        _knockbackCamera.Follow = _dollyCart.transform;
        _knockbackCamera.LookAt = _dollyCart.transform;
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
