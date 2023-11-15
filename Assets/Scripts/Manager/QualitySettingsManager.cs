using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameObjectArray
{
    [SerializeField]
    private GameObject[] _qualityLevelFeatures;

    // Public method to access the GameObjects
    public GameObject[] QualityLevelFeatures => _qualityLevelFeatures;
}


[DefaultExecutionOrder(-999)]
public class QualitySettingsManager : Service
{
    [System.Flags]
    public enum QualityRank
    {
        Low = 1,
        Medium = 2,
        High = 4,
    }

    public GameObjectArray[] qualityLevels = new GameObjectArray[Enum.GetValues(typeof(QualityRank)).Length];
    public QualityObject[] _qualityObjects;

    protected override void Awake()
    {
        base.Awake();

        List<QualityObject> qualityObjectList = new List<QualityObject>();

        // Get all root objects in the scene
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            // Get QualityObject components in children, including inactive ones
            QualityObject[] childQualityObjects = obj.GetComponentsInChildren<QualityObject>(true);
            qualityObjectList.AddRange(childQualityObjects);
        }

        _qualityObjects = qualityObjectList.ToArray();

        SetQualityLevelFeatures((int)GameManager.Data.QualityLevel);
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

        if (Enum.IsDefined(typeof(QualityRank), qualityLevel))
        {
            var currentQualityLevel = (QualityRank)qualityLevel;

            for (int i = 0; i < _qualityObjects.Length; i++)
            {
                var qualityObject = _qualityObjects[i];

                bool turnOnObject = (qualityObject.QualityRank & currentQualityLevel) == currentQualityLevel;
                qualityObject.EnableObject(turnOnObject);
            }
        }
    }
}
