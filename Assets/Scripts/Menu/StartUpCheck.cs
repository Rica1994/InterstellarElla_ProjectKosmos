using System.Collections;
using System.Collections.Generic;
using Trisibo;
using UnityCore.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUpCheck : MonoBehaviour
{
    public void StartGame()
    {
        var sceneController = ServiceLocator.Instance.GetService<SceneController>();

        var gameData = GameManager.Data;
        SceneType toLoadScene;
        LoadSceneMode loadMode;

        if (gameData.PlanetCompletionValues.PlutoCompletion > 0 || gameData.PlanetCompletionValues.MarsCompletion > 0
            || gameData.PlanetCompletionValues.VenusCompletion > 0 || gameData.PlanetCompletionValues.SaturnCompletion > 0
            || gameData.PlanetCompletionValues.MercuryCompletion > 0)
        {
            toLoadScene = SceneType.S_MainMenu;
            loadMode = LoadSceneMode.Single;
        }
        else
        {
            toLoadScene = SceneType.S_GameStartUpScene;
            loadMode = LoadSceneMode.Additive;
        }

        ServiceLocator.Instance.GetService<SceneController>().LoadIntermissionLoading(toLoadScene, loadMode: loadMode);
    }
}
