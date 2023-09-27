using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(100)]
public class FixInputOrientation : MonoBehaviour
{
    [SerializeField]
    private FollowCamera _followCamera;


    void Update()
    {
        this.transform.rotation = Quaternion.Euler(0, _followCamera.ObjectToFollowPlayer.transform.rotation.eulerAngles.y, 0);
    }
}
