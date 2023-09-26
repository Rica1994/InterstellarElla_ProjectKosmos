using System.Collections;
using System.Collections.Generic;
using UnityCore.Scene;
using UnityEngine;


public class LoadSceneButton : MonoBehaviour
{
    [SerializeField]
    private SceneType _sceneType;
    
    public void Load()
    {
        ServiceLocator.Instance.GetService<SceneController>().Load(_sceneType);
    }
}
