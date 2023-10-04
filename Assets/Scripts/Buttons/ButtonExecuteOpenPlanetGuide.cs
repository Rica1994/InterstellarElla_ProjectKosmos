using System.Collections;
using System.Collections.Generic;
using UnityCore.Menus;
using UnityEngine;

public class ButtonExecuteOpenPlanetGuide : ButtonExecuteBase
{
    public bool IsMainMenu;

    public override void MyLogic()
    {
        base.MyLogic();

        if (IsMainMenu == true)
        {
            // enable the correct fiche
            ServiceLocator.Instance.GetService<FicheController>().OpenCorrectFicheMenu(ServiceLocator.Instance.GetService<MainMenuManager>().CurrentLevel.MyLevelType);
        }
        else
        {
            // enable the correct fiche
            ServiceLocator.Instance.GetService<FicheController>().OpenCorrectFicheGameplay(ServiceLocator.Instance.GetService<GameManager>().GetCurrentPlanet());
        }

        // opens up the page for planet-fiches
        ServiceLocator.Instance.GetService<PageController>().TurnPageOn(PageType.Fiche);
    }
}
