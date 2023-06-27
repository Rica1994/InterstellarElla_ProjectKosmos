using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityTrigger : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rigidbody;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _rigidbody.isKinematic = false;
        }
    }
}
