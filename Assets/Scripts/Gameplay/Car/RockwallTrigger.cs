using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockwallTrigger : MonoBehaviour
{
    [SerializeField]
    private RockWallNew _rockWallNew;


    private void OnTriggerEnter(Collider other)
    {
        // Check if it is the car that collides with us
        SimpleCarController car = other.GetComponent<SimpleCarController>();
        if (_rockWallNew.IsActivated == true || car == null) return;

        if (_rockWallNew.RequiresSpeedBoost == true && car.IsBoosting == false) return;

        _rockWallNew.GlitchSmashRockWall();
    }
}
