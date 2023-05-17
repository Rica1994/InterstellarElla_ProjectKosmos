using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostRing : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // TODO: use something other than player tag
        if (other.tag.Equals("Player") && other.TryGetComponent(out SpeederSpace speeder))
        {
            speeder.Boost();
        }
    }

}
