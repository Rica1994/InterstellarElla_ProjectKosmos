using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
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
    public RenderTexture _qualityTexture;
    public Camera _gameplayCamera;

    private int _currentQualityIndex = 0;
    private Vector2 _initialScreensize = Vector2.zero;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        Initialize();
    }
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Initialize();
    }

    public void Initialize()
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

        AdjustCameraAspectRatios();

        _initialScreensize.x = Screen.width;
        _initialScreensize.y = Screen.height;
        SetQualityLevelFeatures(GameManager.Data.QualityRank);
    }

    private void OnQualityObjectDestroyed(QualityObject qualityObject)
    {
        qualityObject.DestroyEvent -= OnQualityObjectDestroyed;
        _qualityObjects = _qualityObjects.Where(x => x != qualityObject).ToArray();
    }

    public void ToggleQualityLevel()
    {
        _currentQualityIndex = QualitySettings.GetQualityLevel();
        _currentQualityIndex = (_currentQualityIndex + 1) % QualitySettings.names.Length;
        var currentQualityLevelToRank = IndexToQualityRank(_currentQualityIndex);
        GameManager.Data.QualityRank = currentQualityLevelToRank;

        SetQualityLevelFeatures(currentQualityLevelToRank);
    }

    public void SetQualityLevelFeatures(QualityRank qualityRank)
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

        // Setting the resolution

        AdjustCameraAspectRatios();

        float screenQualityFactor = 1.0f;

        switch (qualityRank)
        {
            case QualityRank.Low:
                screenQualityFactor = 0.33f;
                break;
            case QualityRank.Medium:
                screenQualityFactor = 0.66f;
                break;
        }

        int newWidth = (int)(_initialScreensize.x * screenQualityFactor);
        int newHeight = (int)(_initialScreensize.y * screenQualityFactor);

        _qualityTexture.Release();
        _qualityTexture.width = newWidth;
        _qualityTexture.height = newHeight;
        _qualityTexture.Create();
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

    public static QualityRank GetQualityRankFromSettings()
    {
        int qualityLevel = QualitySettings.GetQualityLevel();

        switch (qualityLevel)
        {
            case 0: // Assuming '0' is Low in Unity's settings
                return QualityRank.Low;
            case 1: // Assuming '1' is Medium in Unity's settings
                return QualityRank.Medium;
            case 2: // Assuming '2' is High in Unity's settings
                return QualityRank.High;
            default:
                return QualityRank.Low; // Default case, can be adjusted as needed
        }
    }

    private void AdjustCameraAspectRatios()
    {
        Debug.Log("ScreenWidth: " + Screen.width);
        Debug.Log("ScreenHeight: " + Screen.height);

        _initialScreensize.x = Screen.width;
        _initialScreensize.y = Screen.height;

        Camera[] cameras = FindObjectsOfType<Camera>();
        float targetAspect = (float)Screen.width / Screen.height;

        foreach (Camera cam in cameras)
        {
            cam.aspect = targetAspect;
        }
    }
}
