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


    public override void ActivateCameraCutscene(SimpleCarController glitch)
    {
        base.ActivateCameraCutscene(glitch);
    }


    public void SmashRocks()
    {
        // plays animation
        _rockWallNew.BoulderSmashRockWall();

        // plays sound
        _audioController.PlayAudio(_soundSmash);
    }
}
