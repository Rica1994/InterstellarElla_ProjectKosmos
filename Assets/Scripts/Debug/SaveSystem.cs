using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSystem : MonoBehaviour
{
    [SerializeField]
    private Text _dataValue;

    [SerializeField]
    private Text _inputText;

    private void Awake()
    {
        _dataValue.text = "";
    }

    public void SaveData()
    {
        PlayerPrefs.SetString("data", _inputText.text);
    }

    public void LoadData()
    {
        _dataValue.text = PlayerPrefs.GetString("data");
    }

    public void DecreaseValue()
    {
        int value = int.Parse(_inputText.text);
        _inputText.text = (value - 1).ToString();
    }

    public void IncreaseValue()
    {
        int value = int.Parse(_inputText.text);
        _inputText.text = (value + 1).ToString();
    }
}
