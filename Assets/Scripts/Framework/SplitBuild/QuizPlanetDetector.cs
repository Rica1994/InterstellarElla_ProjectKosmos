using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizPlanetDetector : MonoBehaviour
{
    private GameManager.Planet _lastPlanet;

    [SerializeField]
    private GameObject _marsQuizSequence;
    [SerializeField]
    private GameObject _plutoQuizSequence;
    [SerializeField]
    private GameObject _saturnQuizSequence;
    [SerializeField]
    private GameObject _mercuryQuizSequence;
    [SerializeField]
    private GameObject _venusQuizSequence;
    void Start()
    {
        _lastPlanet = (GameManager.Planet)GameManager.Data.LastPlanet;

        switch (_lastPlanet)
        {
            case GameManager.Planet.Mars:
                _marsQuizSequence.SetActive(true);
                break;
            case GameManager.Planet.Pluto:
                _plutoQuizSequence.SetActive(true);
                break;
            case GameManager.Planet.Venus:
                _venusQuizSequence.SetActive(true);
                break;
            case GameManager.Planet.Saturn:
                _saturnQuizSequence.SetActive(true);
                break;
            case GameManager.Planet.Mercury:
                _mercuryQuizSequence.SetActive(true);
                break;
            case GameManager.Planet.None:
                Debug.LogError("Last planet was none");
                break;
        }
    }
}
