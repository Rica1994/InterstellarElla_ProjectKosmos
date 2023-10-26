using System.Collections;
using System.Collections.Generic;
using UnityCore.Menus;
using UnityCore.Scene;
using UnityEngine;
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

    #endregion


    private void Awake()
    {
        _levelsIncluded[0] = _3_Venus;
        _levelsIncluded[1] = _1_Mars;
        _levelsIncluded[2] = _4_Saturn;
        _levelsIncluded[3] = _2_Pluto;
        _levelsIncluded[4] = _5_Mercury;
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

        // show buttons after delay
        StartCoroutine(EnableButtonsDelay(0));
        // set scores
        SetPlanetScores();
    }


    private void SetPlanetScores()
    {
        for (int i = 0; i < _levels.Count; i++)
        {
            _levels[i].SetLevelScore(ParseScoreFromScoreValues(_levels[i].MyLevelType));
        }
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
            _factSheet.ShowSheet(false, true, 1.0f);

            StartCoroutine(Helpers.DoAfter(1f, () => 
            {
                _menuAnimator.CameraAnimation.Play(_camZoom);
            }));

            SceneType sceneToLoad = ChooseCorrectSceneWork();

#if UNITY_EDITOR
            // load the loading scene first, then the actual scene for gameplay
            _sceneController.LoadIntermissionLoading(sceneToLoad, false, null, false, PageType.Loading, 1.8f);
#elif UNITY_WEBGL && !UNITY_EDITOR
            StartCoroutine(Helpers.DoAfter(1.8f, () => _sceneController.Load(sceneToLoad)));
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
        if (show)
        {
            _buttonLevelSelect.DisableButton();
            var length = 1.0f;
            _menuAnimator.PlayPlanetSelectAnimation(false);

            _factSheet.InitializeSheet(CurrentLevel.FactSheetData);

            StartCoroutine(Helpers.DoAfter(length, () =>
            {
                _factSheet.ShowSheet(true, true);
                _buttonBackward.DisableButton(true);
                _buttonForward.DisableButton(true);
            }));

            Helpers.FadeImage(new GameObject[] { _buttonForward.gameObject, _buttonBackward.gameObject }, 0.0f, 1.0f);
        }
        else
        {
            _buttonLevelSelect.EnableButton();
            _factSheet.ShowSheet(false, true);

        
            Helpers.FadeImage(new GameObject[] { _buttonForward.gameObject, _buttonBackward.gameObject }, 1.0f, 1.0f);

            StartCoroutine(Helpers.DoAfter(2.0f, () => {
                _menuAnimator.PlayPlanetSelectAnimation(true);
                _buttonBackward.EnableButton();
                _buttonForward.EnableButton();
            }));
        }
    }

    private int GetLevelsToGoUp(string sceneName)
    {
        if (sceneName.Contains("Level") || sceneName.Contains("Quiz"))
            return 2; // For scenes like S_Level_1_0_Introscene, go two levels up
        else
            return 0; // Default case, stay in the current directory
    }
    private string ParseScoreFromScoreValues(LevelType levelType)
    {
        var planetValues = GameManager.Data.PlanetCompletionValues;

        float planetValue = 0.0f;
        switch (levelType)
        {
            case LevelType.Venus:
                planetValue = planetValues.VenusCompletion;
                break;
            case LevelType.Mars:
                planetValue = planetValues.MarsCompletion;
                break;
            case LevelType.Saturn:
                planetValue = planetValues.SaturnCompletion;
                break;
            case LevelType.Pluto:
                planetValue = planetValues.PlutoCompletion;
                break;
            case LevelType.Mercury:
                planetValue = planetValues.MercuryCompletion;
                break;
        }

        return Helpers.FormatPercentageToString(planetValue);
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
    private SceneType ChooseCorrectSceneBuild()
    {
        switch (_levelIndex)
        {
            case 0:
                return SceneType.S_Level_1_Intro;
            case 1:
                return SceneType.S_Level_2_Intro;
            case 2:
                return SceneType.S_Level_3_Intro;
            case 3:
                return SceneType.S_Level_4_Intro;
            case 4:
                return SceneType.S_Level_5_Intro;
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
}

public enum SceneChoice
{
    Work = 0,
    Build = 1
}

