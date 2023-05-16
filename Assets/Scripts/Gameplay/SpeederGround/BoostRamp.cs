using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostRamp : MonoBehaviour
{
    [SerializeField] private bool _shouldJump = true;

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
