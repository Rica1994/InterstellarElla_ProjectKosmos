using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Service
{
    [SerializeField]
    private bool _simulateMobile = false;
    
    private PlayerController _playerController;

    public struct PlanetCompletionValues
    {
        public float MarsCompletion;
        public float VenusCompletion;
        public float SaturnCompletion;
        public float PlutoCompletion;
        public float MercuryCompletion;

        public override string ToString()
        {
            return FormatPlanetCompletion(MarsCompletion) +
                   FormatPlanetCompletion(VenusCompletion) +
                   FormatPlanetCompletion(SaturnCompletion) +
                   FormatPlanetCompletion(PlutoCompletion) +
                   FormatPlanetCompletion(MercuryCompletion);
        }

        private static string FormatPlanetCompletion(float completionValue)
        {
            int percentage = (int)Math.Round(completionValue);
            return percentage.ToString("D3");
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
        None = -1
    }

    private bool _isMobile = false;
    public bool IsMobileWebGl => _isMobile;

    public LayerMask PlayerLayermask;

    public PlanetCompletionValues PlanetCompletions;
    public int CurrentScore = 0;
    public Planet CurrentPlanet = Planet.None;

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
        ParseCurrentCompletionData(url);
#endif
    }

    private void Awake()
    {
#if !UNITY_EDITOR && UNITY_WEBGL

        string url = Application.absoluteURL;
        ParseCurrentCompletionData(url);
#endif
    }

    private void ParseCurrentCompletionData(string url)
    {
        // Step 1: Parse the URL for the data parameter
        int dataIndex = url.IndexOf("?data=") + 6; // 6 is the length of "?data="
        string encodedData = url.Substring(dataIndex);

        // Step 2: Unescape the URL
        string planetCompletionsCompiled = UnityEngine.Networking.UnityWebRequest.UnEscapeURL(encodedData);

        // Step 3: Extract each planet's completion
        int mars = int.Parse(planetCompletionsCompiled.Substring(0, 3));
        int venus = int.Parse(planetCompletionsCompiled.Substring(3, 3));
        int saturn = int.Parse(planetCompletionsCompiled.Substring(6, 3));
        int pluto = int.Parse(planetCompletionsCompiled.Substring(9, 3));
        int mercury = int.Parse(planetCompletionsCompiled.Substring(12, 3));

        // Step 4: Extract current score
        CurrentScore = int.Parse(planetCompletionsCompiled.Substring(15, 3));

        // Step 5: Extract current level
        int planetValue;
        if (int.TryParse(planetCompletionsCompiled.Substring(18, 1), out planetValue))
        {
            if (Enum.IsDefined(typeof(Planet), planetValue))
            {
                CurrentPlanet = (Planet)planetValue;
            }
            else
            {
                CurrentPlanet = Planet.None; // or handle the error as you see fit
            }
        }
        else
        {
            // Handle the error: the substring isn't a valid integer
            CurrentPlanet = Planet.None; // or handle the error as you see fit
        }

        // Step 6: Assign to the struct
        PlanetCompletionValues values = new PlanetCompletionValues
        {
            MarsCompletion = mars,
            VenusCompletion = venus,
            SaturnCompletion = saturn,
            PlutoCompletion = pluto,
            MercuryCompletion = mercury
        };

        PlanetCompletions = values;
    }

    public void EndGame()
    {
     //  Debug.Log("Game Over");
     //  var pickUpManager = ServiceLocator.Instance.GetService<PickUpManager>();
     //  var pickUpsCollected = pickUpManager.PickUpsPickedUp;
     //  Debug.Log($"You collected {pickUpsCollected} / {pickUpManager.PickUps.Count}");
     //
     //  var completionPercentage = pickUpsCollected / pickUpManager.PickUps.Count;

        var currentPlanet = GetCurrentPlanet();
        Debug.Log("End Game called, with current planet: " + currentPlanet.ToString());
        switch (currentPlanet)
        {
            case Planet.Mars:
                if (CurrentScore > PlanetCompletions.MarsCompletion) PlanetCompletions.MarsCompletion = CurrentScore;
                break;
            case Planet.Venus:
                if (CurrentScore > PlanetCompletions.VenusCompletion) PlanetCompletions.VenusCompletion = CurrentScore;
                break;
            case Planet.Saturn:
                if (CurrentScore > PlanetCompletions.SaturnCompletion) PlanetCompletions.SaturnCompletion = CurrentScore;
                break;
            case Planet.Pluto:
                if (CurrentScore > PlanetCompletions.PlutoCompletion) PlanetCompletions.PlutoCompletion = CurrentScore;
                break;
            case Planet.Mercury:
                if (CurrentScore > PlanetCompletions.MercuryCompletion) PlanetCompletions.MercuryCompletion = CurrentScore;
                break;
        }

   //     var ellaPickUpsCollected = pickUpManager.FoundEllaPickUps;
   //
   //     foreach (var pickupElla in ellaPickUpsCollected)
   //     {
   //         Debug.Log($"You collected Ella's letters: {pickupElla.ToString()}");
   //
   //         // get corresponding slot in endscreen -> pop-in the found letter(s)
   //
   //         // yield return a delay -> repeat
   //     }
        
    }

    public Planet GetCurrentPlanet()
    {
        var sceneTypeString = SceneManager.GetActiveScene().ToString();

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
