using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimator : MonoBehaviour
{
    public Animator MyAnimator;

    [Header("Levels")]
    public List<MenuLevel> MenuLevels = new List<MenuLevel>();
}
