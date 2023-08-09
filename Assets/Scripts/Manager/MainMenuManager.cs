using System.Collections;
using System.Collections.Generic;
using UnityCore.Menus;
using UnityCore.Scene;
using UnityEngine;

public class MainMenuManager : Service
{
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

    private List<MenuLevel> _levels = new List<MenuLevel>();

    private SceneController _sceneController;

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



    private void Start()
    {
        _sceneController = ServiceLocator.Instance.GetService<SceneController>();

        _levels = _menuAnimator.MenuLevels;

        _levelIndex = 0;
        _currentLevel = _levels[0];

        // shrink all planets first
        for (int i = 0; i < _levels.Count; i++)
        {
            _levels[i].AnimationScaler.Play();
        }

        // size up level 1
        _levels[0].AnimationScaler.Play(_levelScaleUp);

        // show buttons after delay
        StartCoroutine(EnableButtonsDelay(0));
    }



    public void LoadLevel()
    {
        // hide the forward & backward buttons
        _buttonBackward.DisableButton();
        _buttonForward.DisableButton();
        _buttonLevelSelect.DisableButton();

        // slowly scale up the clicked level, as a fade out takes place
        _currentLevel.AnimationScaler.Play(_levelScalePop);
        _currentLevel.AnimationRotater.Play(_levelRotateFast);

        // zoom camera (needs to be animation
        _menuAnimator.CameraAnimation.Play(_camZoom);

        SceneType sceneToLoad = SceneType.None;
        switch (ScenesToLoad)
        {
            case SceneChoice.Work:
                sceneToLoad = ChooseCorrectSceneWork();
                break;
            case SceneChoice.Build:
                sceneToLoad = ChooseCorrectSceneBuild();
                break;
        }

        // load the loading scene first, then the actual scene for gameplay
        _sceneController.LoadIntermissionLoading(sceneToLoad, null, false, PageType.Loading, 0.8f);
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
    }
    private void ScaleLevelDown()
    {
        _currentLevel.AnimationScaler.Play(_levelScaleDown);
    }

    private SceneType ChooseCorrectSceneWork()
    {
        switch (_levelIndex)
        {
            case 0:
                return SceneType.S_Level_1_0_Work;
            case 1:
                return SceneType.S_Level_2_0_Work;
            case 2:
                return SceneType.S_Level_3_0_Work;
            case 3:
                return SceneType.S_Level_4_0_Work;
            case 4:
                return SceneType.S_Level_5_0_Work;
            default:
                return SceneType.None;
        }
    }
    private SceneType ChooseCorrectSceneBuild()
    {
        switch (_levelIndex)
        {
            case 0:
                return SceneType.S_Mars_1_0_IntroCutscene;
            case 1:
                return SceneType.S_Level_2_0_Build;
            case 2:
                return SceneType.S_Level_3_0_Build;
            case 3:
                return SceneType.S_Level_4_0_Build;
            case 4:
                return SceneType.S_Level_4_0_Build;
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

