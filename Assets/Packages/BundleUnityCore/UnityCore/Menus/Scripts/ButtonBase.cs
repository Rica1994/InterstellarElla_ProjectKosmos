﻿using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityCore.Menus;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class ButtonBase : MonoBehaviour, IClickable
{
    [SerializeField]
    private float _waitTimeBeforeExecutingLogic = 0.3f;

    [Header("Button Logics to Execute")]
    [SerializeField]
    private List<ButtonExecuteBase> _buttonExecutes = new List<ButtonExecuteBase>();

    [Header("Animation stuff")]
    [SerializeField]
    protected Animation _animationComponent;
    [SerializeField]
    protected string _animationNameAppear;
    [SerializeField]
    protected string _animationNameIdle;
    [SerializeField]
    protected string _animationNameClicked;
    [SerializeField]
    protected string _animationNamePoof;

    [SerializeField]
    private bool _isIdleAfterClick;

    [Header("Audio")]
    [SerializeField]
    protected AudioElement _soundEffect;

    [Header("Trigger")]
    [SerializeField]
    private Collider _myTrigger;
    public Collider MyTrigger => _myTrigger;

    [Header("Unity Button Component")]
    [SerializeField]
    private Button _myButton;
    public Button MyButton => _myButton;

    protected AudioController _audioController;
    protected PageController _pageController;


    #region UNITY FUNCTIONS
    protected virtual void Start()
    {
        _audioController = ServiceLocator.Instance.GetService<AudioController>();
        _pageController = ServiceLocator.Instance.GetService<PageController>();

        DisableButton(true);
    }
    #endregion


    #region PUBLIC FUNCTIONS
    public virtual void Click()
    {
        ClickedButton();
    }
    public virtual void ClickedButton()
    {
        // only do all of this if there currently is not an assigned IEnumerator for buttons
        if (_pageController.ButtonRunner == null)
        {
            // player feedback
            PlayAnimation(_animationNameClicked);
            if (_isIdleAfterClick == true)
            {
                PlayAnimationQueued(_animationNameIdle);
            }
            PlaySoundEffect();

            // PROBLEM - I want to make it so I cannot press another button as long as this buttons logic is running
            // (1) (if I'm dependant on Unity Button, then I need to access all of the available ones and disable them == likely to check 4-8 buttons whenever called)
            // (2) (if I'm dependant on IClick, then I could just toggle on a bool in a possible "system" that detects clicks == one more update loop to check raycasts)
            ServiceLocator.Instance.GetService<InputManager>().EnableUiInput(false);

            // actual logic
            _pageController.ButtonRunner = ExecuteLogicRoutine();
            _pageController.StartCoroutine(_pageController.ButtonRunner);
        }
    }
    public virtual void DisableButton(bool isInstant = false)
    {
        if (isInstant == false)
        {
            // instantly disable collider and button components
            _myTrigger.enabled = false;
            _myButton.enabled = false;

            // play animation
            PlayAnimation(_animationNamePoof);

            // wait for animation to end to disable the gameobject
            float poofTime = _animationComponent.GetClip(_animationNamePoof).length;
            _pageController.StartCoroutine(DisableGameObject(poofTime));
        }
        else
        {
            // instantly disable collider and button components
            _myTrigger.enabled = false;
            _myButton.enabled = false;

            //Debug.Log("disabling button " + gameObject.name);

            //Debug.Log("accessing pagecontroller " + _pageController.name); // null for something
            

            // disable gameobject
            _pageController.StartCoroutine(DisableGameObject(0));
        }

    }
    public virtual void EnableButton()
    {
        // instantly disable collider and button components
        _myTrigger.enabled = false;
        _myButton.enabled = false;

        // enable this object
        this.gameObject.SetActive(true);

        // play animation
        PlayAnimation(_animationNameAppear);
        // play idle afterwards
        _animationComponent.PlayQueued(_animationNameIdle);

        // enable components after appear animation
        float appearTime = _animationComponent.GetClip(_animationNameAppear).length;
        _pageController.StartCoroutine(EnableComponents(appearTime));
    }
    #endregion



    #region PRIVATE FUNCTIONS
    protected virtual void ExecuteLogic()
    {
        for (int i = 0; i < _buttonExecutes.Count; i++)
        {
            _buttonExecutes[i].MyLogic();
        }
    }
    protected virtual IEnumerator ExecuteLogicRoutine()
    {
        yield return new WaitForSeconds(_waitTimeBeforeExecutingLogic);

        ExecuteLogic();
        _pageController.ButtonRunner = null;
    }
    protected virtual void PlaySoundEffect()
    {
        if (_soundEffect != null)
        {
            _audioController.PlayAudio(_soundEffect);
        }
    }
    protected virtual void PlayAnimation(string animationString)
    {
        if (_animationComponent != null)
        {
            _animationComponent.Play(animationString);
        }
    }
    protected virtual void PlayAnimationQueued(string animationString)
    {
        if (_animationComponent != null)
        {
            _animationComponent.PlayQueued(animationString);
        }
    }
    protected virtual IEnumerator DisableGameObject(float poofDuration)
    {
        yield return new WaitForSeconds(poofDuration);

        if (this.gameObject != null)
        {
            this.gameObject.SetActive(false);
        }     
    }
    protected virtual IEnumerator EnableComponents(float appearDuration)
    {
        yield return new WaitForSeconds(appearDuration);

        _myTrigger.enabled = true;
        _myButton.enabled = true;
    }
    #endregion



}
