using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Subtitle : MonoBehaviour
{
    public void SetSubtitleText(string subtitle)
    {
        GetComponent<TextMeshProUGUI>().text = subtitle;
    }
}
