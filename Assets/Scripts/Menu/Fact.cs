using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Fact : MonoBehaviour
{
    [SerializeField]
    private TMP_Text factText;

    [SerializeField]
    private Image _coverImage;

    private float _coverImageStartAlpha;

    private float _factTextStartAlpha;

    private void Start()
    {
        _coverImageStartAlpha = _coverImage.color.a;
        _factTextStartAlpha = factText.color.a;

        var factTextColor = factText.color;
        factText.color = new Color(factTextColor.r, factTextColor.g, factTextColor.b, 0.0f);

        ShowFact(false);
    }

    public void InitializeFact(string factString)
    {
        factText.text = factString;
    }

    public void ShowFact(bool show)
    {
        if (show == false)
        {
            Helpers.FadeImage(new Image[] { _coverImage }, 0.0f, 1.0f);
            Helpers.FadeText(new TMP_Text[] { factText }, 0.0f, 1.0f);
        }
        else
        {
            _coverImage.color = new Color(_coverImage.color.r, _coverImage.color.g, _coverImage.color.b, _coverImageStartAlpha);
            Helpers.FadeImage(new Image[] { _coverImage }, 0.0f, 1.0f);
            Helpers.FadeText(new TMP_Text[] {factText}, _factTextStartAlpha, 1.0f);
        }
    }
}
