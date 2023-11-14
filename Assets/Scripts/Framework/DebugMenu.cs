using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityCore.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    [SerializeField] 
    private Button _showHideDebugMenuButton;

    [SerializeField]
    private GameObject _debugCanvas;

    [SerializeField]
    private Button _increaseGameTimeButton;

    [SerializeField]
    private Button _decreaseGameTimeButton;

    [SerializeField]
    private Button _resetGameTimeButton;

    [SerializeField]
    private TMP_Text _gameTimeText;

    [SerializeField]
    private Button _toggleQualityLevelButton;

    [SerializeField]
    private Button _testSDKButton;

    private void Start()
    {
        // remove ourselves when we are building for the client.
        if (ServiceLocator.Instance.GetService<GameManager>().TargetBuildType == GameManager.BuildType.Client)
        {
            Destroy(gameObject);
        }

        _showHideDebugMenuButton.onClick.AddListener(ShowDebugCanvas);
        _increaseGameTimeButton.onClick.AddListener(IncreaseGameTime);
        _decreaseGameTimeButton.onClick.AddListener(DecreaseGameTime);
        _resetGameTimeButton.onClick.AddListener(ResetGameTime);
        _toggleQualityLevelButton.onClick.AddListener(ToggleQualityLevel);
        _testSDKButton.onClick.AddListener(LoadSDKTestScene);
    }

    private void ShowDebugCanvas()
    {
        _debugCanvas.gameObject.SetActive(!_debugCanvas.gameObject.activeSelf);
    }

    private void IncreaseGameTime()
    {
        Time.timeScale += 0.5f;
        _gameTimeText.text = Time.timeScale.ToString();
    }

    private void DecreaseGameTime()
    {
        Time.timeScale -= 0.5f;
        _gameTimeText.text = Time.timeScale.ToString();
    }

    private void ResetGameTime()
    {
        Time.timeScale = 1.0f;
        _gameTimeText.text = Time.timeScale.ToString();
    }

    private void OnDestroy()
    {
        _showHideDebugMenuButton.onClick.RemoveAllListeners();
        _increaseGameTimeButton.onClick.RemoveAllListeners();
        _decreaseGameTimeButton.onClick.RemoveAllListeners();
        _resetGameTimeButton.onClick.RemoveAllListeners();
        _toggleQualityLevelButton.onClick.RemoveAllListeners();
    }

    private void ToggleQualityLevel()
    {
        ServiceLocator.Instance.GetService<QualitySettingsManager>().ToggleQualityLevel();
    }

    private void LoadSDKTestScene()
    {
        SceneManager.LoadScene("S_SDKTestScene", LoadSceneMode.Additive);
    }
}
