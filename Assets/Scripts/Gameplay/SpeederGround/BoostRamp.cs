using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostRamp : MonoBehaviour
{
    [SerializeField] private bool _shouldJump = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && other.TryGetComponent(out SpeederGround speeder))
        {
            speeder.Boost();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_shouldJump && other.tag.Equals("Player") && other.TryGetComponent(out SpeederGround speeder))
        {
            speeder.ForceJump();
        }
    }
}
