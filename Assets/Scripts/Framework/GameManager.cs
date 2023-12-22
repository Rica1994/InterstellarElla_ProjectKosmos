using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityCore.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Service
{
    #region Events

    public delegate void GameEvent();
    public static event GameEvent CutsceneStartedEvent;
    public static event GameEvent CutsceneEndedEvent;

    #endregion

    #region StructsAndEnums

    public struct SaveData
    {
        public PlanetCompletionValues PlanetCompletionValues;
        public int CurrentScore;
        public int LastPlanet;
        public QualitySettingsManager.QualityRank QualityRank;
        public BuildType BuildType;
        public PlanetLastScenes PlanetLastScenes;
        public PlanetLastScores PlanetLastScores;

        public override string ToString()
        {
            return PlanetCompletionValues.ToString() + Helpers.FormatPercentageToString(CurrentScore) + LastPlanet.ToString()
                + ((int)QualityRank).ToString() + ((int)BuildType).ToString() + PlanetLastScenes.ToString() + PlanetLastScores.ToString();
        }
    }

    public struct PlanetCompletionValues
    {
        public float MarsCompletion;
        public float VenusCompletion;
        public float SaturnCompletion;
        public float PlutoCompletion;
        public float MercuryCompletion;

        public override string ToString()
        {
            return Helpers.FormatPercentageToString(MarsCompletion) +
                   Helpers.FormatPercentageToString(VenusCompletion) +
                   Helpers.FormatPercentageToString(SaturnCompletion) +
                   Helpers.FormatPercentageToString(PlutoCompletion) +
                   Helpers.FormatPercentageToString(MercuryCompletion);
        }
    }

    public struct PlanetLastScenes
    {
        public int MarsLastScene;
        public int PlutoLastScene;
        public int VenusLastScene;
        public int SaturnLastScene;
        public int MercuryLastScene;

        public override string ToString()
        {
            return $"{MarsLastScene}{PlutoLastScene}{VenusLastScene}{SaturnLastScene}{MercuryLastScene}";
        }
    }

    public struct PlanetLastScores
    {
        public int MarsLastScore;
        public int PlutoLastScore;
        public int VenusLastScore;
        public int SaturnLastScore;
        public int MercuryLastScore;

        public override string ToString()
        {
            return MarsLastScore.ToString("D3") + PlutoLastScore.ToString("D3") + VenusLastScore.ToString("D3") + SaturnLastScore.ToString("D3") + MercuryLastScore.ToString("D3");
        }
    }

    private enum GameMode
    {
        SpeederSpace,
        SpeederGround,
        BoostBoots,
        RemoteCar,
    }

    public enum Planet
    {
        Mars = 1,
        Pluto = 2,
        Venus = 3,
        Saturn = 4,
        Mercury = 5,
        None = 0
    }
    public enum BuildType
    {
        Client = 0,
        Debug = 1
    }

    public static int MARS_DATA_NEEDED = 475;
    public static int PLUTO_DATA_NEEDED = 150;
    public static int VENUS_DATA_NEEDED = 350;
    public static int SATURN_DATA_NEEDED = 475;
    public static int MERCURY_DATA_NEEDED = 350;

    #endregion


    [SerializeField]
    private bool _simulateMobile = false;

    [SerializeField]
    private BuildType _targetBuildType = BuildType.Debug;
    public BuildType TargetBuildType => _targetBuildType;

    private PlayerController _playerController;

    private bool _isMobile = false;
    public bool IsMobileWebGl => _isMobile;

    public LayerMask PlayerLayermask;

    // public PlanetCompletionValues PlanetCompletions;
    // public int CurrentScore = 0;
    // public Planet CurrentPlanet = Planet.None;

    public static SaveData Data;
    public PlayerController PlayerController => _playerController;

    public static bool IsShittyDevice = false;

    private static bool _isInCutScene = false;
    private static bool _isGameplayPaused = false;

    public static bool IsInCutscene
    {
        get => _isInCutScene;
        set
        {
            var wasNotInCutScene = _isInCutScene == false;
            _isInCutScene = value;
            //   var audioController = ServiceLocator.Instance.GetService<AudioController>();
            //   if (audioController != null)
            //   {
            //       audioController.MixerAdjustment(value ? MixerType.MixerFXMuted : MixerType.MixerNormal);
            //   }

            if (wasNotInCutScene && _isInCutScene)
            {
                CutsceneStartedEvent?.Invoke();
            }
            else if (wasNotInCutScene == false && _isInCutScene == false)
            {
                CutsceneEndedEvent?.Invoke();
            }
        }
    }

    public static bool IsGamePlayPaused
    {
        get => _isGameplayPaused;
        set
        {
            ServiceLocator.Instance.GetService<SoundManager>().PauseInSceneAudioSources(value);
            Time.timeScale = value ? 0 : 1;
            _isGameplayPaused = value;
        }
    }

    private bool _isInitalized = false;

#if !UNITY_EDITOR && UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern bool IsMobile();
#endif

    protected override void OnEnable()
    {
        base.OnEnable();

        _playerController = FindObjectOfType<PlayerController>();
        if (_playerController == null)
        {
            Debug.LogWarning("No player controller found in this scene");
        }

        _isMobile = _simulateMobile;
#if !UNITY_EDITOR && UNITY_WEBGL
        _isMobile = IsMobile();
#endif
    }

    private void Initialize()
    {
        var hud = ServiceLocator.Instance.GetService<HudManager>();
        var pickupManager = ServiceLocator.Instance.GetService<PickUpManager>();
        var currentPlanet = GetCurrentPlanet();
        var lastPlanetScore = Data.PlanetLastScores.MarsLastScore;

        switch (currentPlanet)
        {
            case Planet.Mars:
                lastPlanetScore = Data.PlanetLastScores.MarsLastScore;
                break;
            case Planet.Pluto:
                lastPlanetScore = Data.PlanetLastScores.PlutoLastScore;
                break;
            case Planet.Venus:
                lastPlanetScore = Data.PlanetLastScores.VenusLastScore;
                break;
            case Planet.Saturn:
                lastPlanetScore = Data.PlanetLastScores.SaturnLastScore;
                break;
            case Planet.Mercury:
                lastPlanetScore = Data.PlanetLastScores.MercuryLastScore;
                break;
            case Planet.None:
                Data.CurrentScore = 0;
                hud.EnableHUD(false);
                break;
        }

        if (currentPlanet != Planet.None)
        {
            hud.EnableHUD(true);
            hud.Initialize(currentPlanet);
            pickupManager.PickUpsPickedUp = lastPlanetScore;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (_isDestroyed) return;

        PlayerController.PlayerControllerEnabledEvent += OnPlayerControllerEnabled;

#if UNITY_EDITOR
        Data = new SaveData();
        Data.QualityRank = QualitySettingsManager.GetQualityRankFromSettings();

#elif !UNITY_EDITOR && UNITY_WEBGL

        if (PlayerPrefs.HasKey("SaveData"))
        {
            Debug.Log("Found data: " + PlayerPrefs.GetString("SaveData"));

            // adding this string below so it gets the same format as an saveDataString
            var saveDataString = PlayerPrefs.GetString("SaveData");
            ParseData(saveDataString);
        }

        _targetBuildType = Data.BuildType;


        // Check if we are in a level or quiz
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Contains("Level") || sceneName.Contains("Quiz"))
        {
            // Get the current planet
            Planet currentPlanet = GetCurrentPlanet();

            // If we're in a quiz, the current level equals the last planet
            if (sceneName.Contains("Quiz"))
            {
                currentPlanet = (Planet)Data.LastPlanet;
            }

            // Get the current scene index
            int currentSceneIndex = ParsePlanetSceneIndex(sceneName, currentPlanet);

            // Overwrite planet last scene index
            switch (currentPlanet)
            {
                case Planet.Mars:
                    Data.PlanetLastScenes.MarsLastScene = currentSceneIndex;
                    break;
                case Planet.Pluto:
                    Data.PlanetLastScenes.PlutoLastScene = currentSceneIndex;
                    break;
                case Planet.Venus:
                    Data.PlanetLastScenes.VenusLastScene = currentSceneIndex;
                    break;
                case Planet.Saturn:
                    Data.PlanetLastScenes.SaturnLastScene = currentSceneIndex;
                    break;
                case Planet.Mercury:
                    Data.PlanetLastScenes.MercuryLastScene = currentSceneIndex;
                    break;
                default:
                    Debug.LogError("Currently not in a planet");
                    break;
            }

            // save new data
            PlayerPrefs.SetString("SaveData", Data.ToString());
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogError("Currently not in a planet");
        }
#endif

    }

    private void Start()
    {
        Initialize();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene arg0)
    {
        Debug.Log($"{arg0.name} scene got unloaded");

        // Save everytime we leave the scene.
        PlayerPrefs.SetString("SaveData", Data.ToString());
        PlayerPrefs.Save();
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine(Helpers.DoAfterFrame(Initialize));
    }

    private void OnPlayerControllerEnabled(PlayerController controller)
    {
        _playerController = controller;
    }

    private void ParseData(string url)
    {
        string planetCompletionsCompiled = url;

        // Step 1: Extract each planet's completion
        int mars = int.Parse(planetCompletionsCompiled.Substring(0, 3));
        int venus = int.Parse(planetCompletionsCompiled.Substring(3, 3));
        int saturn = int.Parse(planetCompletionsCompiled.Substring(6, 3));
        int pluto = int.Parse(planetCompletionsCompiled.Substring(9, 3));
        int mercury = int.Parse(planetCompletionsCompiled.Substring(12, 3));

        // Step 2: Extract current score
        var currentScore = 0;
        if (GetCurrentPlanet() != Planet.None)
        {
            currentScore = int.Parse(planetCompletionsCompiled.Substring(15, 3));
        }

        // Step 3: Extract current level
        var lastPlanet = Planet.None;

        int planetValue;
        if (int.TryParse(planetCompletionsCompiled.Substring(18, 1), out planetValue))
        {
            if (Enum.IsDefined(typeof(Planet), planetValue))
            {
                lastPlanet = (Planet)planetValue;
            }
        }

        // Step 4: Extract quality rank
        var qualityLevel = QualitySettingsManager.QualityRank.Low;
        int qualityValue = 0;
        if (int.TryParse(planetCompletionsCompiled.Substring(19, 1), out qualityValue))
        {
            if (Enum.IsDefined(typeof(QualitySettingsManager.QualityRank), qualityValue))
            {
                qualityLevel = (QualitySettingsManager.QualityRank)qualityValue;
            }
        }

        // Step 5: Extract build type
        var buildType = BuildType.Debug;
        int buildTypeValue = 0;
        if (int.TryParse(planetCompletionsCompiled.Substring(20, 1), out buildTypeValue))
        {
            if (Enum.IsDefined(typeof(BuildType), buildTypeValue))
            {
                buildType = (BuildType)buildTypeValue;
            }
        }

        // Step 6: Extract each planet's last scene
        int marsLastScene = int.Parse(planetCompletionsCompiled.Substring(21, 1));
        int plutoLastScene = int.Parse(planetCompletionsCompiled.Substring(22, 1));
        int venusLastScene = int.Parse(planetCompletionsCompiled.Substring(23, 1));
        int saturnLastScene = int.Parse(planetCompletionsCompiled.Substring(24, 1));
        int mercuryLastScene = int.Parse(planetCompletionsCompiled.Substring(25, 1));

        // Step 7: Assign planet completion values to the struct
        PlanetCompletionValues values = new PlanetCompletionValues
        {
            MarsCompletion = mars,
            VenusCompletion = venus,
            PlutoCompletion = pluto,
            SaturnCompletion = saturn,
            MercuryCompletion = mercury
        };


        // Step 8: Assign planet last scenes to the struct
        PlanetLastScenes lastScenes = new PlanetLastScenes
        {
            MarsLastScene = marsLastScene,
            PlutoLastScene = plutoLastScene,
            VenusLastScene = venusLastScene,
            SaturnLastScene = saturnLastScene,
            MercuryLastScene = mercuryLastScene
        };

        // Step 9: Extract each planet's last scores
        int marsLastScore = int.Parse(planetCompletionsCompiled.Substring(26, 3));
        int plutoLastScore = int.Parse(planetCompletionsCompiled.Substring(29, 3));
        int venusLastScore = int.Parse(planetCompletionsCompiled.Substring(32, 3));
        int saturnLastScore = int.Parse(planetCompletionsCompiled.Substring(35, 3));
        int mercuryLastScore = int.Parse(planetCompletionsCompiled.Substring(38, 3));

        // Step 10: Assign planet last scenes to the struct
        PlanetLastScores lastScores = new PlanetLastScores
        {
            MarsLastScore = marsLastScore,
            PlutoLastScore = plutoLastScore,
            VenusLastScore = venusLastScore,
            SaturnLastScore = saturnLastScore,
            MercuryLastScore = mercuryLastScore
        };


        // Step 11: Parse to the data variable
        var data = new SaveData();
        data.LastPlanet = (int)lastPlanet;
        data.CurrentScore += currentScore;
        data.PlanetCompletionValues = values;
        data.QualityRank = qualityLevel;
        data.BuildType = buildType;
        data.PlanetLastScenes = lastScenes;
        data.PlanetLastScores = lastScores;

        Data = data;
        Debug.Log("Parsed Data: " + Data.ToString());
    }

    public void EndLevel(bool isLastLevel)
    {
        var pickUpManager = ServiceLocator.Instance.GetService<PickUpManager>();
        var pickUpsCollected = pickUpManager.PickUpsPickedUp;
        Debug.Log($"You collected {pickUpsCollected} / {pickUpManager.PickUps.Count} this level.");

        // converting the current score to a percentage number (between 0 and 100)
        Data.CurrentScore = ConvertPlanetScoreToPercentage(pickUpsCollected);
        var currentPlanet = GetCurrentPlanet();

        // Updating the last score of the current planet played
        switch (currentPlanet)
        {
            case Planet.Mars:
                Data.PlanetLastScores.MarsLastScore = Data.CurrentScore;
                break;
            case Planet.Pluto:
                Data.PlanetLastScores.PlutoLastScore = Data.CurrentScore;
                break;
            case Planet.Venus:
                Data.PlanetLastScores.VenusLastScore = Data.CurrentScore;
                break;
            case Planet.Saturn:
                Data.PlanetLastScores.SaturnLastScore = Data.CurrentScore;
                break;
            case Planet.Mercury:
                Data.PlanetLastScores.MercuryLastScore = Data.CurrentScore;
                break;
        }


        if (isLastLevel)
        {
            Debug.Log("Game Ended");

            Data.LastPlanet = (int)currentPlanet;
            Debug.Log("End Game called, with current planet: " + currentPlanet.ToString() + " and current score: " + Data.CurrentScore);
            switch (currentPlanet)
            {
                case Planet.Mars:
                    if (Data.CurrentScore > Data.PlanetCompletionValues.MarsCompletion) Data.PlanetCompletionValues.MarsCompletion = Data.CurrentScore;
                    Data.PlanetLastScores.MarsLastScore = 0;
                    break;
                case Planet.Venus:
                    if (Data.CurrentScore > Data.PlanetCompletionValues.VenusCompletion) Data.PlanetCompletionValues.VenusCompletion = Data.CurrentScore;
                    Data.PlanetLastScores.VenusLastScore = 0;
                    break;
                case Planet.Saturn:
                    if (Data.CurrentScore > Data.PlanetCompletionValues.SaturnCompletion) Data.PlanetCompletionValues.SaturnCompletion = Data.CurrentScore;
                    Data.PlanetLastScores.SaturnLastScore = 0;
                    break;
                case Planet.Pluto:
                    if (Data.CurrentScore > Data.PlanetCompletionValues.PlutoCompletion) Data.PlanetCompletionValues.PlutoCompletion = Data.CurrentScore;
                    Data.PlanetLastScores.PlutoLastScore = 0;
                    break;
                case Planet.Mercury:
                    if (Data.CurrentScore > Data.PlanetCompletionValues.MercuryCompletion) Data.PlanetCompletionValues.MercuryCompletion = Data.CurrentScore;
                    Data.PlanetLastScores.MercuryLastScore = 0;
                    break;
            }

            Data.CurrentScore = 0;
        }

        // save new data
        PlayerPrefs.SetString("SaveData", Data.ToString());
        PlayerPrefs.Save();
    }

    public int ConvertPlanetScoreToPercentage(float score)
    {
        float scoreRecalculated = 0.0f;
        switch (GetCurrentPlanet())
        {
            case Planet.Venus:
                scoreRecalculated = score / VENUS_DATA_NEEDED;
                scoreRecalculated = Mathf.Clamp(scoreRecalculated, 0.0f, 1.0f) * 100;
                break;
            case Planet.Saturn:
                scoreRecalculated = score / SATURN_DATA_NEEDED;
                scoreRecalculated = Mathf.Clamp(scoreRecalculated, 0.0f, 1.0f) * 100;
                break;
            case Planet.Mars:
                scoreRecalculated = score / MARS_DATA_NEEDED;
                scoreRecalculated = Mathf.Clamp(scoreRecalculated, 0.0f, 1.0f) * 100;
                break;
            case Planet.Pluto:
                scoreRecalculated = score / PLUTO_DATA_NEEDED;
                scoreRecalculated = Mathf.Clamp(scoreRecalculated, 0.0f, 1.0f) * 100;
                break;
            case Planet.Mercury:
                scoreRecalculated = score / MERCURY_DATA_NEEDED;
                scoreRecalculated = Mathf.Clamp(scoreRecalculated, 0.0f, 1.0f) * 100;
                break;
            default:
                Debug.LogError("This is being called from a wrong scene, this should only be called from the end of a level");
                scoreRecalculated = 0;
                break;
        }

        return (int)scoreRecalculated;
    }

    public Planet GetCurrentPlanet()
    {
        var sceneTypeString = SceneManager.GetActiveScene().name;

        if (sceneTypeString.Contains("Level_1") || sceneTypeString.Contains("Mars"))
            return Planet.Mars;
        else if (sceneTypeString.Contains("Level_2"))
            return Planet.Pluto;
        else if (sceneTypeString.Contains("Level_3"))
            return Planet.Venus;
        else if (sceneTypeString.Contains("Level_4"))
            return Planet.Saturn;
        else if (sceneTypeString.Contains("Level_5"))
            return Planet.Mercury;
        return Planet.None;
    }

    public void RespawnPlayer(GameObject player, GameObject checkpoint, bool addFlash = false)
    {
        player.transform.position = checkpoint.transform.position;
        if (addFlash)
        {
            StartCoroutine(Helpers.Flicker(player, 4.0f, 5));
        }
    }

    private void Update()
    {
        if (_playerController != null)
        {
            _playerController.UpdateController();
        }
    }
    private void FixedUpdate()
    {
        if (_playerController != null)
        {
            _playerController.FixedUpdateController();
        }
    }

    public void SetPlayerController(PlayerController playerController = null)
    {
        _playerController = playerController;
    }

    private int ParsePlanetSceneIndex(string sceneName, Planet planet)
    {
        if (sceneName.Contains("Intro")) return 0;

        switch (planet)
        {
            case Planet.Mars:
                if (sceneName.Contains("_1_1_Work")) return 1;
                else if (sceneName.Contains("Outro")) return 2;
                else if (sceneName.Contains("Quiz")) return 3;
                break;
            case Planet.Pluto:
                if (sceneName.Contains("_2_0_Work")) return 1;
                else if (sceneName.Contains("_2_1_Work")) return 2;
                else if (sceneName.Contains("_2_2_Work")) return 3;
                else if (sceneName.Contains("Outro")) return 4;
                else if (sceneName.Contains("Quiz")) return 5;
                break;
            case Planet.Venus:
                if (sceneName.Contains("_3_0_Work")) return 1;
                else if (sceneName.Contains("_3_1_Work")) return 2;
                else if (sceneName.Contains("_3_2_Work")) return 3;
                else if (sceneName.Contains("_3_3_Work")) return 4;
                else if (sceneName.Contains("_3_4_Work")) return 5;
                else if (sceneName.Contains("_3_5_Work")) return 6;
                else if (sceneName.Contains("Outro")) return 7;
                else if (sceneName.Contains("Quiz")) return 8;
                break;
            case Planet.Saturn:
                if (sceneName.Contains("_4_0_Work")) return 1;
                else if (sceneName.Contains("_4_1_Work")) return 2;
                else if (sceneName.Contains("_4_2_Work")) return 3;
                else if (sceneName.Contains("_4_3_Work")) return 4;
                else if (sceneName.Contains("_4_4_Work")) return 5;
                else if (sceneName.Contains("_4_5_Work")) return 6;
                else if (sceneName.Contains("Outro")) return 7;
                else if (sceneName.Contains("Quiz")) return 8;
                break;
            case Planet.Mercury:
                if (sceneName.Contains("_5_0_Work")) return 1;
                else if (sceneName.Contains("_5_1_Work")) return 2;
                else if (sceneName.Contains("_5_2_Work")) return 3;
                else if (sceneName.Contains("_5_3_Work")) return 4;
                else if (sceneName.Contains("Outro")) return 5;
                else if (sceneName.Contains("Quiz")) return 6;
                break;
        }

        Debug.LogError("Could not parse follwing scene: " + sceneName);
        return -1;
    }
}