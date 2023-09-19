using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    [HideInInspector]
    public int counter;
    [SerializeField]
    private Text _counterText;

    private void Start()
    {
        SceneManager.sceneUnloaded += OnSceneUnLoaded;
        counter = (int)GameManager.Data.CurrentScore;
        _counterText.text = counter.ToString();
    }

    public void Add()
    {
        counter++;
        GameManager.Data.CurrentScore = counter;
        _counterText.text = "Counter: " + counter.ToString();
    }

    private void OnApplicationQuit()
    {
        GameManager.Data.CurrentScore = counter;
    }
    private void OnSceneUnLoaded(Scene arg0)
    {
        GameManager.Data.CurrentScore = counter;
    }
}
