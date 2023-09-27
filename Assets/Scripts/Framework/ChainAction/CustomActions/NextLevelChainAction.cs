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

    public override void Execute()
    {
        base.Execute();
        ServiceLocator.Instance.GetService<SceneController>().LoadIntermissionLoading(_sceneToLoad, false, null, false, UnityCore.Menus.PageType.Loading);
    }
}
