using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothPathToBeAlligned : MonoBehaviour
{
    public GameObject VisualPlayerBounds;

    public void ShowVisuals(bool show = true)
    {
        if (VisualPlayerBounds != null)
        {
            if (show == true)
            {
                VisualPlayerBounds.SetActive(true);
            }
            else
            {
                VisualPlayerBounds.SetActive(false);
            }
        }
    }
}
