using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FactSheet : MonoBehaviour
{
    [SerializeField]
    private TMP_Text[] factTexts;

    private List<Image> _images = new List<Image>();

    private void Start()
    {
        var images = GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
            if (image.color.a > 0)
            {
                _images.Add(image);
            }
        }

        ShowSheet(false, false);
    }

    public void SetupFacts(string[] facts)
    {
        factTexts = new TMP_Text[facts.Length];
        for (int i = 0; i < facts.Length; i++)
        {
            factTexts[i].text = facts[i];
        }
    }

    public void ShowSheet(bool show, bool fade)
    {
        if (show)
        {
            gameObject.SetActive(show);
        }

        if (fade)
        {
            var targetAlpha = show ? 1.0f : 0.0f;
            _images.ForEach(image => {
                var newColor = new Color(image.color.r, image.color.g, image.color.b, show ? 0.0f : 1.0f);
                image.color = newColor;
            });
            Helpers.FadeImage(_images.ToArray(), targetAlpha, 2.0f);
        }

        if (show == false)
        {
            StartCoroutine(Helpers.DoAfter(fade ? 2.0f : 0.0f, () => gameObject.SetActive(false)));
        }
    }
}
