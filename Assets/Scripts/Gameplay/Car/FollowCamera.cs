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


    private bool _isTransposer;
    private GameObject _transposerLookAtTarget;
    private Vector3 _lookDirectionVector;


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

        // if im a transposer, also rotate it to look at the target
        if (_isTransposer == true)
        {
            _lookDirectionVector = (_transposerLookAtTarget.transform.position - ObjectToFollowPlayer.transform.position).normalized;
            //ObjectToFollowPlayer.transform.rotation = Quaternion.LookRotation(_transposerLookAtTarget.transform.position);
            ObjectToFollowPlayer.transform.rotation = Quaternion.LookRotation(_lookDirectionVector, Vector3.up);
        }
    }
    public void ChangeObjectToFollow(GameObject objToFollowPlayer)
    {
        _isTransposer = false;
        _transposerLookAtTarget = null;

        ObjectToFollowPlayer = objToFollowPlayer;    
    }
    public void ChangeObjectToFollowTransposer(GameObject objToFollowPlayer, GameObject transposerTarget)
    {
        _isTransposer = true;
        _transposerLookAtTarget = transposerTarget;

        ObjectToFollowPlayer = objToFollowPlayer;
    }
}
