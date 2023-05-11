using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityComponent
{
    public void ApplyGravity(CharacterController characterController, ref float yVelocity, float gravity, bool isGrounded)
    {
        if (isGrounded)
        {
            if (yVelocity < 0) yVelocity = 0f;
        }
        yVelocity += gravity * Time.deltaTime;
        characterController.Move(new Vector3(0f, yVelocity * Time.deltaTime, 0f));
    }
}
