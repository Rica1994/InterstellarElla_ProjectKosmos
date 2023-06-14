using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverComponent 
{
    public void Hover(CharacterController characterController, ref float yVelocity, float hoverUpVelocity, float maxHeight, float playerHeight)
    {
        if (yVelocity < 0)
        {
            yVelocity = 0f;
        }

        if (playerHeight < maxHeight)
        {
            yVelocity = hoverUpVelocity;
        }
        else
        {
            yVelocity = 0;
        }


        characterController.Move(new Vector3(0f, yVelocity * Time.deltaTime, 0f));
    }
}
