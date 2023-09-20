using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizPlanetDetector : MonoBehaviour
{
    [SerializeField]
    private Text _planetText;
    // Start is called before the first frame update
    void Start()
    {
        _planetText.text = ((GameManager.Planet)GameManager.Data.LastPlanet).ToString();
    }
}
