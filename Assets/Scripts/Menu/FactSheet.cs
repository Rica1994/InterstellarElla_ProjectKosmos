using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using static FactSheet;

public class FactSheet : MonoBehaviour
{
    [Serializable]
    public struct FactSheetData
    {
        public string[] FactStrings;
        public float CompletionPercentage;
        public int Diameter;
        public int RotationTimeInMinutes;
        public int AmountOfMoons;
        public TypeOfPlanet TypeOfPlanet;
    }

    public enum TypeOfPlanet
    {
        Aards,
        Gas,
        Dwerg
    }

    [SerializeField]
    private List<Fact> _facts;

    [SerializeField]
    private TMP_Text _diameter;

    [SerializeField]
    private TMP_Text _rotationInMinutesText;

    [SerializeField]
    private TMP_Text _amountofMoonsText;

    [SerializeField]
    private TMP_Text _isEarthPlanetText;

    [SerializeField]
    private TMP_Text _completionText;

    [SerializeField]
    private Image _completionCircleImage;

    [SerializeField]
    private float _progressBarFillSpeed;

    [SerializeField]
    private List<TMP_Text> _allText = new List<TMP_Text>();

    [SerializeField]
    private AudioClip _factPopSound;

    [SerializeField]
    private AudioClip _factPopSoundPunch;

    private FactSheetData _factSheetData;

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

    public void InitializeSheet(FactSheetData data)
    {
        _factSheetData = data;

        _amountofMoonsText.text = data.AmountOfMoons.ToString();
        _diameter.text = data.Diameter.ToString() + " km";

        int days = (data.RotationTimeInMinutes / 60) / 24;
        int daysInMinutes = days * 24 * 60;

        int hours = (data.RotationTimeInMinutes - daysInMinutes) / 60;
        int hoursInMinutes = hours * 60;

        int minutes = (data.RotationTimeInMinutes - daysInMinutes - hoursInMinutes) % 60;

        bool noDays = days < 1;
        bool noMinutes = minutes < 1;
        bool noHours = hours < 1;

        string dayString = noDays ? "" : $"{days} dagen ";
        string hourString = (noHours ? "en " : $"{hours} uur") + (noMinutes ? "" : " en");
        string minuteString = (noMinutes ? "" : $" {minutes} minuten");

        _rotationInMinutesText.text = dayString + hourString + minuteString;
        _isEarthPlanetText.text = data.TypeOfPlanet.ToString();

        _completionText.text = Mathf.Round(data.CompletionPercentage / 10.0f).ToString() + " / 10";

        for (int i = 0; i < data.FactStrings.Length; i++)
        {
            _facts[i].InitializeFact(data.FactStrings[i]);
        }
    }

    public void ShowSheet(bool show, bool fade, float time = 2.0f)
    {
        if (show)
        {
            gameObject.SetActive(show);
        }

        if (fade)
        {
            var targetAlpha = show ? 1.0f : 0.0f;
            _images.ForEach(image =>
            {
                var newColor = new Color(image.color.r, image.color.g, image.color.b, show ? 0.0f : 1.0f);
                image.color = newColor;
            });
            Helpers.FadeImage(_images.ToArray(), targetAlpha, time);
            Helpers.FadeText(_allText.ToArray(), targetAlpha, time);
        }

        if (show) StartCoroutine(Helpers.DoAfter(time, ApplyCompletion));
        else
        {
            _facts.ForEach(fact => fact.ShowFact(false));
            StartCoroutine(Helpers.DoAfter(fade ? time : 0.0f, () => gameObject.SetActive(false)));
        }
    }

    public void ApplyCompletion()
    {
        StartCoroutine(FillCircleAndShowFacts(Mathf.Round(_factSheetData.CompletionPercentage / 10.0f) / 10.0f));
    }

    private IEnumerator FillCircleAndShowFacts(float progress)
    {
        _completionCircleImage.fillAmount = 0.0f;
        var soundManager = ServiceLocator.Instance.GetService<SoundManager>();

        float passedTime = 0.0f;
        int factIndex = 0;
        int amountOfFactsToShow = (int)(_factSheetData.CompletionPercentage / 10.0f);

        float timeTillProgressFilled = progress / _progressBarFillSpeed;
        float timePerFactShown = timeTillProgressFilled / amountOfFactsToShow;

        // Show fact while the progresscircle is being filled.
        while (passedTime < timeTillProgressFilled)
        {
            _completionCircleImage.fillAmount += _progressBarFillSpeed * Time.deltaTime;
            passedTime += Time.deltaTime;

            if (passedTime >= timePerFactShown && factIndex < _facts.Count && factIndex < amountOfFactsToShow)
            {
                passedTime = 0.0f;

                soundManager.PlayNoteInMajorChord(_factPopSound, factIndex, 1.0f);
                soundManager.PlaySFX(_factPopSoundPunch, false);

                _facts[factIndex].ShowFact(true);
                factIndex++;
            }

            yield return null;
        }


        // Make sure all facts are shown
        if (factIndex < amountOfFactsToShow)
        {
            do
            {
                soundManager.PlayNoteInMajorChord(_factPopSound, factIndex, 1.0f);
                soundManager.PlaySFX(_factPopSoundPunch, false);

                _facts[factIndex].ShowFact(true);
                factIndex++;
            }
            while (factIndex < amountOfFactsToShow);
        }

        _completionCircleImage.fillAmount = progress;
    }
}
