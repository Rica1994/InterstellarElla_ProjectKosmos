using DG.Tweening;
using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HudManager : Service
{
    [SerializeField]
    private GameObject _hudRoot;

    [SerializeField] private GameObject _joystick;
    [SerializeField] private TouchButton _touchButton;

    [SerializeField]
    private Button _pauseScreenButton;

    [SerializeField]
    private GameObject _scoreCanvas;

    [SerializeField]
    private PauseMenu _pauseMenu;

    [SerializeField]
    private TMP_Text _pickupsCollectedText;

    [SerializeField]
    private TMP_Text _pickupsNeededText;

    public TouchButton TouchButton => _touchButton;

    private DG.Tweening.Sequence _scoreTextSequence;

    private Material _textMaterial;

    [SerializeField]
    private ParticleSystem _particleSystem;

    private void Start()
    {
        _textMaterial = _pickupsCollectedText.fontSharedMaterial;
    }

    private void OnCutSceneEnded()
    {
        _hudRoot.SetActive(true);
        _pauseScreenButton.gameObject.SetActive(true);
    }

    private void OnCutSceneStarted()
    {
        _hudRoot.SetActive(false);
        _pauseScreenButton.gameObject.SetActive(false);
    }

    public void Initialize(GameManager.Planet currentPlanet)
    {
        var gameManager = ServiceLocator.Instance.GetService<GameManager>();

        GameManager.CutsceneStartedEvent += OnCutSceneStarted;
        GameManager.CutsceneEndedEvent += OnCutSceneEnded;

        var isCurrentPlanetNone = currentPlanet == GameManager.Planet.None;

        var sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.IndexOf("Intro", StringComparison.OrdinalIgnoreCase) >= 0 || sceneName.IndexOf("Outro", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            _scoreCanvas.SetActive(false);
            _joystick.SetActive(false);
            _touchButton.gameObject.SetActive(false);
        }
        else
        {
            _scoreCanvas.SetActive(!isCurrentPlanetNone);
        }

        _pauseMenu.gameObject.SetActive(true);
        _pauseMenu.Show(false, 0.0f);

        _pauseScreenButton.onClick.AddListener(OpenPauseMenu);

        if (!gameManager.IsMobileWebGl || isCurrentPlanetNone)
        {
            _joystick.SetActive(false);
            _touchButton.gameObject.SetActive(false);
        }

        ServiceLocator.Instance.GetService<PickUpManager>().PickUpPickedUpEvent += OnPickupPickedUp;

        _pauseScreenButton.gameObject.SetActive(true);

        switch (currentPlanet)
        {
            case GameManager.Planet.Mars:
                _pickupsNeededText.text = GameManager.MARS_DATA_NEEDED.ToString();
                _pickupsCollectedText.text = GameManager.Data.PlanetLastScores.MarsLastScore.ToString();
                break;
            case GameManager.Planet.Pluto:
                _pickupsNeededText.text = GameManager.PLUTO_DATA_NEEDED.ToString();
                _pickupsCollectedText.text = GameManager.Data.PlanetLastScores.PlutoLastScore.ToString();
                break;
            case GameManager.Planet.Venus:
                _pickupsNeededText.text = GameManager.VENUS_DATA_NEEDED.ToString();
                _pickupsCollectedText.text = GameManager.Data.PlanetLastScores.VenusLastScore.ToString();
                break;
            case GameManager.Planet.Saturn:
                _pickupsNeededText.text = GameManager.SATURN_DATA_NEEDED.ToString();
                _pickupsCollectedText.text = GameManager.Data.PlanetLastScores.SaturnLastScore.ToString();
                break;
            case GameManager.Planet.Mercury:
                _pickupsNeededText.text = GameManager.MERCURY_DATA_NEEDED.ToString();
                _pickupsCollectedText.text = GameManager.Data.PlanetLastScores.MercuryLastScore.ToString();
                break;
            case GameManager.Planet.None:
                _pauseScreenButton.gameObject.SetActive(false);
                break;
        }
    }

    private void OnPickupPickedUp(int pickUpsPickedUp)
    {
        if (_scoreTextSequence != null)
        {
            _scoreTextSequence.Kill();
        }

        _pickupsCollectedText.text = pickUpsPickedUp.ToString();
        _pickupsCollectedText.transform.localScale = Vector3.one;

        _scoreTextSequence = DOTween.Sequence();

        _scoreTextSequence.Append(_pickupsCollectedText.transform.DOScale(2.5f, 0.1f))
            .Join(DOTween.To(() => _textMaterial.GetFloat(ShaderUtilities.ID_GlowPower), x => _textMaterial.SetFloat(ShaderUtilities.ID_GlowPower, x), 0.016f, 0.5f))
            .Append(_pickupsCollectedText.transform.DOScale(1.0f, 0.2f))
            .Join(DOTween.To(() => _textMaterial.GetFloat(ShaderUtilities.ID_GlowPower), x => _textMaterial.SetFloat(ShaderUtilities.ID_GlowPower, x), 0.0f, 0.4f));

        if (_particleSystem != null)
        {
            _particleSystem.Play();
        }
    }

    public void SetScore(int pickupsCollected)
    {
        _pickupsCollectedText.text = pickupsCollected.ToString();
    }

    private void OpenPauseMenu()
    {
        _pauseMenu.Show(!_pauseMenu.gameObject.activeSelf);
    }

    public void EnableHUD(bool enable)
    {
        _hudRoot.SetActive(enable);
    }
}
