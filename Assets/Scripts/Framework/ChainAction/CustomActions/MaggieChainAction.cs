using System;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class MaggieChainAction : ChainAction
{
    [SerializeField]
    private AudioElement _maggieVoiceClip;

    private Maggie _maggie;

    private void Start()
    {
        _maggie = FindObjectOfType<Maggie>(true);
        _maxTime = _maggie.PopUpLength + _maggie.PopDownLength + _maggieVoiceClip.Clip.length;
    }

    public override void Execute()
    {
        base.Execute();
        _maggie.gameObject.SetActive(true);
      //  StartCoroutine(PlayAudioAfter(_maggie.PopUpLength));
      StartCoroutine(DoAfter(_maggie.PopUpLength, () =>
      {
          ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_maggieVoiceClip);
          StartCoroutine(DoAfter(_maggieVoiceClip.Clip.length, () => _maggie.PopDown()));
      }));
    }

    public override void UpdateAction(float elapsedTime)
    {
        base.UpdateAction(elapsedTime);
    }

    public override void OnExit()
    {
        base.OnExit();
        _maggie.gameObject.SetActive(false);
    }

   // private IEnumerator PlayAudioAfter(float time)
   // {
   //     yield return new WaitForSeconds(time);
   //     ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_maggieVoiceClip);
   //     StartCoroutine()
   // }

    private IEnumerator DoAfter(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
    }
}