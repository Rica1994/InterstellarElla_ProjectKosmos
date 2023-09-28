using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToActiveCamera : MonoBehaviour
{
    [SerializeField]
    private Transform _target; // The GameObject to look at (MainCamera)

    public float RotationSpeed = 10f; 

    private Vector3 _lookDirection;

    private void Start()
    {
        if (_target == null)
        {
            Debug.LogWarning("Disabling rotate to cam as its missing references !");
            this.enabled = false;
        }
    }

    void Update()
    {
        _lookDirection = (_target.transform.position - this.transform.position).normalized;
        
        // SmoothDamp to the target position
        var step = 360 * Time.deltaTime * RotationSpeed;
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(-_lookDirection), step);    
    }
}
