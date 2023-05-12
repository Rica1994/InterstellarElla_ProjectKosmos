using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCollisionSpace : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player") && collision.gameObject.TryGetComponent(out SpeederSpace speeder))
        {
            //speeder.Collide();
        }
    }
}
