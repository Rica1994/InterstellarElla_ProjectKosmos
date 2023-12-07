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



    public void PlayPlanetSelectAnimation(bool reverse)
    {
        if (reverse)
        {
            CameraAnimator.SetTrigger("PlanetDeselected");
        }
        else
        {
            CameraAnimator.SetTrigger("LevelSelected");
        }
    }

    public void ZoomPlanet()
    {
        CameraAnimator.SetTrigger("ZoomPlanet");
    }

}
