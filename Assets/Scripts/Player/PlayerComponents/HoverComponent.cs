using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverComponent 
{
    private float _hoverActiveTimer;
    private float _adjustedHoverVelocity;

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
            yVelocity = hoverUpVelocity / (4f);
        }

        characterController.Move(new Vector3(0f, yVelocity * Time.deltaTime, 0f));
    }

    public void HoverFromGround(CharacterController characterController, ref float yVelocity, float hoverUpVelocity, float maxHeight, float playerHeight)
    {
        _hoverActiveTimer += Time.deltaTime;

        if (_hoverActiveTimer >= 0.3f)
        {
            _adjustedHoverVelocity = hoverUpVelocity;
        }
        else if (_hoverActiveTimer >= 0.1f)
        {
            _adjustedHoverVelocity = hoverUpVelocity * (1.25f);
        }
        else
        {
            _adjustedHoverVelocity = hoverUpVelocity * (1.5f);
        }

        if (yVelocity < 0)
        {
            yVelocity = 0f;
        }


        if (playerHeight < maxHeight)
        {
            yVelocity = _adjustedHoverVelocity;
        }
        else
        {
            yVelocity = hoverUpVelocity / (4f);
        }

        characterController.Move(new Vector3(0f, yVelocity * Time.deltaTime, 0f));
    }

    public void HoverValueReset()
    {
        _hoverActiveTimer = 0;
        _adjustedHoverVelocity = 0;
    }
}
