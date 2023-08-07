using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControllerAdjuster : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField]
    private Animator _myAnimator;

    [Header("Variables")]
    [SerializeField]
    [Range(0.05f, 3f)]
    private float _rotationSpeed = 1f;

    [SerializeField]
    private bool _flipTheRotation = false;


    //[SerializeField]
    //[Range(1, 3)]
    //private int _amountOfAxisRotations;

    // USING multiple layers for rotations is just unecessary and would tank CPU //

    //[Header("Controllers from Assets")]
    //[SerializeField]
    //private RuntimeAnimatorController _controller1Axis;    
    //[SerializeField]
    //private RuntimeAnimatorController _controller2Axis;
    //[SerializeField]
    //private RuntimeAnimatorController _controller3Axis;



    private void Start()
    {
        // set animator controller
        //if (_amountOfAxisRotations == 3)
        //{
        //    _myAnimator.runtimeAnimatorController = _controller3Axis;
        //}
        //else if (_amountOfAxisRotations == 2)
        //{
        //    _myAnimator.runtimeAnimatorController = _controller2Axis;
        //}
        //else
        //{
        //    _myAnimator.runtimeAnimatorController = _controller1Axis;
        //}

        // set speed
        _myAnimator.speed = _rotationSpeed;


        // reverse if required
        if (_flipTheRotation == true)
        {
            _myAnimator.SetFloat("Speed", -1);                    
        }
    }
}
