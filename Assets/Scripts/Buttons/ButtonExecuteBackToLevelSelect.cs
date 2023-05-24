using System.Collections;
using System.Collections.Generic;
using UnityCore.Menus;
using UnityCore.Scene;
using UnityEngine;

public class ButtonExecuteBackToLevelSelect : ButtonExecuteBase
{
    private PageController _pageController;

    protected virtual void Start()
    {
        _pageController = ServiceLocator.Instance.GetService<PageController>();
    }

    public override void MyLogic()
    {
        // create extra if logic as to whether i should even be executed

        base.MyLogic();

        _pageController.TurnPageOff(PageType.Pause);

        ServiceLocator.Instance.GetService<SceneController>().LoadIntermissionLoading(SceneType.S_MainMenu, null, false, PageType.Loading, 0.8f);

        //ServiceLocator.Instance.GetService<SceneController>().TargetSceneAfterLoading = SceneType.S_MainMenu;
        //ServiceLocator.Instance.GetService<SceneController>().Load(SceneType.S_Loading, null, false, PageType.Loading);
    }
}
