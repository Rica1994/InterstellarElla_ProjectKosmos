using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Scene;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private Button _menuButton;

    [SerializeField]
    private Button _settingsButton;

    [SerializeField]
    private Button _continuePlayingButton;

    [SerializeField, Tooltip("Time it takes for this menu to pop up")]
    private float _popupTime = 0.5f;

    [SerializeField]
    private SettingsMenu _settingsMenu;

    public void Show(bool show, float popUpTime = -1)
    {
        if (show) GameManager.IsGamePlayPaused = true;

        float targetAlpha = 0.0f;

        if (popUpTime < 0.0f) popUpTime = _popupTime;

        if (show)
        {
            targetAlpha = 1.0f;
            this.gameObject.SetActive(true);
            _menuButton.enabled = true;
            _settingsButton.enabled = true;
            _continuePlayingButton.enabled = true;
        }
        else
        {
            _menuButton.enabled = false;
            _settingsButton.enabled = false;
            _continuePlayingButton.enabled = false;

            if (Mathf.Approximately(0.0f, popUpTime)) gameObject.SetActive(false);
        }

        Helpers.FadeImage(new GameObject[] { this.gameObject }, targetAlpha, popUpTime, true);
        Helpers.FadeText(new GameObject[] { this.gameObject }, targetAlpha, popUpTime, true);

        Action lambda = () => {
            this.gameObject.SetActive(show);
            GameManager.IsGamePlayPaused = show;
        };

        if (gameObject.activeSelf && gameObject.activeInHierarchy)
        {
            StartCoroutine(Helpers.DoAfterUnscaledTime(_popupTime, lambda));
        }
        else
        {
            lambda();
        }
    }

    private void Start()
    {
        _menuButton.onClick.AddListener(LoadMenu);
        _continuePlayingButton.onClick.AddListener(() => Show(false));
        //_settingsButton.onClick.AddListener(ShowSettings);
    }

    private void LoadMenu()
    {
        ServiceLocator.Instance.GetService<SceneController>().LoadIntermissionLoading(SceneType.S_MainMenu);
        Show(false);
    }
}
