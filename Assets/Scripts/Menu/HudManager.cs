using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    [SerializeField] private GameObject _joystick;
    [SerializeField] private GameObject _action;
    [SerializeField] private bool _simulateMobile = false; 

    private void Start()
    {
        var gameManager = ServiceLocator.Instance.GetService<GameManager>();
        //if (!gameManager.IsMobileWebGl)
        //{
        //    _joystick.SetActive(false);
        //    _action.SetActive(false);
        //}
    }
}
