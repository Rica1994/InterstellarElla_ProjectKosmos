using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    // I need to constantly set 'an object rotation' to an adjusted one of the followObject camera 
    [SerializeField]
    public GameObject ObjectToFollowPlayer;

    [SerializeField]
    private Transform _target;

    private Vector3 _offset;


    void Start()
    {
        if (ObjectToFollowPlayer == null)
        {
            ObjectToFollowPlayer = this.gameObject;
        }      
    }

    void Update()
    {
        //ObjectToFollowPlayer.transform.position = _target.position + _offset;
        ObjectToFollowPlayer.transform.position = _target.position;
    }
    public void ChangeObjectToFollow(GameObject objToFollowPlayer)
    {
        ObjectToFollowPlayer = objToFollowPlayer;
    }
}
