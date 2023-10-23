using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimator : MonoBehaviour
{
    public Animator MyAnimator;
    public Animator CameraAnimator;

    public Animation CameraAnimation;

    [Header("Levels")]
    public List<MenuLevel> MenuLevels = new List<MenuLevel>();



    public void PlayCameraAnimation(string cameraAnimation, bool reverse)
    {
        CameraAnimator.SetTrigger("LevelSelected");
     //   CameraAnimation.clip = CameraAnimation.GetClip(cameraAnimation);
     //   
     //   if (reverse) CameraAnimation.Rewind(cameraAnimation);
     //   else CameraAnimation.Play();
    }

}
