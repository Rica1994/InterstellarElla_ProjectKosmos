using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSphereRotation : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;


    private void Update()
    {
        this.transform.Rotate(0, _speed * Time.deltaTime, 0);
    }

}
