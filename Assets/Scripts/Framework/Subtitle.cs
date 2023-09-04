using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Subtitle : MonoBehaviour
{
    [SerializeField]
    private string[] _subtitles;

    public int subtitleIndex;

    private int _previousSubtitleIndex = -1;

    private void Update()
    {
        CheckForSubtitleChange();
    }

    public void CheckForSubtitleChange()
    {
        if (subtitleIndex != _previousSubtitleIndex)
        {
            GetComponent<TextMeshProUGUI>().text = _subtitles[subtitleIndex];
            _previousSubtitleIndex = subtitleIndex;
        }
    }
}
