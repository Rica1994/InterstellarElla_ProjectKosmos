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

    [Header("Swap Cameras I influence")]
    [SerializeField] private SwapCamera _swapCameraActiveBeforeLeverHit;
    [SerializeField] private SwapCamera _swapCameraActiveAfterLeverHit;


    private bool _isActive = false;


    private void Start()
    {
        SwapCurrentActiveCamera(false);
    }


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

        // disable/enable a specific camera if present
        SwapCurrentActiveCamera();

        _isActive = true;
    }



    private void SwapCurrentActiveCamera(bool hitLever = false)
    {
        if (_swapCameraActiveBeforeLeverHit != null)
        {
            _swapCameraActiveBeforeLeverHit.gameObject.SetActive(!hitLever);
        }
        
        if (_swapCameraActiveAfterLeverHit != null)
        {
            _swapCameraActiveAfterLeverHit.gameObject.SetActive(hitLever);
        }      
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
