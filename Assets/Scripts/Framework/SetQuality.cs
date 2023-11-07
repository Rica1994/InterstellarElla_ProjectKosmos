using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetQuality : MonoBehaviour
{
    public void SetQualityHigh()
    {
        QualitySettings.SetQualityLevel(1);
    }

    public void ToggleQuality()
    {
        // Toggle the quality settings to either low (0) or high (1)
        if (QualitySettings.GetQualityLevel() > 0)
        {
            QualitySettings.SetQualityLevel(0);
        }
        else
        {
            QualitySettings.SetQualityLevel(1);
        }

        // Toggle the features based on the quality settings
        GetComponent<QualitySettingsManager>().ToggleFeatures();
    }
}
