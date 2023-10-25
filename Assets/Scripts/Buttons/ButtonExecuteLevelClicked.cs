using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonExecuteLevelClicked : ButtonExecuteBase
{
    public override void MyLogic()
    {
        base.MyLogic();

        MainMenuManager menuManager = ServiceLocator.Instance.GetService<MainMenuManager>();

        //menuManager.LoadLevel();
        menuManager.ShowPlanetSheet(true);
    }
}
