using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipControls : MonoBehaviour
{
    public bool InvertPlayerControls;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SpeederSpace player))
        {
            player.InvertControls = InvertPlayerControls;
        }
    }
}
