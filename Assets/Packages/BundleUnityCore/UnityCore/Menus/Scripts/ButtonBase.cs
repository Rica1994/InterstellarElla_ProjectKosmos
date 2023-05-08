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

    [Header("Animation stuff")]
    [SerializeField]
    protected Animation _animationComponent;
    [SerializeField]
    protected string _animationNameClicked;

    [Header("Audio")]
    [SerializeField]
    protected AudioElement _soundEffect;


    protected AudioController _audioInstance;
    protected PageController _pageController;



    protected virtual void Start()
    {
        _audioInstance = AudioController.Instance;
        _pageController = PageController.Instance;
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
            _audioInstance.PlayAudio(_soundEffect);
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
