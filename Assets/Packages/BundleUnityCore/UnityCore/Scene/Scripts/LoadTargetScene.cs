using System.Collections;
using System.Collections.Generic;
using UnityCore.Scene;
using UnityEngine;

public class LoadTargetScene : MonoBehaviour
{
    [SerializeField]
    private float _delayToLoadNewScene = 2;

    void Start()
    {
        StartCoroutine(LoadTarget());
    }

    private IEnumerator LoadTarget()
    {
        yield return new WaitForSeconds(_delayToLoadNewScene);

        ServiceLocator.Instance.GetService<SceneController>().LoadTargetAfterLoadingScene(null, false, UnityCore.Menus.PageType.Loading);
    }
}
