using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostRing : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SpeederSpace speeder))
        {
            speeder.Boost();
        }
    }

}
