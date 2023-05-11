using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player") && collision.gameObject.TryGetComponent(out SpeederGround speeder))
        {
            speeder.Collide();
        }
    }
}
