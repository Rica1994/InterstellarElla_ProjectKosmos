using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpComponent
{
    public void Jump(ref float yVelocity, float gravity, float jumpHeight)
    {
        if (yVelocity < 0) yVelocity = 0f;
        yVelocity += Mathf.Sqrt(-2.0f * jumpHeight * gravity);
    }
}
