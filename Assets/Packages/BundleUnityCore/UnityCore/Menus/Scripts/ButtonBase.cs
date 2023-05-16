using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityCore.Menus;
using UnityEngine;
using UnityEngine.UI;

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
    protected string _animationNameClicked;

    [Header("Audio")]
    [SerializeField]
    protected AudioElement _soundEffect;


    protected AudioController _audioController;
    protected PageController _pageController;



    protected virtual void Start()
    {
        _audioController = ServiceLocator.Instance.GetService<AudioController>();
        _pageController = ServiceLocator.Instance.GetService<PageController>();
    }

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
            PlayAnimationPress();
            PlaySoundEffect();

            // actual logic
            _pageController.ButtonRunner = ExecuteLogicRoutine();
            _pageController.StartCoroutine(_pageController.ButtonRunner);
        }
    }

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
    protected virtual void PlayAnimationPress()
    {
        if (_animationComponent != null)
        {
            _animationComponent.Play(_animationNameClicked);
        }
    }



}
