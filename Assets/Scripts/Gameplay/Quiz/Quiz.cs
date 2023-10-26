using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Quiz : MonoBehaviour
{
    #region Events

    public delegate void QuizDelegate();

    public event QuizDelegate OnQuestionCompleted;

    #endregion

    [SerializeField]
    private Answer[] _answers;

    [SerializeField]
    private Answer _correctAnswer;

    [SerializeField]
    private Button _submitButton;

    [SerializeField]
    private Transform _questionTransform;

    [SerializeField]
    private TMP_Text _questionText;

    [SerializeField]
    private AudioElement _questionStartSound;

    [SerializeField]
    private AudioElement _questionCompleteSound;

    [SerializeField]
    private AudioElement _questionFailedSound;

    [SerializeField]
    private AudioElement _endQuizSound;

    [SerializeField]
    private AudioElement _maggieComment;
    [SerializeField]
    private List<AudioClip> _maggieWrongAnswerClips = new List<AudioClip>();
    [SerializeField]
    private List<AudioClip> _maggieRightAnswerClips = new List<AudioClip>();

    [SerializeField]
    private AudioSource _maggieAudioSource;

    public AudioSource MaggieAudioSource => _maggieAudioSource;

    public Transform QuestionTransform => _questionTransform;
    public Answer[] Answers => _answers;

    [Serializable]
    public struct QuestionData
    {
        public string QuestionString;
        public AudioElement AudioElement;
        public string[] AnswerStrings;
        public Answer.AnswerData[] AnswerAnswerData;
        public int CorrectIndex;
    }

    private void Awake()
    {
        MakeInteractable(false);

        _questionTransform.gameObject.SetActive(false);
        _submitButton.gameObject.SetActive(false);
        _submitButton.onClick.AddListener(OnSubmit);
        foreach (var answer in _answers)
        {
            answer.gameObject.SetActive(false);
            answer.SelectedEvent += OnAnswerSelected;
        }
    }

    private void OnAnswerSelected(Answer answer)
    {
        if (_submitButton.gameObject.activeSelf == false)
        {
            Helpers.Show(_submitButton.transform, 0.3f);
        }

        foreach (var a in _answers)
        {
            if (a != answer) a.Deselect();
        }
    }

    private void OnSubmit()
    {
        // Deactivate answer buttons
        MakeInteractable(false);
        Helpers.Hide(_submitButton.transform, 0.3f, this);

        // Check the selected answer
        Answer selectedAnswer = null;
        foreach (var answer in _answers)
        {
            if (answer.IsSelected)
            {
                selectedAnswer = answer;
                break;
            }
        }

        if (selectedAnswer == null)
        {
            Debug.Log("No answer was selected!");
            return;
        }

        // Check whether it is correct.
        bool answeredCorrectly = _correctAnswer == selectedAnswer;
        selectedAnswer.Highlight(answeredCorrectly);
        //selectedAnswer.Button.interactable = false;

        // Act accordingly
        // draft for now to have maggie react here. maybe with want to keep the quiz a generic script? 

        if (answeredCorrectly)
        {
            Debug.Log("Answered Correctly");
            if (_questionCompleteSound != null)
                ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_questionCompleteSound);
            Helpers.DoAfter(2.5f, () => OnQuestionCompleted?.Invoke(), this);
        }
        else
        {
            Debug.Log("Answered Incorrectly");
            if (Random.Range(0, 4) == 3)
            {
                PlayMaggieComment(false);
            }

            if (_questionFailedSound != null)
                ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_questionFailedSound);

            selectedAnswer.IsDisabled = true;
            
            Helpers.DoAfter(2.5f, () =>
            {
                // reset other answers besides the disabled
                foreach (var answer in _answers)
                {
                    if (answer == selectedAnswer || answer.IsDisabled) continue;
                    answer.Button.interactable = true;
                    answer.Deselect();
                    answer.UnHighlight();
                }
                _submitButton.interactable = true;
                //ResetQuestion();
            }, this);
        }
    }

    public void ResetQuestion()
    {
        foreach (var a in _answers)
        {
            a.IsDisabled = false;
            a.Deselect();
            a.UnHighlight();
        }

        MakeInteractable(true);
    }

    private void MakeInteractable(bool makeInteractable)
    {
        _submitButton.interactable = makeInteractable;
        foreach (var a in _answers) a.Button.interactable = makeInteractable;
    }

    public void SetupQuestion(QuestionData questionData)
    {
        ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_questionStartSound);
        for (int i = 0; i < _answers.Length; i++)
        {
            _answers[i].SetAnswerText(questionData.AnswerStrings[i]);
        }

        _questionText.text = questionData.QuestionString;
        _correctAnswer = _answers[questionData.CorrectIndex];
    }

    public void PlayVo(AudioClip audioClip)
    {
        _maggieAudioSource.clip = audioClip;
        _maggieAudioSource.Play();
    }

    public void PlayMaggieComment(bool correctAnswer)
    {
        if (correctAnswer)
        {
            _maggieComment.Clip = _maggieRightAnswerClips[Random.Range(0, _maggieRightAnswerClips.Count)];
        }
        else
        {
            _maggieComment.Clip = _maggieWrongAnswerClips[Random.Range(0, _maggieWrongAnswerClips.Count)];
        }
        ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_maggieComment);
    }

    public void PlayEndSound()
    {
        ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_endQuizSound);
    }
}