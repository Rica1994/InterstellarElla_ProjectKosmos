using System.Collections;
using System.Collections.Generic;
using UnityCore.Menus;
using UnityCore.Scene;
using UnityEngine;

public class ButtonExecuteLevelSkip : ButtonExecuteBase
{
    private LevelManager _levelManager;




    private void OnEnable()
    {
        // do not do this if we're in the main menu
        if (FindObjectOfType<MainMenuManager>() != null)
        {

        }
        else if (FindObjectOfType<SkipCutscene>() != null)
        {
            
        }
        else
        {
            _levelManager = ServiceLocator.Instance.GetService<LevelManager>();
        }
    }

    public override void MyLogic()
    {
        base.MyLogic();

        if (FindObjectOfType<MainMenuManager>() != null)
        {
            // we're in the main menu
            Debug.LogWarning("this is the main menu !!! loading nothing");
        }
        else
        {
            _levelManager = ServiceLocator.Instance.GetService<LevelManager>();

            if (_levelManager == null)
            {
                Debug.LogWarning("no level manager present in this scene !!! loading nothing");
                return;
            }
            if (_levelManager.NextScene == SceneType.None)
            {
                Debug.LogWarning("no scene-type chosen as the next scene on the levelmanager !!! loading nothing");
                return;
            }
            ServiceLocator.Instance.GetService<SceneController>().LoadIntermissionLoading(_levelManager.NextScene, null, false, UnityCore.Menus.PageType.Loading);
        }
    }
}
