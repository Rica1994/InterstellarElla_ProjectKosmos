using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetPickupSpace : MonoBehaviour
{
    [SerializeField]
    private float _timeToAttract = 1.0f;

    private float _timeAttracting = 0.0f;

    public Transform Target;


    private void Update()
    {
        _timeAttracting += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, Target.position, _timeAttracting / _timeToAttract);

        if (_timeAttracting > _timeToAttract)
        {
            transform.position = Target.position;
        }      
    }
}
