using System.Collections;
using System.Collections.Generic;
using UnityCore.Scene;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class NextLevelChainAction : ChainAction
{
    [SerializeField]
    private SceneType _sceneToLoad;

    [SerializeField]
    private bool _endGame = false;

    public override void Execute()
    {
        base.Execute();
        if (_endGame)
        {
            ServiceLocator.Instance.GetService<GameManager>().EndGame();
        }
        ServiceLocator.Instance.GetService<SceneController>().LoadIntermissionLoading(_sceneToLoad, false, null, false, UnityCore.Menus.PageType.Loading);

    }
}
