using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpeedTriggerSpeederGround : MonoBehaviour
{
    [SerializeField]
    private float _speedForward = 26.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SpeederGround>())
        {
            other.GetComponent<SpeederGround>().speedForward = _speedForward;
        }
    }
}
