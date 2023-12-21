using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Menus;
using UnityCore.Scene;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;


public class MainMenuManager : Service
{
    [Header("Included Levels")]
    [SerializeField]
    private bool _3_Venus = true;
    [SerializeField]
    private SceneType _venusStartScene;

    [SerializeField]
    private bool _1_Mars = true;
    [SerializeField]
    private SceneType _marsStartScene;

    [SerializeField]
    private bool _4_Saturn = true;
    [SerializeField]
    private SceneType _saturnStartScene;

    [SerializeField]
    private bool _2_Pluto = true;
    [SerializeField]
    private SceneType _plutoStartScene;

    [SerializeField]
    private bool _5_Mercury = true;
    [SerializeField]
    private SceneType _mercuryStartScene;

    private bool[] _levelsIncluded = new bool[5];

    [Header("What types of scenes do I load ?")]
    public SceneChoice ScenesToLoad;

    private int _levelIndex = 0;
    private MenuLevel _currentLevel;
    public MenuLevel CurrentLevel => _currentLevel;

    [Header("Menu Animator")]
    [SerializeField]
    private MenuAnimator _menuAnimator;

    [Header("Buttons for level selection")]
    [SerializeField]
    private ButtonBase _buttonForward;
    [SerializeField]
    private ButtonBase _buttonBackward;
    [SerializeField]
    private ButtonBase _buttonLevelSelect;

    [SerializeField]
    private AudioClip _openFactSheetClip;

    [SerializeField]
    private AudioClip _closeFactSheetClip;

    [SerializeField]
    private AudioClip _zoomOnPlanetClip;

    [SerializeField]
    private AudioClip _zoomOutPlanetClip;

    [SerializeField]
    private AudioClip _startLevelClip;

    [SerializeField]
    private FactSheet _factSheet;

    private List<MenuLevel> _levels = new List<MenuLevel>();

    private SceneController _sceneController;


    #region Animation / Animator strings
    // animation strings
    private const string _levelScaleUp = "A_MenuLevelScaleUp";
    private const string _levelScaleDown = "A_MenuLevelScaleDown";
    private const string _levelScalePop = "A_MenuLevelScalePop";
    private const string _levelRotateFast = "A_MenuLevelRotateFast";

    // Animator strings
    private const string _triggerForward = "GoForward";
    private const string _triggerBackward = "GoBackward";

    private const string _level_1_2 = "A_Rotate_L1-L2";
    private const string _level_1_5 = "A_Rotate_L1-L5";
    private const string _level_2_3 = "A_Rotate_L2-L3";
    private const string _level_2_1 = "A_Rotate_L2-L1";
    private const string _level_3_4 = "A_Rotate_L3-L4";
    private const string _level_3_2 = "A_Rotate_L3-L2";
    private const string _level_4_5 = "A_Rotate_L4-L5";
    private const string _level_4_3 = "A_Rotate_L4-L3";
    private const string _level_5_1 = "A_Rotate_L5-L1";
    private const string _level_5_4 = "A_Rotate_L5-L4";

    private const string _camZoom = "A_CameraLevelZoom";
    private const string _camLevelSelected = "A_CameraPlanetSelected";

    [SerializeField]
    private PlayableDirector _introTextPlayableDirector;

    [SerializeField]
    private GameObject _lockedSign;
    [SerializeField]
    private Material _mercuryMaterial;
    [SerializeField]
    private MeshRenderer _mercuryMesh;
    [SerializeField]
    private GameObject _mercuryText;

    private bool _mercuryLocked => (GameManager.Data.PlanetCompletionValues.VenusCompletion > 0
                && GameManager.Data.PlanetCompletionValues.MarsCompletion > 0
                && GameManager.Data.PlanetCompletionValues.SaturnCompletion > 0
                && GameManager.Data.PlanetCompletionValues.PlutoCompletion > 0) == false;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        if (_isDestroyed) return;
        _levelsIncluded[0] = _3_Venus;
        _levelsIncluded[1] = _1_Mars;
        _levelsIncluded[2] = _4_Saturn;
        _levelsIncluded[3] = _2_Pluto;
        _levelsIncluded[4] = _5_Mercury;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Opens the fact sheet when the scene is loaded and it comes from a quiz
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        var lastPlanet = (GameManager.Planet)GameManager.Data.LastPlanet;
        if (lastPlanet == GameManager.Planet.None)
        {
            _introTextPlayableDirector.Play();
            float waitTime = (float)_introTextPlayableDirector.duration;
            StartCoroutine(Helpers.DoAfter(waitTime, _menuAnimator.ShowPlanets));
            StartCoroutine(EnableButtonsDelay(waitTime));
            return;
        }
        else
        {
            _menuAnimator.ShowPlanets();
            StartCoroutine(EnableButtonsDelay(3));
            
            if (_mercuryLocked == false)
            {
                _mercuryMesh.material = _mercuryMaterial;
                _lockedSign.SetActive(false);
                _mercuryText.SetActive(true);
            }
        }

