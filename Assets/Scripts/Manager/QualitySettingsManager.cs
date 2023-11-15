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


        Initialize();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Initialize();
    }

    private void Initialize()
    {
        _qualityObjects = null;
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

        SetQualityLevelFeatures(GameManager.Data.QualityRank);
    }

    public void ToggleQualityLevel()
    {
        int currentQualityLevelIndex = QualitySettings.GetQualityLevel();
        currentQualityLevelIndex = (currentQualityLevelIndex + 1) % QualitySettings.names.Length;
        var currentQualityLevelToRank = IndexToQualityRank(currentQualityLevelIndex);

        SetQualityLevelFeatures(currentQualityLevelToRank);

        //if (QualitySettings.names.Length - 1 != QualitySettings.GetQualityLevel())
        //{
        //    QualitySettings.IncreaseLevel();
        //}
        //else
        //{
        //    QualitySettings.SetQualityLevel(0);
        //}

    }

    private void SetQualityLevelFeatures(QualityRank qualityRank)
    {
        int qualityLevelIndex = QualityRankToIndex(qualityRank);
        QualitySettings.SetQualityLevel(qualityLevelIndex);


        //for (int i = 0; i < qualityLevels.Length; i++)
        //{
        //    if (i != qualityRank)
        //    {
        //        foreach (GameObject go in qualityLevels[i].QualityLevelFeatures)
        //        {
        //            if (go != null)
        //            {
        //                go.SetActive(false);
        //            }
        //        }
        //    }
        //}

        //for (int i = 0; i < qualityLevels.Length; i++)
        //{
        //    if (i == qualityRank)
        //    {
        //        foreach (GameObject go in qualityLevels[i].QualityLevelFeatures)
        //        {
        //            if (go != null)
        //            {
        //                go.SetActive(true);
        //            }
        //        }
        //    }
        //}

        if (Enum.IsDefined(typeof(QualityRank), qualityRank))
        {
            var currentQualityLevel = (QualityRank)qualityRank;

            for (int i = 0; i < _qualityObjects.Length; i++)
            {
                var qualityObject = _qualityObjects[i];

                bool turnOnObject = (qualityObject.QualityRank & currentQualityLevel) == currentQualityLevel;
                qualityObject.EnableObject(turnOnObject);
            }
        }
    }

    private int QualityRankToIndex(QualityRank rank)
    {
        // Handle the case where rank is None or not defined
        if (!Enum.IsDefined(typeof(QualityRank), rank))
        {
            return 0; 
        }

        int index = 0;
        int value = 1;

        while (value < (int)rank)
        {
            index++;
            value <<= 1; // Equivalent to value *= 2
        }

        return index;
    }

    private QualityRank IndexToQualityRank(int index)
    {
        // Assuming the order in QualitySettings.names matches the order of your QualityRank enum
        if (Enum.IsDefined(typeof(QualityRank), 1 << index))
        {
            return (QualityRank)(1 << index);
        }
        return QualityRank.Low;
    }
}
