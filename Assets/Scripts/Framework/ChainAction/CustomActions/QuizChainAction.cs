using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class QuizChainAction : ChainAction
{
    [SerializeField]
    private Quiz _quiz;
    
    [SerializeField]
    private List<Quiz.QuestionData> _questions = new List<Quiz.QuestionData>();

    private int _currentQuestionIndex = -1;

    private Tween _typeWriterTween;
    [SerializeField]
    float _typeSpeed = 15f;

    private void Start()
    {
        _quiz.OnQuestionCompleted += OnQuestionCompleted;
    }

    void OnValidate()
    {
        _useUserBasedAction = true;
    }

    private void OnQuestionCompleted()
    {
        _quiz.QuestionTransform.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
        foreach (var quizAnswer in _quiz.Answers)
        {
            Helpers.Hide(quizAnswer.transform, 0.2f, this);
        }
        
        StartCoroutine(Helpers.DoAfter(1.1f,  () => StartCoroutine(StartNextQuestion())));
    }

    private IEnumerator StartNextQuestion()
    {
        var audioController = ServiceLocator.Instance.GetService<AudioController>();
        ++_currentQuestionIndex;
        if (_currentQuestionIndex > _questions.Count - 1)
        {
            Debug.Log("All questions done");
            EndQuiz();
            yield break;
        }

        var question = _questions[_currentQuestionIndex];
        _quiz.SetupQuestion(question);

        _quiz.QuestionTransform.gameObject.SetActive(true);

        // Show the question
        string text = "";
        string questionText = _quiz.QuestionTransform.gameObject.GetComponent<TextMeshProUGUI>().text;

        _quiz.QuestionTransform.gameObject.GetComponent<TextMeshProUGUI>().text = "";
        _quiz.QuestionTransform.gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
        _typeWriterTween = DOTween.To(() => text, x => text = x, questionText, questionText.Length / _typeSpeed).OnUpdate(() => 
        {
            _quiz.QuestionTransform.gameObject.GetComponent<TextMeshProUGUI>().text = text;
        });



        // Ask the question
        if (question.AudioElement.Clip != null)
        {
            audioController.PlayAudio(question.AudioElement);
        }

        yield return new WaitForSeconds(question.AudioElement.Clip.length);

        // Show the answers
        foreach (var quizAnswer in _quiz.Answers)
        {
            quizAnswer.IsDisabled = false;
            quizAnswer.UnHighlight();
            quizAnswer.Deselect();
            Helpers.Show(quizAnswer.transform, 0.4f);
            audioController.PlayAudio(quizAnswer.AudioRecording);
            yield return new WaitForSeconds(quizAnswer.AudioRecording.Clip.length);
        }

        // Reset the question so it's interactable
        _quiz.ResetQuestion();


        // OLD
        //var audioController = ServiceLocator.Instance.GetService<AudioController>();
        //++_currentQuestionIndex;
        //if (_currentQuestionIndex > _questions.Count - 1)
        //{
        //    Debug.Log("All questions done");
        //    EndQuiz();
        //    yield break;
        //}

        //var question = _questions[_currentQuestionIndex];
        //_quiz.SetupQuestion(question);

        //// Show the question
        //Helpers.Show(_quiz.QuestionTransform, 1.0f);

        //// Ask the question
        //if (question.AudioElement.Clip != null)
        //{
        //    audioController.PlayAudio(question.AudioElement);
        //}

        //yield return new WaitForSeconds(question.AudioElement.Clip.length);

        //// Show the answers
        //foreach (var quizAnswer in _quiz.Answers)
        //{
        //    quizAnswer.IsDisabled = false;
        //    quizAnswer.UnHighlight();
        //    quizAnswer.Deselect();
        //    Helpers.Show(quizAnswer.transform, 1.0f);
        //    audioController.PlayAudio(quizAnswer.AudioRecording);
        //    yield return new WaitForSeconds(quizAnswer.AudioRecording.Clip.length);
        //}

        //// Reset the question so it's interactable
        //_quiz.ResetQuestion();
    }

    public override void Execute()
    {
        base.Execute();
        // Start the question
        Helpers.DoAfter(1.0f, () => StartCoroutine(StartNextQuestion()), this);


        // OLD
        //// Start the question
        //Helpers.Show(_quiz.transform, 1.0f);
        //Helpers.DoAfter(1.0f, () => StartCoroutine(StartNextQuestion()), this);
    }

    private void EndQuiz()
    {
        _quiz.QuestionTransform.gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
        foreach (var quizAnswer in _quiz.Answers)
        {
            Helpers.Hide(quizAnswer.transform, 0.2f, this);
        }
        Helpers.DoAfter(1.0f, () => _userBasedActionCompleted = true, this);
    }
}
