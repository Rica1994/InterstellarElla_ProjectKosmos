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
        var planetValues =  ServiceLocator.Instance.GetService<GameManager>().PlanetCompletions;
        if (_planet == "Mars")
        {
            return planetValues.MarsCompletion.ToString();
        }
        else if (_planet == "Venus")
        {
            return planetValues.VenusCompletion.ToString();
        }
        else if (_planet == "Pluto")
        {
            return planetValues.PlutoCompletion.ToString();
        }
        else if (_planet == "Mercury")
        {
            return planetValues.MercuryCompletion.ToString();
        }
        else
        {
            return planetValues.SaturnCompletion.ToString();
        }

    }
}
