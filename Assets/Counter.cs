using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    [HideInInspector]
    public int counter;
    [SerializeField]
    private Text _counterText;

    public void Add()
    {
        counter++;
        _counterText.text = "Counter: " + counter.ToString();
    }

}
