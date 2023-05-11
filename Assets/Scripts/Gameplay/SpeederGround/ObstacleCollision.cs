using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && other.TryGetComponent(out SpeederGround speeder))
        {
            speeder.Collide();
        }
    }
}
