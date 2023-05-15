using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimator : MonoBehaviour
{
    public Animator MyAnimator;

    public Animation CameraAnimation;

    [Header("Levels")]
    public List<MenuLevel> MenuLevels = new List<MenuLevel>();
}