        StartCoroutine(Helpers.DoAfterFrame(() =>
        {
            while (_currentLevel.MyPlanetType != lastPlanet)
            {
                ForwardLevel();
            }

            ShowPlanetSheet(true);
        }));
    }

    private void Start()
    {
        _sceneController = ServiceLocator.Instance.GetService<SceneController>();
        _levels = _menuAnimator.MenuLevels;
        _levelIndex = 0;
        _currentLevel = _levels[0];

        // hide text on planets
        for (int i = 0; i < _levels.Count; i++)
        {
            _levels[i].HideAllText();
        }
        // shrink all planets first
        for (int i = 0; i < _levels.Count; i++)
        {
            _levels[i].AnimationScaler.Play();
        }
        // size up level 1
        ScaleLevelUp();
    }


    public void LoadLevel()
    {
        if (_levelsIncluded[_levelIndex])
        {
            // hide the forward & backward buttons
            _buttonBackward.DisableButton();
            _buttonForward.DisableButton();
            _buttonLevelSelect.DisableButton();

            // slowly scale up the clicked level, as a fade out takes place
            _currentLevel.AnimationScaler.Play(_levelScalePop);
            _currentLevel.AnimationRotater.Play(_levelRotateFast);

            // zoom camera (needs to be animation
            _menuAnimator.PlayPlanetSelectAnimation(true);

            // play music
            ServiceLocator.Instance.GetService<SoundManager>().PlayClip(_startLevelClip);

            _factSheet.ShowSheet(false, true, 1.0f);

            StartCoroutine(Helpers.DoAfter(1f, () =>
            {
                _menuAnimator.ZoomPlanet();
            }));

            SceneType sceneToLoad = ChooseCorrectSceneWork();
#if UNITY_EDITOR
            // load the loading scene first, then the actual scene for gameplay
            _sceneController.LoadIntermissionLoading(sceneToLoad, false, null, false, PageType.Loading, _startLevelClip.length);
#elif UNITY_WEBGL && !UNITY_EDITOR
            sceneToLoad = GetSceneToLoad();
            Debug.Log("Scene to load: " + sceneToLoad);
            GameManager.Data.CurrentScore = GetPlanetLastScore();
            Debug.Log("Last score to load: " + GameManager.Data.CurrentScore);
            StartCoroutine(Helpers.DoAfter(_startLevelClip.length, () => _sceneController.Load(sceneToLoad)));
#endif
            // switch (ScenesToLoad)
            // {
            //     case SceneChoice.Work:
            //         sceneToLoad = ChooseCorrectSceneWork();
            //         break;
            //     case SceneChoice.Build:
            //         sceneToLoad = ChooseCorrectSceneBuild();
            //         break;
            // }

        }
    }

    public void ShowPlanetSheet(bool show)
    {
        var soundManager = ServiceLocator.Instance.GetService<SoundManager>();

        if (show)
        {
            _currentLevel.HideAllText(false, 1.0f);

            _buttonLevelSelect.DisableButton();
            _menuAnimator.PlayPlanetSelectAnimation(false);
            StartCoroutine(Helpers.DoAfter(0.25f, () => soundManager.PlaySFX(_zoomOnPlanetClip, true)));

            _factSheet.InitializeSheet(CurrentLevel.FactSheetData);


            StartCoroutine(Helpers.DoAfter(1.0f, () =>
            {
                soundManager.PlaySFX(_openFactSheetClip, true);
                _factSheet.ShowSheet(true, true, _openFactSheetClip.length);
                _buttonBackward.DisableButton(true);
                _buttonForward.DisableButton(true);
            }));

            Helpers.FadeImage(new GameObject[] { _buttonForward.gameObject, _buttonBackward.gameObject }, 0.0f, 1.0f);
        }
        else
        {
            // show planet text

            _buttonLevelSelect.EnableButton();
            _factSheet.ShowSheet(false, true, _closeFactSheetClip.length);
            soundManager.PlaySFX(_closeFactSheetClip, true);

            Helpers.FadeImage(new GameObject[] { _buttonForward.gameObject, _buttonBackward.gameObject }, 1.0f, _closeFactSheetClip.length);

            StartCoroutine(Helpers.DoAfter(_closeFactSheetClip.length + 0.25f, () =>
            {
                soundManager.PlaySFX(_zoomOutPlanetClip, true);
                _currentLevel.ShowAllText(false, 1.0f);

                _menuAnimator.PlayPlanetSelectAnimation(true);
                _buttonBackward.EnableButton();
                _buttonForward.EnableButton();
            }));
        }
    }

    public string GetPlanetNameFromEnum(SceneType sceneType)
    {
        var sceneTypeString = sceneType.ToString();
        if (sceneTypeString.Contains("Level_1") || sceneTypeString.Contains("Mars"))
            return "Mars/";
        else if (sceneTypeString.Contains("Level_2"))
            return "Pluto/";
        else if (sceneTypeString.Contains("Level_3"))
            return "Venus/";
        else if (sceneTypeString.Contains("Level_4"))
            return "Saturn/";
        else if (sceneTypeString.Contains("Level_5"))
            return "Mercury/";
        else if (sceneTypeString.Contains("Quiz"))
            return "Quiz/";
        else return "";
    }

    public void ForwardLevel()
    {
        _menuAnimator.MyAnimator.SetTrigger(_triggerForward);

        ScaleLevelDown();

        if ((_levelIndex + 1) >= 5)
        {
            _levelIndex = 0;
        }
        else
        {
            _levelIndex += 1;
        }
        _currentLevel = _levels[_levelIndex];

        RotateLevelSetup(true);

        ScaleLevelUp();

        if (_currentLevel.MyPlanetType == GameManager.Planet.Mercury && _mercuryLocked)
        {
            _buttonLevelSelect.DisableButton(true);
        }
        else
        {
            _buttonLevelSelect.EnableButton();
        }
    }

    public void BackwardLevel()
    {
        _menuAnimator.MyAnimator.SetTrigger(_triggerBackward);

        ScaleLevelDown();

        if ((_levelIndex - 1) < 0)
        {
            _levelIndex = 4;
        }
        else
        {
            _levelIndex -= 1;
        }
        _currentLevel = _levels[_levelIndex];

        RotateLevelSetup(false);

        ScaleLevelUp();

        if (_currentLevel.MyPlanetType == GameManager.Planet.Mercury && _mercuryLocked)
        {
            _buttonLevelSelect.DisableButton(true);
        }
        else
        {
            _buttonLevelSelect.EnableButton();
        }
    }

    private void RotateLevelSetup(bool isForward)
    {
        if (isForward == true)
        {
            switch (_levelIndex)
            {
                case 0:
                    _menuAnimator.MyAnimator.Play(_level_5_1);
                    break;
                case 1:
                    _menuAnimator.MyAnimator.Play(_level_1_2);
                    break;
                case 2:
                    _menuAnimator.MyAnimator.Play(_level_2_3);
                    break;
                case 3:
                    _menuAnimator.MyAnimator.Play(_level_3_4);
                    break;
                case 4:
                    _menuAnimator.MyAnimator.Play(_level_4_5);
                    break;
            }
        }
        else
        {
            switch (_levelIndex)
            {
                case 0:
                    _menuAnimator.MyAnimator.Play(_level_2_1);
                    break;
                case 1:
                    _menuAnimator.MyAnimator.Play(_level_3_2);
                    break;
                case 2:
                    _menuAnimator.MyAnimator.Play(_level_4_3);
                    break;
                case 3:
                    _menuAnimator.MyAnimator.Play(_level_5_4);
                    break;
                case 4:
                    _menuAnimator.MyAnimator.Play(_level_1_5);
                    break;
            }
        }
    }
    private void ScaleLevelUp()
    {
        _currentLevel.AnimationScaler.Play(_levelScaleUp);

        _currentLevel.PopupAnimationsText();
    }
    private void ScaleLevelDown()
    {
        _currentLevel.AnimationScaler.Play(_levelScaleDown);

        _currentLevel.PoofAnimationsText();
    }

    private SceneType ChooseCorrectSceneWork()
    {
        switch (_levelIndex)
        {
            case 0:
                return _venusStartScene;
            case 1:
                return _marsStartScene;
            case 2:
                return _saturnStartScene;
            case 3:
                return _plutoStartScene;
            case 4:
                return _mercuryStartScene;
            default:
                return SceneType.None;
        }
    }

    private IEnumerator EnableButtonsDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        _buttonBackward.EnableButton();
        _buttonForward.EnableButton();
        _buttonLevelSelect.EnableButton();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private int GetPlanetLastScore()
    {
        switch (_currentLevel.MyPlanetType)
        {
            case GameManager.Planet.Mars:
                return GameManager.Data.PlanetLastScores.MarsLastScore;  
            case GameManager.Planet.Pluto:
                return GameManager.Data.PlanetLastScores.PlutoLastScore;  
            case GameManager.Planet.Venus:
                return GameManager.Data.PlanetLastScores.VenusLastScore;  
            case GameManager.Planet.Saturn:
                return GameManager.Data.PlanetLastScores.SaturnLastScore;  
            case GameManager.Planet.Mercury:
                return GameManager.Data.PlanetLastScores.MercuryLastScore;  
            case GameManager.Planet.None:
                return -1;
        }
        return -1;
    }

    private SceneType GetSceneToLoad()
    {
        switch (_currentLevel.MyPlanetType)
        {
            case GameManager.Planet.Mars:
                switch (GameManager.Data.PlanetLastScenes.MarsLastScene)
                {
                    case 0:
                        return SceneType.S_Level_1_Intro;
                    case 1:
                        return SceneType.S_Level_1_1_Work;
                    case 2:
                        return SceneType.S_Level_1_Outro;
                    case 3:
                        return SceneType.S_Quiz;
                }
                break;
            case GameManager.Planet.Pluto:
                switch (GameManager.Data.PlanetLastScenes.PlutoLastScene)
                {
                    case 0:
                        return SceneType.S_Level_2_Intro;
                    case 1:
                        return SceneType.S_Level_2_0_Work;
                    case 2:
                        return SceneType.S_Level_2_1_Work;
                    case 3:
                        return SceneType.S_Level_2_2_Work;
                    case 4:
                        return SceneType.S_Level_2_Outro;
                    case 5:
                        return SceneType.S_Quiz;
                }
                break;
            case GameManager.Planet.Venus:
                switch (GameManager.Data.PlanetLastScenes.VenusLastScene)
                {
                    case 0:
                        return SceneType.S_Level_3_Intro;
                    case 1:
                        return SceneType.S_Level_3_0_Work;
                    case 2:
                        return SceneType.S_Level_3_1_Work;
                    case 3:
                        return SceneType.S_Level_3_2_Work;
                    case 4:
                        return SceneType.S_Level_3_3_Work;
                    case 5:
                        return SceneType.S_Level_3_4_Work;
                    case 6:
                        return SceneType.S_Level_3_5_Work;
                    case 7:
                        return SceneType.S_Level_3_Outro;
                    case 8:
                        return SceneType.S_Quiz;
                }
                break;
            case GameManager.Planet.Saturn:
                switch (GameManager.Data.PlanetLastScenes.SaturnLastScene)
                {
                    case 0:
                        return SceneType.S_Level_4_Intro;
                    case 1:
                        return SceneType.S_Level_4_0_Work;
                    case 2:
                        return SceneType.S_Level_4_1_Work;
                    case 3:
                        return SceneType.S_Level_4_2_Work;
                    case 4:
                        return SceneType.S_Level_4_3_Work;
                    case 5:
                        return SceneType.S_Level_4_4_Work;
                    case 6:
                        return SceneType.S_Level_4_5_Work;
                    case 7:
                        return SceneType.S_Level_4_Outro;
                    case 8:
                        return SceneType.S_Quiz;
                }
                break;
            case GameManager.Planet.Mercury:
                switch (GameManager.Data.PlanetLastScenes.MercuryLastScene)
                {
                    case 0:
                        return SceneType.S_Level_5_Intro;
                    case 1:
                        return SceneType.S_Level_5_0_Work;
                    case 2:
                        return SceneType.S_Level_5_1_Work;
                    case 3:
                        return SceneType.S_Level_5_2_Work;
                    case 4:
                        return SceneType.S_Level_5_3_Work;
                    case 5:
                        return SceneType.S_Level_5_Outro;
                    case 6:
                        return SceneType.S_Quiz;
                }
                break;
            case GameManager.Planet.None:
                return SceneType.None;
        }
        return SceneType.None;
    }
}

public enum SceneChoice
{
    Work = 0,
    Build = 1
}

