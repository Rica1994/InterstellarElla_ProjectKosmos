using System.Collections;
using System.Collections.Generic;
using UnityCore.Scene;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    private LevelManager _levelManager;

    private void OnEnable()
    {
        _levelManager = ServiceLocator.Instance.GetService<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _levelManager = ServiceLocator.Instance.GetService<LevelManager>();
        ServiceLocator.Instance.GetService<SceneController>().LoadIntermissionLoading(_levelManager.NextScene, null, false, UnityCore.Menus.PageType.Loading);
    }   
}
