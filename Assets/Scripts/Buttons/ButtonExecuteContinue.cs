using System.Collections;
using System.Collections.Generic;
using UnityCore.Menus;
using UnityEngine;

public class ButtonExecuteContinue : ButtonExecuteBase
{
    private PageController _pageController;

    protected virtual void Start()
    {
        _pageController = ServiceLocator.Instance.GetService<PageController>();
    }


    public override void MyLogic()
    {
        base.MyLogic();

        _pageController.TurnPageOff(PageType.Pause);
    }
}
