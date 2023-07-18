using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class QuizChainAction : ChainAction
{
    [SerializeField]
    private Quiz _quiz;
    
    [SerializeField]
    private List<Quiz.QuestionData> _questions = new List<Quiz.QuestionData>();

    private int _currentQuestionIndex = -1;
    
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
        Helpers.Hide(_quiz.QuestionTransform, 1.0f, this);
        foreach (var quizAnswer in _quiz.Answers)
        {
            Helpers.Hide(quizAnswer.transform, 1.0f, this);
        }

        StartCoroutine(Helpers.DoAfter(1.1f, StartNextQuestion));
    }

    private void StartNextQuestion()
    {
        ++_currentQuestionIndex;
        if (_currentQuestionIndex > _questions.Count - 1)
        {
            Debug.Log("All questions done");
            EndQuiz();
            return;
        }

        var question = _questions[_currentQuestionIndex];
        _quiz.SetupQuestion(question);
        
        // Show the question
        Helpers.Show(_quiz.QuestionTransform, 1.0f);
        
        // Ask the question
        if (question.AudioElement.Clip != null)
        {
            ServiceLocator.Instance.GetService<AudioController>().PlayAudio(question.AudioElement);
        }
        
        // Show the answers
        foreach (var quizAnswer in _quiz.Answers)
        {
            StartCoroutine(Helpers.DoAfter(question.AudioElement.Clip.length, () => Helpers.Show(quizAnswer.transform, 1.0f)));
        }
        
        // Reset the question so it's interactable
        _quiz.ResetQuestion();
    }
    
    public override void Execute()
    {
        base.Execute();
        // Start the question
        Helpers.Show(_quiz.transform, 1.0f);
        Helpers.DoAfter(1.0f, StartNextQuestion, this);
    }

    private void EndQuiz()
    {
        Helpers.Hide(_quiz.transform, 1.0f, this);
        Helpers.DoAfter(1.0f, () => _userBasedActionCompleted = true, this);
    }
}
