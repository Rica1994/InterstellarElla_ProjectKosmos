using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class FicheController : Service
{
    [SerializeField]
    private List<FichePlanet> _fichesPlanets = new List<FichePlanet>();
    public List<FichePlanet> FichesPlanets => _fichesPlanets;



    public void OpenCorrectFicheMenu(LevelType typeOfMenuLevel)
    {
        // disable all fiches
        for (int i = 0; i < _fichesPlanets.Count; i++)
        {
            _fichesPlanets[i].gameObject.SetActive(false);
        }

        // enable 1 fiche
        for (int i = 0; i < _fichesPlanets.Count; i++)
        {
            if (_fichesPlanets[i].TypeLevel == typeOfMenuLevel)
            {
                _fichesPlanets[i].gameObject.SetActive(true);
                break;
            }
        }
    }


    public void OpenCorrectFicheGameplay(Planet typeOfPlanet)
    {
        // disable all fiches
        for (int i = 0; i < _fichesPlanets.Count; i++)
        {
            _fichesPlanets[i].gameObject.SetActive(false);
        }

        // enable 1 fiche
        for (int i = 0; i < _fichesPlanets.Count; i++)
        {
            if (_fichesPlanets[i].TypePlanet == typeOfPlanet)
            {
                _fichesPlanets[i].gameObject.SetActive(true);
                break;
            }
        }
    }
}
