using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        for (int i = 0; i < _qualityObjects.Length; i++) _qualityObjects[i].DestroyEvent -= OnQualityObjectDestroyed;
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

        qualityObjectList.ForEach(x => x.DestroyEvent += OnQualityObjectDestroyed);
        _qualityObjects = qualityObjectList.ToArray();
        

        SetQualityLevelFeatures(GameManager.Data.QualityRank);
    }

    private void OnQualityObjectDestroyed(QualityObject qualityObject)
    {
        qualityObject.DestroyEvent -= OnQualityObjectDestroyed;
        _qualityObjects = _qualityObjects.Where(x => x != qualityObject).ToArray();
    }

    public void ToggleQualityLevel()
    {
        int currentQualityLevelIndex = QualitySettings.GetQualityLevel();
        currentQualityLevelIndex = (currentQualityLevelIndex + 1) % QualitySettings.names.Length;
        var currentQualityLevelToRank = IndexToQualityRank(currentQualityLevelIndex);

        SetQualityLevelFeatures(currentQualityLevelToRank);
    }

    private void SetQualityLevelFeatures(QualityRank qualityRank)
    {
        int qualityLevelIndex = QualityRankToIndex(qualityRank);
        QualitySettings.SetQualityLevel(qualityLevelIndex);

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


    //   // Setting the resolution
    //
    //   int width = 800;
    //   int height = 600;
    //   int depth = 24;
    //
    //   switch (qualityRank)
    //   {
    //       case QualityRank.Low:
    //           width = 800;
    //           height = 600;
    //           break;
    //       case QualityRank.Medium:
    //           width = 1280;
    //           height = 720;
    //           break;
    //       case QualityRank.High:
    //           width = 1920;
    //           height = 1080;
    //           break;
    //   }
    //
    //   RenderTexture newRenderTexture = new RenderTexture(width, height, depth);
    //   newRenderTexture.Create();
    //   Camera.main.targetTexture = newRenderTexture;
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
