using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Helpers
{
   /// <summary>
   /// Executes an action after time
   /// </summary>
   /// <param name="time"></param>
   /// <param name="action"></param>
   /// <returns></returns>
   public static IEnumerator DoAfter(float time, Action action)
   {
      yield return new WaitForSeconds(time);
      action.Invoke();
   }
   
   /// <summary>
   /// Calls DoAfter but uses a given monobehaviour to execute it.
   /// </summary>
   /// <param name="time"></param>
   /// <param name="action"></param>
   /// <param name="mono"></param>
   public static void DoAfter(float time, Action action, MonoBehaviour mono)
   {
      mono.StartCoroutine(DoAfter(time, action));
   }
   
   /// <summary>
   /// Shows a transform using a DoTween, within an amount of time.
   /// </summary>
   /// <param name="t"></param>
   /// <param name="time"></param>
   public static void Show(Transform t, float time)
   {
      t.localScale = Vector3.zero;
      t.gameObject.SetActive(true);
      t.DOScale(Vector3.one, time);
   }

   /// <summary>
   /// Shows a transform using a DoTween, within an amount of time.
   /// </summary>
   /// <param name="t"></param>
   /// <param name="time"></param>
   /// <param name="mono"></param>
   public static void Hide(Transform t, float time, MonoBehaviour mono)
   {
      t.DOScale(Vector3.zero, time);
      mono.StartCoroutine(DoAfter(time, () => t.gameObject.SetActive(false)));
   }

   /// <summary>
   /// Flickers a certain mesh on and off
   /// </summary>
   /// <param name="go">object that should flicker</param>
   /// <param name="flickerTimes">amount of time it should flicker</param>
   /// <param name="flickerSpeed">amount of times it should flicker per second</param>
   public static IEnumerator Flicker(GameObject go, float flickerTime, int flickerSpeed = 10)
   {
      var meshRenderers = go.GetComponentsInChildren<MeshRenderer>();
      var skinnedMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
      List<Renderer> renderers = new List<Renderer>();
      renderers.AddRange(meshRenderers);
      renderers.AddRange(skinnedMeshRenderers);
      
      if (meshRenderers.Length <= 0) yield break;

      float timeFlickering = 0.0f;
      float timeBetweenEachFlicker = 1.0f / flickerSpeed;
      
      while (timeFlickering < flickerTime)
      {
         foreach (var meshRenderer in renderers) meshRenderer.enabled = !meshRenderer.enabled;

         yield return new WaitForSeconds(timeBetweenEachFlicker);
      
         timeFlickering += timeBetweenEachFlicker;
      }
   }
    public static string FormatPercentageToString(float value)
    {
        int percentage = (int)Math.Round(value);
        return percentage.ToString("D3");
    }
}
