using System;
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

    private void Awake()
    {
        ServiceLocator.Instance.GetService<SDKManager>().LoadedEvent += OnScoreLoaded;
        _counterText.text = counter.ToString();
    }

    private void OnScoreLoaded(string stateId, string loadedData)
    {
        try
        {
            int.TryParse(loadedData, out counter);
            _counterText.text = counter.ToString();
            Debug.Log("Score was loaded!");
        }
        catch (FormatException)
        {
            Console.WriteLine($"Unable to parse '{loadedData}'");
        }
    }

    public void Load()
    {
        ServiceLocator.Instance.GetService<SDKManager>().LoadScore();
    }

    public void Save()
    {
        ServiceLocator.Instance.GetService<SDKManager>().SaveScore(counter);
    }

    public void Add()
    {
        counter++;
        _counterText.text = "Counter: " + counter.ToString();
    }
}
