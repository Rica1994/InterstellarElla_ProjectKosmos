using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MultiplierTimerComponent))]
public class BoostRing : MonoBehaviour
{
    private MultiplierTimerComponent _multiplierComponent;

    private void Awake()
    {
        _multiplierComponent = GetComponent<MultiplierTimerComponent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SpeederSpace speeder))
        {
            speeder.Boost(_multiplierComponent);
        }
    }

}
