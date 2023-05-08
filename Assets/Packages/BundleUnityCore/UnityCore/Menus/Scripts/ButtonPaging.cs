using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityCore.Menus;
using UnityEngine;

public class ButtonPaging : ButtonBase
{
    [Header("Page to activate")]
    [SerializeField]
    protected PageType _turnThisPage;

    [Header("Audio additional")]
    [SerializeField]
    protected AudioElement _soundEffectOff;



    protected override void ExecuteLogic()
    {
        base.ExecuteLogic();

        TurnOnPage();
    }


    protected virtual void TurnOnPage()
    {
        if (_pageController.PageIsOn(_turnThisPage) == true)
        {
            _pageController.TurnPageOff(_turnThisPage);
        }
        else
        {
            _pageController.TurnPageOn(_turnThisPage);
        }
    }
    protected override void PlaySoundEffect()
    {
        // if this page is alrdy on...
        if (PageController.Instance.PageIsOn(_turnThisPage) == true)
        {
            _audioInstance.PlayAudio(_soundEffectOff);
        }
        else
        {
            _audioInstance.PlayAudio(_soundEffect);
        }
    }

}
