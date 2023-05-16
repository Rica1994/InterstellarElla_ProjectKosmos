using System.Collections;
using System.Collections.Generic;
using UnityCore.Menus;
using UnityEngine;

public class ButtonExecutePause : ButtonExecuteBase
{
    private PageController _pageController;


    protected virtual void Start()
    {
        _pageController = ServiceLocator.Instance.GetService<PageController>();
    }


    public override void MyLogic()
    {
        base.MyLogic();

        _pageController.TurnPageOn(PageType.Pause);
    }
}
