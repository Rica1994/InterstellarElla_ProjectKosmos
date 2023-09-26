using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TouchButton : MonoBehaviour
{
    public delegate void ButtonDelegate();
    public event ButtonDelegate Pressed;
    public event ButtonDelegate Unpressed;

    // [SerializeField]
    // private GameObject _fingerIndicator;

    public bool isPressed = false;

    //  [SerializeField]
    //  private Text _buttonState;

    [SerializeField]
    private int _fingerId = 1000;
    [SerializeField]
    private int _fingerIndex = 1000;
    private Collider2D collider;

    [SerializeField]
    private float _cooldownLength = 0.5f;

    public float CooldownLength
    {
        get { return _cooldownLength; } 
        set { _cooldownLength = value; }
    }

    private float _timePassed = 0.0f;

    private void Start()
    {
        collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        _timePassed += Time.deltaTime;

        if (Input.touchCount > 0)
        {
            bool fingerIdFound = false;

            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (collider.OverlapPoint(touch.position))
                {
                    _fingerId = touch.fingerId;
                    isPressed = true;
                }
                if (touch.fingerId == _fingerId)
                {
                    _fingerIndex = i;
                    fingerIdFound = true;
                    break;
                }
            }
            if (!fingerIdFound)
            {
                _fingerId = 1000;
                isPressed = false;
            }
        }
        else
        {
            _fingerId = 1000;
            isPressed = false;
        }


        //     if (_fingerIndicator.activeSelf)
        //     {
        //         if (_fingerId != 1000) _fingerIndicator.transform.position = Input.GetTouch(_fingerIndex).position;
        //     }


        if (isPressed && _timePassed > _cooldownLength)
        {
            _timePassed = 0.0f;
    //        if (_buttonState.text != "Boosting") _buttonState.text = "Boosting";
            Pressed?.Invoke();
        }
        else
        {
            Unpressed?.Invoke();
        }
   //     else
   //     {
    //        if (_buttonState.text != "Hold To Boost") _buttonState.text = "Hold To Boost";
    //    }
    }
}