using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerHandler))]
public class MagnetComponent : MonoBehaviour
{
    [SerializeField]
    private Transform _transformAttracting;

    [SerializeField]
    private float _timeToAttract = 1.0f;

    private TriggerHandler _perimeterTriggerHandler;

    private float _timeAttracting = 0.0f;
    private bool _magnetismEnabled = false;
    private Transform _target;

    private void Awake()
    {
        if (_transformAttracting == null)
        {
            _transformAttracting = transform;
        }
        _perimeterTriggerHandler = GetComponent<TriggerHandler>();
        _perimeterTriggerHandler.OnTriggered += OnTriggered;
    }

    private void Start()
    {
    //   if (FindObjectOfType<PlayerController>() == null) 
    //   {
    //       this.enabled = false;
    //   }
    }

    private void OnDestroy()
    {
        _perimeterTriggerHandler.OnTriggered -= OnTriggered;
    }

    private void OnTriggered(TriggerHandler me, Collider other, bool hasEntered)
    {
        if (hasEntered && other.GetComponent<PlayerController>())
        {
            _target = other.transform;
            StartMagnetism();
        }
    }

    private void FixedUpdate()
    {
        if (_magnetismEnabled)
        {
            _timeAttracting += Time.deltaTime;
            _transformAttracting.position = Vector3.Lerp(_transformAttracting.position, _target.position, _timeAttracting / _timeToAttract);

            if (_timeAttracting >  _timeToAttract)
            {
                _transformAttracting.position = _target.position;
            }
        }
    }

    private void StartMagnetism()
    {
        _magnetismEnabled = true;
    }
}
