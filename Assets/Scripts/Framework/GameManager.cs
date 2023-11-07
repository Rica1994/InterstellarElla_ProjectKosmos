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
        //   public bool IsShittyDevice;

        public override string ToString()
        {
            return PlanetCompletionValues.ToString() + Helpers.FormatPercentageToString(CurrentScore) + LastPlanet.ToString() /*+ (IsShittyDevice ? "1" : "0")*/;
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
        Debug,
        Client
    }

    public static int MARS_DATA_NEEDED = 250;
    public static int PLUTO_DATA_NEEDED = 250;
    public static int VENUS_DATA_NEEDED = 250;
    public static int SATURN_DATA_NEEDED = 250;
    public static int MERCURY_DATA_NEEDED = 250;

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
            var wasNotInCutScene = _isInCutScene;
            _isInCutScene = value;
            var audioController = ServiceLocator.Instance.GetService<AudioController>();
            if (audioController != null)
            {
                audioController.MixerAdjustment(value ? MixerType.MixerFXMuted : MixerType.MixerNormal);
            }

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
            ServiceLocator.Instance.GetService<SoundtrackManager>().PauseInSceneAudioSources(value);
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

        string url = Application.absoluteURL;
        if (url.IndexOf("?data=") != -1)
        {
            ParseData(url);
        }
        else
        {
            Debug.Log("?data= not found in URL");
        }
