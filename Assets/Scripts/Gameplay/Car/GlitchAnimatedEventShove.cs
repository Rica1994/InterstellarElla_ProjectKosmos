using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class GlitchAnimatedEventShove : GlitchAnimatedEvent
{
    [Header("Swap-Cameras to enable/disable")]
    [SerializeField]
    private SwapCameraGlitch _swapCamToDisable;
    [SerializeField]
    private SwapCameraGlitch _swapCamToEnable;


    protected override void Start()
    {
        base.Start();

        // disable/enable the cameras
        if (_swapCamToEnable != null)
        {
            _swapCamToEnable.gameObject.SetActive(false);
        }
        if (_swapCamToDisable != null)
        {
            _swapCamToDisable.gameObject.SetActive(true);
        }
    }

    public override void ActivateCameraCutscene(SimpleCarController glitch)
    {
        base.ActivateCameraCutscene(glitch);

        StartCoroutine(UseNewCameras());
    }


    private IEnumerator UseNewCameras()
    {
        // wait for the amount of time in the animation - 2f (still see jumpPad activate within cutscene)
        yield return new WaitForSeconds(_animationCamera.clip.length + 0.1f);

        // disabled one
        if (_swapCamToDisable != null)
        {
            _swapCamToDisable.gameObject.SetActive(false);
        }

        // enable one
        if (_swapCamToEnable != null)
        {
            _swapCamToEnable.gameObject.SetActive(true);
        }
    }
}
