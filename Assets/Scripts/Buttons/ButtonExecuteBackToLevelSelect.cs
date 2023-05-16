using System.Collections;
using System.Collections.Generic;
using UnityCore.Menus;
using UnityCore.Scene;
using UnityEngine;

public class ButtonExecuteBackToLevelSelect : ButtonExecuteBase
{
    public override void MyLogic()
    {
        // create extra if logic as to whether i should even be executed

        base.MyLogic();
      
        ServiceLocator.Instance.GetService<SceneController>().LoadIntermissionLoading(SceneType.S_MainMenu, null, false, PageType.Loading, 0.8f);
    }
}
