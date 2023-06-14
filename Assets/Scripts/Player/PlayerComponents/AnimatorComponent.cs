using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorComponent
{
    public void SetAnimatorBool(Animator animator, string animatorBoolString, bool value)
    {
        animator.SetBool(animatorBoolString, value);
    }
}
