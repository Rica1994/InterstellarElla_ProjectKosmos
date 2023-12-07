using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityCore.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUpCheck : MonoBehaviour
{
    [SerializeField]
    private Button _startButton;

    [SerializeField]
    private TextMeshProUGUI _buttonText;

    [SerializeField]
    private Image _buttonImage;

    [SerializeField]
    private WebGLPerformanceBenchmark _performanceBenchmark;

    [SerializeField]
    private GameObject _startupScreenCanvas;

    private void Start()
    {
        _startButton.enabled = false;
        _buttonImage.fillAmount = 0;
        _buttonText.text = "LADEN";
    }

    private void Update()
    {
        if (_performanceBenchmark.enabled)
        {
            _buttonImage.fillAmount = _performanceBenchmark.BenchMarkingProgress;
        }
        else
        {
            _buttonImage.fillAmount = 1.0f;
            _buttonText.text = "START";
            _buttonText.color = new Color(0.48f, 0.0f, 0.48f, 1.0f);
            _startButton.enabled = true;
            enabled = false;
        }
    }

    public void StartGame()
    {
        var sceneController = ServiceLocator.Instance.GetService<SceneController>();

        var gameData = GameManager.Data;
        SceneType toLoadScene;
        LoadSceneMode loadMode;

        if (gameData.PlanetCompletionValues.PlutoCompletion > 0 || gameData.PlanetCompletionValues.MarsCompletion > 0
            || gameData.PlanetCompletionValues.VenusCompletion > 0 || gameData.PlanetCompletionValues.SaturnCompletion > 0
            || gameData.PlanetCompletionValues.MercuryCompletion > 0)
        {
            toLoadScene = SceneType.S_MainMenu;
            loadMode = LoadSceneMode.Single;
        }
        else
        {
            toLoadScene = SceneType.S_GameStartUpScene;
            loadMode = LoadSceneMode.Additive;
        }

        Helpers.FadeImage(new GameObject[] { _startupScreenCanvas }, 0.0f, 1.0f);
        Helpers.FadeText(new GameObject[] { _startupScreenCanvas}, 0.0f, 1.0f);

        ServiceLocator.Instance.GetService<SceneController>().LoadIntermissionLoading(toLoadScene, loadMode: loadMode);
    }
}
