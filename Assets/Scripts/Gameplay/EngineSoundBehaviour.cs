using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSoundBehaviour : MonoBehaviour
{
    [SerializeField]
    private AudioSource _engineAudioSource;

    private float _defaultEnginePitch;
    private float _lastVehicleSpeed = 0.0f;

    [SerializeField]
    private GameObject _vehicleObject;

    [SerializeField]
    private float _defaultVehicleSpeed = 26.666f;

    private IVehicle _vehicle;

    [SerializeField]
    private float _differenceBetweenSpeeds = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        _defaultEnginePitch = _engineAudioSource.pitch;
        _vehicle = _vehicleObject.GetComponent<IVehicle>();
        if (_vehicle == null)
            Debug.LogError("You added a non vehicle object to the engine sound script!");
    }

    // Update is called once per frame
    void Update()
    {
        if (_vehicle == null) return;
        float currentVehicleSpeed = Mathf.Clamp(_vehicle.GetSpeed(), 1.0f, _vehicle.GetSpeed());
        var dif = Mathf.Abs(_lastVehicleSpeed - _vehicle.GetSpeed());
        if (dif > _differenceBetweenSpeeds)
        {
            _lastVehicleSpeed = currentVehicleSpeed;
            _engineAudioSource.pitch = Mathf.Clamp(_defaultEnginePitch * (currentVehicleSpeed / _defaultVehicleSpeed), 0.6f, 2);
        }
    }
}
