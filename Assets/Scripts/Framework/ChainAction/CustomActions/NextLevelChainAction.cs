using System.Collections;
using System.Collections.Generic;
using UnityCore.Scene;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class NextLevelChainAction : ChainAction
{
    [SerializeField]
    private SceneType _sceneToLoad;


    [SerializeField, FormerlySerializedAs("_endGame")]
    private bool _endLevel = false;

    [SerializeField, FormerlySerializedAs("_endGame")]
    private bool _isLastLevel = false;

    public override void Execute()
    {
        base.Execute();
        if (_endLevel)
        {
            ServiceLocator.Instance.GetService<GameManager>().EndLevel(_isLastLevel);
        }
        ServiceLocator.Instance.GetService<SceneController>().LoadIntermissionLoading(_sceneToLoad, false, null, false, UnityCore.Menus.PageType.Loading);

    }
}
