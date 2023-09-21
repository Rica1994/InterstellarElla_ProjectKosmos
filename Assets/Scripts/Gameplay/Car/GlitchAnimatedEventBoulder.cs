using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class GlitchAnimatedEventBoulder : GlitchAnimatedEvent
{
    [SerializeField]
    private AudioElement _soundSmash;

    [Header("Rock Wall New")]
    [SerializeField]
    private RockWallNew _rockWallNew;

    [Header("Swap Cameras")]
    [SerializeField]
    private List<SwapCameraGlitch> _swapCamerasToDisableOnEvent = new List<SwapCameraGlitch>();
    [SerializeField]
    private List<SwapCameraGlitch> _swapCamerasToEnableOnEvent = new List<SwapCameraGlitch>();

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

        for (int i = 0; i < _swapCamerasToDisableOnEvent.Count; i++)
        {
            _swapCamerasToDisableOnEvent[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < _swapCamerasToEnableOnEvent.Count; i++)
        {
            _swapCamerasToEnableOnEvent[i].gameObject.SetActive(true);
        }
    }


    public void SmashRocks()
    {
        // plays animation
        _rockWallNew.BoulderSmashRockWall();

        // plays sound
        _audioController.PlayAudio(_soundSmash);
    }
}