#endif
    }

    private void Initialize()
    {
        var hud = ServiceLocator.Instance.GetService<HudManager>();
        var pickupManager = ServiceLocator.Instance.GetService<PickUpManager>();
        var currentPlanet = GetCurrentPlanet();

        switch (currentPlanet)
        {
            case Planet.Mars:
            case Planet.Pluto:
            case Planet.Venus:
            case Planet.Saturn:
            case Planet.Mercury:
                hud.EnableHUD(true);
                hud.Initialize(currentPlanet);
                pickupManager.PickUpsPickedUp = Data.CurrentScore;
                break;
            case Planet.None:
                hud.EnableHUD(false);
                break;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (_isDestroyed) return;

        PlayerController.PlayerControllerEnabledEvent += OnPlayerControllerEnabled;

#if UNITY_EDITOR
        Data = new SaveData();
        //ParseData(Data.ToString());
#elif !UNITY_EDITOR && UNITY_WEBGL

        string url = Application.absoluteURL;
        if (url.IndexOf("?data=") != -1)
        {
            ParseData(url);
        }
        else
        {
            Debug.Log("?data= not found in URL");
        }
#endif
    }   

    private void Start()
    {
        Initialize();
        SceneManager.sceneLoaded += OnSceneLoaded;
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

        // Step 2: Unescape the URL
#if UNITY_WEBGL && !UNITY_EDITOR
        // Step 1: Parse the URL for the data parameter
        int dataIndex = url.IndexOf("?data=") + 6; // 6 is the length of "?data="
        string encodedData = url.Substring(dataIndex);
        planetCompletionsCompiled = UnityEngine.Networking.UnityWebRequest.UnEscapeURL(encodedData);
#endif
        // Step 3: Extract each planet's completion
        int mars = int.Parse(planetCompletionsCompiled.Substring(0, 3));
        int venus = int.Parse(planetCompletionsCompiled.Substring(3, 3));
        int saturn = int.Parse(planetCompletionsCompiled.Substring(6, 3));
        int pluto = int.Parse(planetCompletionsCompiled.Substring(9, 3));
        int mercury = int.Parse(planetCompletionsCompiled.Substring(12, 3));

        // Step 4: Extract current score
        var currentScore = int.Parse(planetCompletionsCompiled.Substring(15, 3));

        // Step 5: Extract current level
        var lastPlanet = Planet.None;

        int planetValue;
        if (int.TryParse(planetCompletionsCompiled.Substring(18, 1), out planetValue))
        {
            if (Enum.IsDefined(typeof(Planet), planetValue))
            {
                lastPlanet = (Planet)planetValue;
            }
        }

        // int isShittyDevice = 0;
        //   int.TryParse(planetCompletionsCompiled.Substring(19,1), out isShittyDevice);

        // Step 6: Assign to the struct
        PlanetCompletionValues values = new PlanetCompletionValues
        {
            MarsCompletion = mars,
            VenusCompletion = venus,
            SaturnCompletion = saturn,
            PlutoCompletion = pluto,
            MercuryCompletion = mercury
        };

        var data = new SaveData();
        data.LastPlanet = (int)lastPlanet;
        data.CurrentScore += currentScore;
        data.PlanetCompletionValues = values;
        //   data.IsShittyDevice = (isShittyDevice == 1) ? true : false;

        Data = data;
    }

    public void EndLevel(bool isLastLevel)
    {
        var pickUpManager = ServiceLocator.Instance.GetService<PickUpManager>();
        var pickUpsCollected = pickUpManager.PickUpsPickedUp;
        Debug.Log($"You collected {pickUpsCollected} / {pickUpManager.PickUps.Count} this level.");

        // Add the collected pickups of the last level, to the current score.
        Data.CurrentScore = pickUpsCollected;

        if (isLastLevel)
        {
            Debug.Log("Game Ended");

            // converting the current score to a percentage number (between 0 and 100)
            Data.CurrentScore = ConvertPlanetScoreToPercentage(Data.CurrentScore);

            var currentPlanet = GetCurrentPlanet();
            Data.LastPlanet = (int)currentPlanet;
            Debug.Log("End Game called, with current planet: " + currentPlanet.ToString() + " and current score: " + Data.CurrentScore);
            switch (currentPlanet)
            {
                case Planet.Mars:
                    if (Data.CurrentScore > Data.PlanetCompletionValues.MarsCompletion) Data.PlanetCompletionValues.MarsCompletion = Data.CurrentScore;
                    break;
                case Planet.Venus:
                    if (Data.CurrentScore > Data.PlanetCompletionValues.VenusCompletion) Data.PlanetCompletionValues.VenusCompletion = Data.CurrentScore;
                    break;
                case Planet.Saturn:
                    if (Data.CurrentScore > Data.PlanetCompletionValues.SaturnCompletion) Data.PlanetCompletionValues.SaturnCompletion = Data.CurrentScore;
                    break;
                case Planet.Pluto:
                    if (Data.CurrentScore > Data.PlanetCompletionValues.PlutoCompletion) Data.PlanetCompletionValues.PlutoCompletion = Data.CurrentScore;
                    break;
                case Planet.Mercury:
                    if (Data.CurrentScore > Data.PlanetCompletionValues.MercuryCompletion) Data.PlanetCompletionValues.MercuryCompletion = Data.CurrentScore;
                    break;
            }

            Data.CurrentScore = 0;
        }
    }

    public int ConvertPlanetScoreToPercentage(float score)
    {
        float scoreRecalculated = 0.0f;
        switch (GetCurrentPlanet())
        {
            case Planet.Venus:
                scoreRecalculated = score / 250.0f;
                scoreRecalculated = Mathf.Clamp(scoreRecalculated, 0.0f, 1.0f) * 100;
                break;
            case Planet.Saturn:
                scoreRecalculated = score / 250.0f;
                scoreRecalculated = Mathf.Clamp(scoreRecalculated, 0.0f, 1.0f) * 100;
                break;
            case Planet.Mars:
                scoreRecalculated = score / 250.0f;
                scoreRecalculated = Mathf.Clamp(scoreRecalculated, 0.0f, 1.0f) * 100;
                break;
            case Planet.Pluto:
                scoreRecalculated = score / 250.0f;
                scoreRecalculated = Mathf.Clamp(scoreRecalculated, 0.0f, 1.0f) * 100;
                break;
            case Planet.Mercury:
                scoreRecalculated = score / 250.0f;
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
}
