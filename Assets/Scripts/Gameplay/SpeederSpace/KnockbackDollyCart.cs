using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Assertions;

public class KnockbackDollyCart : MonoBehaviour
{
    [SerializeField] private GameObject _knockbackPathPrefab;

    private SpeederSpace _playerSpeeder;
    private CinemachineDollyCart _dollyCart;

    private List<GameObject> _spawnedKnockbackPaths = new List<GameObject>();   
    private CinemachineSmoothPath _knockbackPath;
    private CinemachinePathBase _originalPath = null;

    private CinemachineVirtualCamera _knockbackCamera;
    private Collider _collider;
    private SwitchPath _switchPath;

    private GameObject _knockbackParent;

    private float _lastDistance = -1;
    private bool _isGettingKnockedback = false;

    private void Start()
    {
        _playerSpeeder = GetComponentInChildren<SpeederSpace>();
        Assert.IsNotNull(_playerSpeeder, $"[{GetType()}] - SpeederSpace is null");
        //_playerSpeeder.OnCollision += OnCollision;
        //_playerSpeeder.OnKnockbackEnded += OnKnockbackEnded;

        _dollyCart = GetComponentInChildren<CinemachineDollyCart>();
        Assert.IsNotNull(_dollyCart, $"[{GetType()}] - DollyCart is null");

        _knockbackParent = new GameObject("KnockbackParent");
    }

    private void OnCollision(Vector3 position)
    {
        InitializePath();

        // Check if last waypoint distance has been filled in, only overwrite in once previous last waypoint has been passed
        if (_lastDistance == -1)
        {
            // Take a new position a few units in front of current position on track
            _lastDistance = _dollyCart.m_Path.ToNativePathUnits(_dollyCart.m_Position + 5f, CinemachinePathBase.PositionUnits.Distance);
            _originalPath = _dollyCart.m_Path;
        }

        // Create waypoint list for new knockback track
        Vector3 lastPoint = _originalPath.EvaluatePosition(_lastDistance);
        var waypointList = new List<Vector3> { position, transform.position, lastPoint };
        _knockbackPath.m_Waypoints = CreatePath.CreateNewWaypoints(waypointList);

        // Set knockback camera
        _knockbackCamera.gameObject.SetActive(true);
        _knockbackCamera.MoveToTopOfPrioritySubqueue();
        _knockbackCamera.Follow = _dollyCart.transform;
        _knockbackCamera.LookAt = _dollyCart.transform;

        // Set dolly cart path
        _dollyCart.m_Path = _knockbackPath;

        // Position exit collider correctly
        _collider.gameObject.transform.position = _knockbackPath.m_Waypoints[_knockbackPath.m_Waypoints.Length - 1].position;

        // Set switch path destination
        _switchPath.SetPathDestination(_originalPath);

        _isGettingKnockedback = true;
    }

    private void InitializePath()
    {
        // Instantiate knockback path
        GameObject knockbackObject = Instantiate(_knockbackPathPrefab, _knockbackParent.transform);
        _knockbackPath = knockbackObject.GetComponentInChildren<CinemachineSmoothPath>();
        Assert.IsNotNull(knockbackObject, $"[{GetType()}] - Knockback object is null");
        _spawnedKnockbackPaths.Add(knockbackObject);

        // Get camera from track
        _knockbackCamera = _knockbackPath.gameObject.GetComponentInChildren<CinemachineVirtualCamera>(true);
        Assert.IsNotNull(_knockbackCamera, $"[{GetType()}] - VirtualCamera is null");

        // Get swith track trigger
        _collider = knockbackObject.GetComponentInChildren<Collider>();
        Assert.IsNotNull(_collider, $"[{GetType()}] - Collider is null");

        // Get switch path logic
        _switchPath = knockbackObject.GetComponentInChildren<SwitchPath>();
        Assert.IsNotNull(_switchPath, $"[{GetType()}] - SwitchPath is null");
    }

    private void OnKnockbackEnded(Vector3 position)
    {
        _isGettingKnockedback = false;
    }

    private void LateUpdate()
    {
        // Follow player allong knockback track
        if (_isGettingKnockedback && _knockbackPath)
        {
            var pos = _knockbackPath.FindClosestPoint(_playerSpeeder.transform.position, 0, -1, 100);
            _dollyCart.m_Position = _knockbackPath.FromPathNativeUnits(pos, CinemachinePathBase.PositionUnits.Distance);
        }

        // Check if cart is back on original track 
        if (_lastDistance == -1 || _dollyCart.m_Path != _originalPath)
        {
            return;
        }

        // Reached last position on original path
        if (_dollyCart.m_Position > _lastDistance)
        {
            _lastDistance = -1;

            // Cleanup all knockback paths
            foreach (var path in _spawnedKnockbackPaths)
            {
                Destroy(path.gameObject);
            }
            _spawnedKnockbackPaths.Clear();
        }
    }
}
