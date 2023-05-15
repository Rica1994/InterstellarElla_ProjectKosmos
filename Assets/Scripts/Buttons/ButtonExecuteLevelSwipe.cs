using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonExecuteLevelSwipe : ButtonExecuteBase
{
    [SerializeField]
    private bool _isForward;

    public override void MyLogic()
    {
        MainMenuManager menuManager = ServiceLocator.Instance.GetService<MainMenuManager>();

        if (_isForward == true)
        {
            menuManager.ForwardLevel();
        }
        else
        {
            menuManager.BackwardLevel();
        }
    }
}
