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

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
   override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
        ServiceLocator.Instance.GetService<AudioController>().StartCoroutine(RandomTime(animator, stateInfo));
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    IEnumerator RandomTime(Animator animator, AnimatorStateInfo stateInfo)
    {
        float randomSeconds = Random.Range(_minSeconds, _maxSeconds);
        yield return new WaitForSeconds(randomSeconds);
        animator.Play(stateInfo.shortNameHash, 1, 0);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
