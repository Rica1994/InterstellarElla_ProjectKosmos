using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityCore.Menus;
using UnityCore.Scene;
using UnityEngine;

public class TestingServices : MonoBehaviour
{
    ServiceLocator _serviceLocator;

    AudioController _audioCon;
    PageController _pageCon;
    SceneController _sceneCon;





    void Start()
    {
        _serviceLocator = ServiceLocator.Instance;

        _audioCon = _serviceLocator.GetService<AudioController>();
        _pageCon = _serviceLocator.GetService<PageController>();
        _sceneCon = _serviceLocator.GetService<SceneController>();


        Debug.Log(_audioCon.TracksOST.Count + " = the amount of tracks for the ost");
    }


}
