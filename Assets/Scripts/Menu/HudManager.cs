using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HudManager : Service
{
    [SerializeField] private GameObject _joystick;
    [SerializeField] private TouchButton _touchButton;

    public TouchButton TouchButton => _touchButton;

    private void Start()
    {
        var gameManager = ServiceLocator.Instance.GetService<GameManager>();

        if (!gameManager.IsMobileWebGl || gameManager.GetCurrentPlanet() == GameManager.Planet.None)
        {
            _joystick.SetActive(false);
            _touchButton.gameObject.SetActive(false);
        }
    }
}
