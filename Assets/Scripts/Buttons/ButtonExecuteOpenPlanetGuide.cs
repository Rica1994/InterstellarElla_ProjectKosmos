using System.Collections;
using System.Collections.Generic;
using UnityCore.Menus;
using UnityEngine;

public class ButtonExecuteOpenPlanetGuide : ButtonExecuteBase
{
    public override void MyLogic()
    {
        base.MyLogic();

        // opens up the guide specific to the planet shown
        ServiceLocator.Instance.GetService<PageController>().TurnPageOn(PageType.Fiche);
    }
}
