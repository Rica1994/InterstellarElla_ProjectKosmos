﻿using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityCore.Scene;
using UnityEngine;

public class GlitchAnimatedEventBoulder : GlitchAnimatedEvent
{
    [SerializeField]
    private AudioClip _soundRockSmash;

    [SerializeField]
    private AudioSource _rockAudioSource;

    [Header("Rock Wall New")]
    [SerializeField]
    private RockWallNew _rockWallNew;

    [Header("Swap Cameras")]
    [SerializeField]
    private List<SwapCameraGlitch> _swapCamerasToDisableOnEvent = new List<SwapCameraGlitch>();
    [SerializeField]
    private List<SwapCameraGlitch> _swapCamerasToEnableOnEvent = new List<SwapCameraGlitch>();

    [Header("Ends level ?")]
    [SerializeField]
    private bool _endsLevel;
    [SerializeField]
    private float _waitTimeToEndLevel = 12f;

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < _swapCamerasToDisableOnEvent.Count; i++)
        {
            _swapCamerasToDisableOnEvent[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < _swapCamerasToEnableOnEvent.Count; i++)
        {
            _swapCamerasToEnableOnEvent[i].gameObject.SetActive(false);
        }
    }

    public override void ActivateCameraCutscene(SimpleCarController glitch)
    {
        base.ActivateCameraCutscene(glitch);
        _rockAudioSource.Play();

        for (int i = 0; i < _swapCamerasToDisableOnEvent.Count; i++)
        {
            _swapCamerasToDisableOnEvent[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < _swapCamerasToEnableOnEvent.Count; i++)
        {
            _swapCamerasToEnableOnEvent[i].gameObject.SetActive(true);
        }

        if (_endsLevel == true)
        {
            StartCoroutine(EndLevelRoutine());
        }      
    }


    public void SmashRocks()
    {
        // plays animation
        _rockWallNew.BoulderSmashRockWall();

        _rockAudioSource.Stop();
        _rockAudioSource.clip = _soundRockSmash;
        _rockAudioSource.Play();
    }


    private IEnumerator EndLevelRoutine()
    {
        yield return new WaitForSeconds(_waitTimeToEndLevel);

        var levelManager = ServiceLocator.Instance.GetService<LevelManager>();
        levelManager.EndLevel();
        ServiceLocator.Instance.GetService<SceneController>().LoadIntermissionLoading(levelManager.NextScene, levelManager.IsSameBuildNextScene, 
            null, false, UnityCore.Menus.PageType.Loading);
    }
}
