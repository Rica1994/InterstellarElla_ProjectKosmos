using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameObjectArray
{
    [SerializeField]
    private GameObject[] _qualityLevelFeatures;

    // Public method to access the GameObjects
    public GameObject[] QualityLevelFeatures => _qualityLevelFeatures;
}


public class QualitySettingsManager : Service
{
    public GameObjectArray[] qualityLevels = new GameObjectArray[Enum.GetValues(typeof(QualityLevel)).Length];

    private void Start()
    {
        SetQualityLevelFeatures(QualitySettings.GetQualityLevel());
    }

    public void ToggleQualityLevel()
    {
        if (QualitySettings.names.Length - 1 != QualitySettings.GetQualityLevel())
        {
            QualitySettings.IncreaseLevel();
        }
        else
        {
            QualitySettings.SetQualityLevel(0);
        }

        SetQualityLevelFeatures(QualitySettings.GetQualityLevel());
    }

    private void SetQualityLevelFeatures(int qualityLevel)
    {
        for (int i = 0; i < qualityLevels.Length; i++)
        {
            if (i != qualityLevel)
            {
                foreach (GameObject go in qualityLevels[i].QualityLevelFeatures)
                {
                    if (go != null)
                    {
                        go.SetActive(false);
                    }
                }
            }
        }

        for (int i = 0; i < qualityLevels.Length; i++)
        {
            if (i == qualityLevel)
            {
                foreach (GameObject go in qualityLevels[i].QualityLevelFeatures)
                {
                    if (go != null)
                    {
                        go.SetActive(true);
                    }
                }
            }
        }
    }
}
