using System.Collections;
using System.Collections.Generic;
using UnityCore.Scene;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField]
    private SceneType _sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        // get the next scene depending on my scene index
        ServiceLocator.Instance.GetService<SceneController>().LoadIntermissionLoading(_sceneToLoad, null, false, UnityCore.Menus.PageType.Loading);
    }
}
