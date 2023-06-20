using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoostRamp : MonoBehaviour
{
    [SerializeField] private bool _shouldJump = true;
    [SerializeField] private MultiplierTimerComponent _boostMultiplierComponent;
    private void OnTriggerEnter(Collider other)
    {
        // TODO: use something other than player tag
        if (other.TryGetComponent(out SpeederGround speeder))
        {
            speeder.BoostSpeed();
            speeder.BoostJump();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_shouldJump && other.TryGetComponent(out SpeederGround speeder))
        {
            speeder.ForceJump();
        }
    }
}

/*public class BoostRampEditor : Editor
{
    private void OnSceneGUI()
    {
        BoostRamp be = target as BoostRamp;
        var startPoint = be.transform.position;
        var endPoint = be.transform.position + be.transform.forward * SpeederGround.SpeedForward;
        Handles.DrawBezier(be.startPoint, be.endPoint, be.startTangent, be.endTangent, Color.red, null, 2f);
    }
}*/