using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetReport : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private string _planet;

    void Start()
    {
        _scoreText.text = ParseScoreFromScoreValues();   
    }

    private string ParseScoreFromScoreValues()
    {
        var planetValues = GameManager.Data.PlanetCompletionValues;
        float planetValue = 0.0f;
        if (_planet == "Mars")
        {
            planetValue = planetValues.MarsCompletion;
        }
        else if (_planet == "Venus")
        {
            planetValue = planetValues.VenusCompletion;
        }
        else if (_planet == "Pluto")
        {
            planetValue = planetValues.PlutoCompletion;
        }
        else if (_planet == "Mercury")
        {
            planetValue = planetValues.MercuryCompletion;
        }
        else
        {
            planetValue = planetValues.SaturnCompletion;
        }

        return Helpers.FormatPercentageToString(planetValue);
    }
}
