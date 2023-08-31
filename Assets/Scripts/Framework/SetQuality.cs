using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetQuality : MonoBehaviour
{
    public void SetQualityHigh()
    {
        QualitySettings.SetQualityLevel(1);
    }
}
