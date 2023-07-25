using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Answer : MonoBehaviour
{
    public delegate void AnswerDelegate(Answer answer);

    public event AnswerDelegate SelectedEvent;
    
    [SerializeField]
    private Button _button;
    
    [SerializeField]
    private Image _borderImage;
    
    [SerializeField]
    private TMP_Text _answerText;

    [SerializeField]
    private AudioElement _audioRecording;


    private Color _originalBorderImageColor;
    private BaseEventData data;
    private bool _isSelected;

    public Button Button => _button;
    public AudioElement AudioRecording => _audioRecording;
    public bool IsSelected => _isSelected;

    
    private void Awake()
    {
        if (_borderImage== null)
        {
            Debug.LogError($"No image found on this {gameObject.name}!");
            return;
        }

        _button.onClick.AddListener(Select);
        _originalBorderImageColor = _borderImage.color;
    }

    public void Highlight(bool correct)
    {
        if (correct) _borderImage.color = Color.green;
        else _borderImage.color = Color.red;
    }

    public void UnHighlight()
    {
        _borderImage.color = _originalBorderImageColor;
    }
    
    public void Select()
    {
        SelectedEvent?.Invoke(this);
       _isSelected = true;
    }

    public void Deselect()
    {
        _isSelected = false;
    }

    public void SetAnswerText(string text)
    {
        _answerText.text = text;
    }
}
