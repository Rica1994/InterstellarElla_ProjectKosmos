using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimator : MonoBehaviour
{
    public Animator MyAnimator;
    public Animator CameraAnimator;
    [SerializeField]
    private Animator _levelCenterAnimator;

    [Header("Levels")]
    public List<MenuLevel> MenuLevels = new List<MenuLevel>();

    // Animation trigger strings
    private string PLANET_DESELECT = "PlanetDeselected";
    private string PLANET_SELECTED = "LevelSelected";
    private string ZOOM_PLANET = "ZoomPlanet";
    private string SHOW_PLANETS = "ShowPlanets";

    public void PlayPlanetSelectAnimation(bool reverse)
    {
        if (reverse)
        {
            CameraAnimator.SetTrigger(PLANET_DESELECT);
        }
        else
        {
            CameraAnimator.SetTrigger(PLANET_SELECTED);
        }
    }

    public void ZoomPlanet()
    {
        CameraAnimator.SetTrigger(ZOOM_PLANET);
    }

    public void ShowPlanets()
    {
        _levelCenterAnimator.SetTrigger(SHOW_PLANETS);
    }
}