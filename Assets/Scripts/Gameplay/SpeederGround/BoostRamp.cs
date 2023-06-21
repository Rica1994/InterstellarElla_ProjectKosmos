using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoostRamp : MonoBehaviour
{
    [SerializeField] private bool _shouldJump = true;
    [SerializeField] private MultiplierTimerComponent _jumpBoostMultiplierComponent;
    [SerializeField] private MultiplierTimerComponent _speedBoostMultiplierComponent;
    [SerializeField] private TriggerHandler _trigger;

    private void Awake()
    {
        _trigger.OnTriggered += OnTriggered;
    }

    private void OnTriggered(TriggerHandler me, Collider other, bool hasentered)
    {
        if (hasentered)
        {
            if (other.TryGetComponent(out SpeederGround speeder))
            {
                speeder.SetJumpMultiplierComponent(_jumpBoostMultiplierComponent);
                speeder.SetSpeedMultiplierComponent(_speedBoostMultiplierComponent);
                speeder.BoostSpeed();
                speeder.BoostJump();
                if (_shouldJump) speeder.ForceJump();
            }
        }
    }


  // private void OnTriggerExit(Collider other)
  // {
  //      && other.TryGetComponent(out SpeederGround speeder))
  //     {
  //         speeder.ForceJump();
  //     }
  // }
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