using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityComponent
{
    public void ApplyGravity(CharacterController characterController, ref float yVelocity, float gravity, bool isGrounded)
    {
        if (isGrounded == true)
        {
            if (yVelocity < 0)
            {
                yVelocity = 0f;
            }
        }
        yVelocity += gravity * Time.deltaTime;

        characterController.Move(new Vector3(0f, yVelocity * Time.deltaTime, 0f));
    }


    public void ApplyGravity(CharacterController characterController, ref bool canHover, ref float hoverTimer, ref float yVelocity, float gravity, bool isGrounded)
    {
        if (isGrounded == true)
        {
            if (yVelocity < 0)
            {
                yVelocity = 0f;
            }

            canHover = true;
            hoverTimer = 0;
        }
        yVelocity += gravity * Time.deltaTime;

        characterController.Move(new Vector3(0f, yVelocity * Time.deltaTime, 0f));
    }
}
