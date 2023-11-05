using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class RandomLoop : StateMachineBehaviour
{
    [SerializeField]
    private float _minSeconds = 2.0f;
    [SerializeField]
    private float _maxSeconds = 4.0f;

    [SerializeField]
    private List<string> _additionalTriggers = new List<string>();

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
   override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
        if (_additionalTriggers.Count > 0 && Random.Range(0, 5) == 0)
        {
            animator.SetTrigger(_additionalTriggers[Random.Range(0, _additionalTriggers.Count)]);
        }
        else
        {
            ServiceLocator.Instance.GetService<AudioController>().StartCoroutine(RandomTime(animator, stateInfo));
        }
   }

    IEnumerator RandomTime(Animator animator, AnimatorStateInfo stateInfo)
    {
        float randomSeconds = Random.Range(_minSeconds, _maxSeconds);
        yield return new WaitForSeconds(randomSeconds);
        if (animator != null)
        {
            animator.Play(stateInfo.shortNameHash, 1, 0);
        }
    }
}
