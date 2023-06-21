using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("Jump Pad")]
    [SerializeField] private JumpPad _jumpPad;

    [Header("Cutscene")]
    [SerializeField] private Animation _animationCamera;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    [Header("Components &|| Children")]
    [SerializeField] private Animation _animationLever;

    private bool _isActive = false;


    public void ActivateCameraCutscene(EllaExploring exploringScript)
    {
        float animationLength = _animationCamera.clip.length;

        // freeze gameplay
        exploringScript.ToggleMoveInput(animationLength);
     
        // logic to swap current virtual camera, and return to normal afterwards
        ServiceLocator.Instance.GetService<VirtualCameraManagerExploring>().SwapCutsceneCamera(_virtualCamera, animationLength);

        // play cutscene animation
        _animationCamera.Play();

        // activate animation lever
        _animationLever.Play();

        // start routine for JumpPad
        StartCoroutine(EnableJumpPad());

        _isActive = true;
    }


    private IEnumerator EnableJumpPad()
    {
        // wait for the amount of time in the animation - 2f (still see jumpPad activate within cutscene)
        yield return new WaitForSeconds(_animationCamera.clip.length - 1.5f);

        // activates JumpPad
        _jumpPad.ActivateJumpPad();
    }
    private IEnumerator TogglePlayerInput(EllaExploring exploringScript)
    {
        exploringScript.BlockMove = true;

        yield return new WaitForSeconds(_animationCamera.clip.length);

        exploringScript.BlockMove = false;
    }

}
