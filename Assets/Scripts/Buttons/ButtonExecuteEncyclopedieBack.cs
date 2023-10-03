using System.Collections;
using System.Collections.Generic;
using UnityCore.Menus;
using UnityEngine;

public class ButtonExecuteEncyclopedieBack : ButtonExecuteBase
{
    public override void MyLogic()
    {
        base.MyLogic();

        ServiceLocator.Instance.GetService<PageController>().TurnPageOff(PageType.Fiche);
    }
}
